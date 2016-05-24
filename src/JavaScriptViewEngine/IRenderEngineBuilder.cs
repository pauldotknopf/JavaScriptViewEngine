namespace JavaScriptViewEngine
{
    /// <summary>
    /// Used to build a new/clean instance of a IRenderEngine engine.
    /// Implement this if you wish to use a custom engine.
    /// </summary>
    public interface IRenderEngineBuilder
    {
        /// <summary>
        /// Build a new clean IRenderEngine.
        /// </summary>
        /// <returns></returns>
        IRenderEngine Build();
    }
}
