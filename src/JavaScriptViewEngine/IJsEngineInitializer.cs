namespace JavaScriptViewEngine
{
    /// <summary>
    /// The initializer that will "ready-up" the JavaScript, adding the required
    /// methods to render full/partial views.
    /// This must be implemented in each implementation.
    /// </summary>
    public interface IJsEngineInitializer
    {
        /// <summary>
        /// Initializes the specified engine.
        /// </summary>
        /// <param name="engine">The engine.</param>
        void Initialize(IRenderEngine engine);
    }
}
