using System;

namespace JavaScriptViewEngine
{
    /// <summary>
	/// Handles creation of <see cref="IRenderEngine"/> instances. All methods are thread-safe.
	/// </summary>
    public interface IRenderEngineFactory : IDisposable
    {
        /// <summary>
        /// Gets a <see cref="IRenderEngine"/> engine from the pool.
        /// </summary>
        /// <returns>The <see cref="IRenderEngine"/></returns>
        IRenderEngine GetEngine();

        /// <summary>
        /// Returns an <see cref="IRenderEngine"/> to the pool so it can be reused
        /// </summary>
        /// <param name="engine">Engine to return</param>
        void ReturnEngineToPool(IRenderEngine engine);
    }
}
