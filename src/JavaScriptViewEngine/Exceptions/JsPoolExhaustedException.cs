using System;

namespace JavaScriptViewEngine.Exceptions
{
    /// <summary>
	/// Thrown when no engines are available in the pool.
	/// </summary>
    public class JsPoolExhaustedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsPoolExhaustedException"/> class.
        /// </summary>
        public JsPoolExhaustedException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsPoolExhaustedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public JsPoolExhaustedException(string message) : base(message) { }
    }
}
