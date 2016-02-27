using JavaScriptViewEngine.Exceptions;
using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JavaScriptViewEngine.Pool
{
    /// <summary>
	/// Handles acquiring JavaScript engines from a shared pool. This class is thread-safe.
	/// </summary>
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class JsPool : IJsPool
    {
        #region Fields

        readonly JsPoolOptions _options;
        readonly IJsEngineInitializer _jsEngineInitializer;
        readonly IJsEngineBuilder _jsEngineBuilder;
        readonly BlockingCollection<IJsEngine> _availableEngines = new BlockingCollection<IJsEngine>();
        readonly IDictionary<IJsEngine, EngineMetadata> _metadata = new Dictionary<IJsEngine, EngineMetadata>();
        int _engineCount;
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        readonly object _engineCreationLock = new object();

        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new JavaScript engine pool
        /// </summary>
        public JsPool(IOptions<JsPoolOptions> options, IJsEngineInitializer jsEngineInitializer, IJsEngineBuilder jsEngineBuilder)
        {
            _options = options.Value;
            _jsEngineInitializer = jsEngineInitializer;
            _jsEngineBuilder = jsEngineBuilder;
            PopulateEngines();
        }

        #endregion
        
        #region IJsPool

        /// <summary>
        /// Gets an engine from the pool. This engine should be returned to the pool via
        /// <see cref="ReturnEngineToPool"/> when you are finished with it.
        /// If an engine is free, this method returns immediately with the engine.
        /// If no engines are available but we have not reached <see cref="JsPoolOptions.MaxEngines"/>
        /// yet, creates a new engine. If MaxEngines has been reached, blocks until an engine is
        /// avaiable again.
        /// </summary>
        /// <param name="timeout">
        /// Maximum time to wait for a free engine. If not specified, defaults to the timeout 
        /// specified in the configuration.
        /// </param>
        /// <returns>A JavaScript engine</returns>
        /// <exception cref="JsPoolExhaustedException">
        /// Thrown if no engines are available in the pool within the provided timeout period.
        /// </exception>
        public virtual IJsEngine GetEngine(TimeSpan? timeout = null)
        {
            IJsEngine engine;

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
                throw new JsPoolExhaustedException($"Could not acquire JavaScript engine within {_options.GetEngineTimeout}");

            return TakeEngine(engine);
        }

        /// <summary>
        /// Returns an engine to the pool so it can be reused
        /// </summary>
        /// <param name="engine">Engine to return</param>
        public virtual void ReturnEngineToPool(IJsEngine engine)
        {
            if (!_metadata.ContainsKey(engine))
            {
                // This engine was from another pool. This could happen if a pool is recycled
                // and replaced with a different one (like what ReactJS.NET does when any 
                // loaded files change). Let's just pretend we never saw it.
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

            // TODO: VroomJs doesn't expose an garbage collection
            //if (
            //    _config.GarbageCollectionInterval > 0 &&
            //    usageCount % _config.GarbageCollectionInterval == 0 &&
            //    engine.SupportsGarbageCollection()
            //)
            //{
            //    engine.CollectGarbage();
            //}

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
        public virtual void DisposeEngine(IJsEngine engine, bool repopulateEngines = true)
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
        /// Disposes all the JavaScript engines in this pool.
        /// </summary>
        public virtual void Dispose()
        {
            DisposeAllEngines();
            _cancellationTokenSource.Cancel();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Ensures that at least <see cref="JsPoolOptions.StartEngines"/> engines have been created.
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
        /// Creates a new JavaScript engine and adds it to the list of all available engines.
        /// </summary>
        private IJsEngine CreateEngine()
        {
            var engine = _jsEngineBuilder.Build();
            _jsEngineInitializer.Initialize(engine);
            _metadata[engine] = new EngineMetadata();
            Interlocked.Increment(ref _engineCount);
            return engine;
        }

        /// <summary>
        /// Marks the specified engine as "in use"
        /// </summary>
        /// <param name="engine"></param>
        private IJsEngine TakeEngine(IJsEngine engine)
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
            IJsEngine engine;
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
