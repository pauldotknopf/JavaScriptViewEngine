using System;

namespace JavaScriptViewEngine
{
    /// <summary>
	/// Handles creation of <see cref="IRenderEngine"/> instances. All methods are thread-safe.
	/// </summary>
    public interface IRenderEngineFactory : IDisposable
    {
        /// <summary>
        /// Gets a <see cref="IRenderEngine"/> engine from the factory.
        /// It may be pooled, it may be a singlton. Depends upon the
        /// implementation
        /// </summary>
        /// <returns>The <see cref="IRenderEngine"/></returns>
        IRenderEngine RequestEngine();

        /// <summary>
        /// Returns an <see cref="IRenderEngine"/> to the factory.
        /// If this is a singleton factory, nothing is done.
        /// If this is a pooled factory, the pool is returned
        /// to the pool for re-use.
        /// </summary>
        /// <param name="engine">Engine to return</param>
        void ReturnEngine(IRenderEngine engine);
    }
}
