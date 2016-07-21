using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JavaScriptViewEngine.Pool;

namespace JavaScriptViewEngine
{
    public class SingletonRenderEngineFactory: IRenderEngineFactory
    {
        IRenderEngine _renderEngine;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledRenderEngineFactory" /> class.
        /// </summary>
        /// <param name="renderEngineBuilder">The render engine builder.</param>
        public SingletonRenderEngineFactory(IRenderEngineBuilder renderEngineBuilder)
        {
            _renderEngine = renderEngineBuilder.Build();
        }

        /// <summary>
        /// Gets a <see cref="IRenderEngine" /> engine from the pool.
        /// </summary>
        /// <returns>
        /// The <see cref="IRenderEngine" />
        /// </returns>
        public virtual IRenderEngine RequestEngine()
        {
            EnsureValidState();
            return _renderEngine;
        }

        /// <summary>
        /// Returns an <see cref="IRenderEngine" /> to the pool so it can be reused
        /// </summary>
        /// <param name="engine">Engine to return</param>
        public virtual void ReturnEngine(IRenderEngine engine)
        {
            // no pooling
        }

        /// <summary>
        /// Dispose of the render engine
        /// </summary>
        public virtual void Dispose()
        {
            _disposed = true;
            _renderEngine?.Dispose();
            _renderEngine = null;
        }

        /// <summary>
        /// Ensures that this object isn't disposed
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public void EnsureValidState()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
