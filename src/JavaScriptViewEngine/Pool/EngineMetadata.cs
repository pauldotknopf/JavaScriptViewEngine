namespace JavaScriptViewEngine.Pool
{
    /// <summary>
	/// Contains metadata relating to a render engine.
	/// </summary>
	public class EngineMetadata
    {
        /// <summary>
        /// Gets or sets whether this render engine is currently in use.
        /// </summary>
        public bool InUse { get; set; }

        /// <summary>
        /// Gets or sets the number of times this engine has been used.
        /// </summary>
        public int UsageCount { get; set; }
    }
}
