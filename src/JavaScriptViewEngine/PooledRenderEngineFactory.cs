using JavaScriptViewEngine.Pool;
using System;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// Handles creation of <see cref="IRenderEngine"/> instances. All methods are thread-safe.
    /// </summary>
    public class PooledRenderEngineFactory : IRenderEngineFactory
    {
        IRenderEnginePool _pool;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledRenderEngineFactory"/> class.
        /// </summary>
        public PooledRenderEngineFactory(IRenderEnginePool pool)
        {
            _pool = pool;
        }
       
        /// <summary>
        /// Gets a JavaScript engine from the pool.
        /// </summary>
        /// <returns>The JavaScript engine</returns>
        public virtual IRenderEngine RequestEngine()
        {
            EnsureValidState();
            return _pool.GetEngine();
        }

        /// <summary>
        /// Returns an engine to the pool so it can be reused
        /// </summary>
        /// <param name="engine">Engine to return</param>
        public virtual void ReturnEngine(IRenderEngine engine)
        {
            if (!_disposed)
                _pool.ReturnEngineToPool(engine);
        }
        
        /// <summary>
        /// Clean up all engines
        /// </summary>
        public virtual void Dispose()
        {
            _disposed = true;
            _pool?.Dispose();
            _pool = null;
        }

        /// <summary>
        /// Ensures that this object has not been disposed, and that no error was thrown while
        /// loading the scripts.
        /// </summary>
        public void EnsureValidState()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
