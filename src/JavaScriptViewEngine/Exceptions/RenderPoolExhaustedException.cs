using System;

namespace JavaScriptViewEngine.Exceptions
{
    /// <summary>
	/// Thrown when no engines are available in the pool.
	/// </summary>
    public class RenderPoolExhaustedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPoolExhaustedException"/> class.
        /// </summary>
        public RenderPoolExhaustedException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPoolExhaustedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RenderPoolExhaustedException(string message) : base(message) { }
    }
}
