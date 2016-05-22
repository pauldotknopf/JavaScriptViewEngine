using System;
using System.Text;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The default implementing of <see cref="IJsEngine"/> that uses V8
    /// </summary>
    /// <seealso cref="JavaScriptViewEngine.IJsEngine" />
    public class VroomJsEngine : IJsEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VroomJsEngine"/> class.
        /// </summary>
        /// <exception cref="System.Exception">V8 engine context couldn't be created.</exception>
        public VroomJsEngine()
        {
           
        }

        /// <summary>
        /// Call a function with the given arguments.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public object CallFunction(string function, params object[] args)
        {
            return null;
        }

        /// <summary>
        /// Call a function with the given arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function">The function.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public T CallFunction<T>(string function, params object[] args)
        {
            return default(T);
        }

        /// <summary>
        /// Excecutes some JavaScript in the engine.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Execute(string code)
        {

        }

        /// <summary>
        /// Load the file and execute it in the engine.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void ExecuteFile(string path, Encoding encoding = null)
        {

        }

        /// <summary>
        /// Load an embedded resource and execute it in the engine.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="type"></param>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void ExecuteResource(string resourceName, Type type)
        {

        }

        /// <summary>
        /// Gets the global object, and set/read some properties from it.
        /// </summary>
        /// <param name="action">The action.</param>
        public void GetGlobal(Action<dynamic> action)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
