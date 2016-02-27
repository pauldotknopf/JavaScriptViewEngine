using JavaScriptViewEngine.Pool;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// Handles creation of JavaScript engines. All methods are thread-safe.
    /// </summary>
    public class JsEngineFactory : IJsEngineFactory
    {
        readonly IJsPool _pool;
        readonly IJsEngineBuilder _jsEngineBuilder;
        readonly IJsEngineInitializer _jsEngineInitializer;
        readonly ConcurrentDictionary<int, IJsEngine> _engines = new ConcurrentDictionary<int, IJsEngine>();
        bool _disposed;
        Exception _scriptLoadException;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsEngineFactory"/> class.
        /// </summary>
        public JsEngineFactory(IJsPool pool, IJsEngineBuilder jsEngineBuilder, IJsEngineInitializer jsEngineInitializer)
        {
            _pool = pool;
            _jsEngineBuilder = jsEngineBuilder;
            _jsEngineInitializer = jsEngineInitializer;
            // Reset the recycle exception on recycle. If there *are* errors loading the scripts
            // during recycle, the errors will be caught in the initializer.
            _pool.Recycled += (sender, args) => _scriptLoadException = null;
        }
       
        /// <summary>
        /// Gets the JavaScript engine for the current thread. It is recommended to use 
        /// <see cref="GetEngine"/> instead, which will pool/reuse engines.
        /// </summary>
        /// <returns>The JavaScript engine</returns>
        public virtual IJsEngine GetEngineForCurrentThread()
        {
            EnsureValidState();
            return _engines.GetOrAdd(Thread.CurrentThread.ManagedThreadId, id =>
            {
                var engine = _jsEngineBuilder.Build();
                _jsEngineInitializer.Initialize(engine);
                EnsureValidState();
                return engine;
            });
        }

        /// <summary>
        /// Disposes the JavaScript engine for the current thread.
        /// </summary>
        public virtual void DisposeEngineForCurrentThread()
        {
            IJsEngine engine;
            if (_engines.TryRemove(Thread.CurrentThread.ManagedThreadId, out engine))
                engine?.Dispose();
        }

        /// <summary>
        /// Gets a JavaScript engine from the pool.
        /// </summary>
        /// <returns>The JavaScript engine</returns>
        public virtual IJsEngine GetEngine()
        {
            EnsureValidState();
            return _pool.GetEngine();
        }

        /// <summary>
        /// Returns an engine to the pool so it can be reused
        /// </summary>
        /// <param name="engine">Engine to return</param>
        public virtual void ReturnEngineToPool(IJsEngine engine)
        {
            // This could be called from ReactEnvironment.Dispose if that class is disposed after 
            // this class. Let's just ignore this if it's disposed.
            if (!_disposed)
                _pool.ReturnEngineToPool(engine);
        }
        
        /// <summary>
        /// Clean up all engines
        /// </summary>
        public virtual void Dispose()
        {
            _disposed = true;
            foreach (var engine in _engines)
                engine.Value?.Dispose();
            _pool?.Dispose();
        }

        /// <summary>
        /// Ensures that this object has not been disposed, and that no error was thrown while
        /// loading the scripts.
        /// </summary>
        public void EnsureValidState()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (_scriptLoadException != null)
                // This means an exception occurred while loading the script (eg. syntax error in the file)
                throw _scriptLoadException;
        }
    }
}
