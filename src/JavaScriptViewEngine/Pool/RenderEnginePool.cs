using JavaScriptViewEngine.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
#if DI
using Microsoft.Extensions.Options;
#endif

namespace JavaScriptViewEngine.Pool
{
    /// <summary>
	/// Handles acquiring render engines from a shared pool. This class is thread-safe.
	/// </summary>
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class RenderEnginePool : IRenderEnginePool
    {
#region Fields

        readonly RenderPoolOptions _options;
        readonly IRenderEngineBuilder _renderEngineBuilder;
        readonly IFileWatcher _fileWatcher;
        readonly BlockingCollection<IRenderEngine> _availableEngines = new BlockingCollection<IRenderEngine>();
        readonly IDictionary<IRenderEngine, EngineMetadata> _metadata = new Dictionary<IRenderEngine, EngineMetadata>();
        int _engineCount;
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        readonly object _engineCreationLock = new object();

#endregion

#region Ctor

        /// <summary>
        /// Creates a new <see cref="RenderEnginePool"/>
        /// </summary>
        public RenderEnginePool(
            IOptions<RenderPoolOptions> options, 
            IRenderEngineBuilder renderEngineBuilder,
            IFileWatcher fileWatcher)
        {
            _options = options.Value;
            _renderEngineBuilder = renderEngineBuilder;
            _fileWatcher = fileWatcher;
            PopulateEngines();
            InitializeWatcher();
        }

#endregion

#region IRenderEnginePool

        /// <summary>
        /// Gets an engine from the pool. This engine should be returned to the pool via
        /// <see cref="ReturnEngineToPool"/> when you are finished with it.
        /// If an engine is free, this method returns immediately with the engine.
        /// If no engines are available but we have not reached <see cref="RenderPoolOptions.MaxEngines"/>
        /// yet, creates a new engine. If MaxEngines has been reached, blocks until an engine is
        /// avaiable again.
        /// </summary>
        /// <param name="timeout">
        /// Maximum time to wait for a free engine. If not specified, defaults to the timeout 
        /// specified in the configuration.
        /// </param>
        /// <returns>A render engine</returns>
        /// <exception cref="RenderPoolExhaustedException">
        /// Thrown if no engines are available in the pool within the provided timeout period.
        /// </exception>
        public virtual IRenderEngine GetEngine(TimeSpan? timeout = null)
        {
            IRenderEngine engine;

            // First see if a pooled engine is immediately available
            if (_availableEngines.TryTake(out engine))
            {
                return TakeEngine(engine);
            }

            // If we're not at the limit, a new engine can be added immediately
            if (EngineCount < _options.MaxEngines)
            {
                lock (_engineCreationLock)
                {
                    if (EngineCount < _options.MaxEngines)
                    {
                        return TakeEngine(CreateEngine());
                    }
                }
            }

            // At the limit, so block until one is available
            if (!_availableEngines.TryTake(out engine, timeout ?? _options.GetEngineTimeout))
                throw new RenderPoolExhaustedException($"Could not acquire render engine within {_options.GetEngineTimeout}");

            return TakeEngine(engine);
        }

        /// <summary>
        /// Returns an engine to the pool so it can be reused
        /// </summary>
        /// <param name="engine">Engine to return</param>
        public virtual void ReturnEngineToPool(IRenderEngine engine)
        {
            if (!_metadata.ContainsKey(engine))
            {
                // This engine was from another pool. This could happen if a pool is recycled
                // and replaced with a different one. Let's just pretend we never saw it.
                engine.Dispose();
                return;
            }

            _metadata[engine].InUse = false;
            var usageCount = _metadata[engine].UsageCount;
            if (_options.MaxUsagesPerEngine > 0 && usageCount >= _options.MaxUsagesPerEngine)
            {
                // Engine has been reused the maximum number of times, recycle it.
                DisposeEngine(engine);
                return;
            }
            
            _availableEngines.Add(engine);
        }

        /// <summary>
        /// Gets the total number of engines in this engine pool, including engines that are
        /// currently busy.
        /// </summary>
        public virtual int EngineCount => _engineCount;

        /// <summary>
        /// Gets the number of currently available engines in this engine pool.
        /// </summary>
        public virtual int AvailableEngineCount => _availableEngines.Count;

        /// <summary>
        /// Disposes the specified engine.
        /// </summary>
        /// <param name="engine">Engine to dispose</param>
        /// <param name="repopulateEngines">
        /// If <c>true</c>, a new engine will be created to replace the disposed engine
        /// </param>
        public virtual void DisposeEngine(IRenderEngine engine, bool repopulateEngines = true)
        {
            engine.Dispose();
            _metadata.Remove(engine);
            Interlocked.Decrement(ref _engineCount);

            if (repopulateEngines)
            {
                // Ensure we still have at least the minimum number of engines.
                PopulateEngines();
            }
        }
        
        /// <summary>
        /// Disposes all engines in this pool, and creates new engines in their place.
        /// </summary>
        public virtual void Recycle()
        {
            DisposeAllEngines();
            PopulateEngines();

            var recycled = Recycled;
            recycled?.Invoke(this, null);
        }

        /// <summary>
        /// Occurs when any watched files have changed (including renames and deletions).
        /// </summary>
        public event EventHandler Recycled;

        /// <summary>
        /// Disposes all the render engines in this pool.
        /// </summary>
        public virtual void Dispose()
        {
            DisposeAllEngines();
            _cancellationTokenSource.Cancel();
            _fileWatcher?.Dispose();
        }

#endregion

#region Methods

        /// <summary>
		/// Initializes a <see cref="FileWatcher"/> if enabled in the configuration.
		/// </summary>
		private void InitializeWatcher()
        {
            if (!string.IsNullOrEmpty(_options.WatchPath))
            {
                _fileWatcher.DebounceTimeout = _options.WatchDebounceTimeout;
                _fileWatcher.Path = _options.WatchPath;
                _fileWatcher.Files = _options.WatchFiles;
                _fileWatcher.Changed += (sender, args) => Recycle();
                _fileWatcher.Start();
            }
        }

        /// <summary>
        /// Ensures that at least <see cref="RenderPoolOptions.StartEngines"/> engines have been created.
        /// </summary>
        private void PopulateEngines()
        {
            while (EngineCount < _options.StartEngines)
            {
                var engine = CreateEngine();
                _availableEngines.Add(engine);
            }
        }

        /// <summary>
        /// Creates a new render engine and adds it to the list of all available engines.
        /// </summary>
        private IRenderEngine CreateEngine()
        {
            var engine = _renderEngineBuilder.Build();
            _metadata[engine] = new EngineMetadata();
            Interlocked.Increment(ref _engineCount);
            return engine;
        }

        /// <summary>
        /// Marks the specified engine as "in use"
        /// </summary>
        /// <param name="engine"></param>
        private IRenderEngine TakeEngine(IRenderEngine engine)
        {
            _metadata[engine].InUse = true;
            _metadata[engine].UsageCount++;
            return engine;
        }

        /// <summary>
        /// Disposes all engines in this pool. Note that this will only dispose the engines that 
        /// are *currently* available. Engines that are in use will be disposed when the user
        /// attempts to return them.
        /// </summary>
        private void DisposeAllEngines()
        {
            IRenderEngine engine;
            while (_availableEngines.TryTake(out engine))
                DisposeEngine(engine, false);

            // Also clear out all metadata so engines that are currently in use while this disposal is 
            // happening get disposed on return.
            _metadata.Clear();
            _engineCount = 0;
        }
        
#endregion

#region Statistics and debugging

        // ReSharper disable once UnusedMember.Local
        /// <summary>
        /// Gets a string for displaying this engine pool in the Visual Studio debugger.
        /// </summary>
        private string DebuggerDisplay => $"Engines = {EngineCount}, Available = {AvailableEngineCount}, Max = {_options.MaxEngines}";

#endregion
    }
}
