using System;
using System.Text;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// An abstracted JavaScript engine.
    /// </summary>
    public interface IJsEngine : IDisposable
    {
        /// <summary>
        /// Call a function with the given arguments.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        object CallFunction(string function, params object[] args);

        /// <summary>
        /// Call a function with the given arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function">The function.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        T CallFunction<T>(string function, params object[] args);

        /// <summary>
        /// Excecutes some JavaScript in the engine.
        /// </summary>
        /// <param name="code">The code.</param>
        void Execute(string code);

        /// <summary>
        /// Load the file and execute it in the engine.
        /// </summary>
        void ExecuteFile(string path, Encoding encoding = null);

        /// <summary>
        /// Load an embedded resource and execute it in the engine.
        /// </summary>
        void ExecuteResource(string resourceName, Type type);
    }
}
