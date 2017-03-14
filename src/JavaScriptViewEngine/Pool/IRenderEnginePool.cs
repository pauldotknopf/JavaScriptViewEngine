using System;

namespace JavaScriptViewEngine.Pool
{
    /// <summary>
	/// Handles acquiring <see cref="IRenderEngine"/> instances from a shared pool. This class is thread safe.
	/// </summary>
	public interface IRenderEnginePool : IDisposable
    {
        /// <summary>
        /// Gets an engine from the pool. This engine should be returned to the pool via
        /// <see cref="IRenderEnginePool.ReturnEngineToPool"/> when you are finished with it.
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
        IRenderEngine GetEngine(TimeSpan? timeout = null);

        /// <summary>
        /// Returns an engine to the pool so it can be reused
        /// </summary>
        /// <param name="engine">Engine to return</param>
        void ReturnEngineToPool(IRenderEngine engine);

        /// <summary>
        /// Gets the total number of engines in this engine pool, including engines that are
        /// currently busy.
        /// </summary>
        int EngineCount { get; }

        /// <summary>
        /// Gets the number of currently available engines in this engine pool.
        /// </summary>
        int AvailableEngineCount { get; }

        /// <summary>
        /// Disposes the specified engine
        /// </summary>
        /// <param name="engine">Engine to dispose</param>
        /// <param name="repopulateEngines">
        /// If <c>true</c>, a new engine will be created to replace the disposed engine
        /// </param>
        void DisposeEngine(IRenderEngine engine, bool repopulateEngines = true);

        /// <summary>
        /// Disposes all engines in this pool, and creates new engines in their place.
        /// </summary>
        void Recycle();

        /// <summary>
		/// Occurs when any watched files have changed (including renames and deletions).
		/// </summary>
		event EventHandler Recycled;
    }
}
