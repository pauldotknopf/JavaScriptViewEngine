using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VroomJs;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The default implementing of <see cref="IJsEngine"/> that uses V8
    /// </summary>
    /// <seealso cref="JavaScriptViewEngine.IJsEngine" />
    public class VroomJsEngine : IJsEngine
    {
        private static readonly Lazy<JsEngine> JsEngine = new Lazy<JsEngine>(() => new JsEngine());
        private readonly JsContext _context;
        private readonly object _lock = new object();
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VroomJsEngine"/> class.
        /// </summary>
        /// <exception cref="System.Exception">V8 engine context couldn't be created.</exception>
        public VroomJsEngine()
        {
            try
            {
                _context = JsEngine.Value.CreateContext();
            }
            catch (Exception ex)
            {
                throw new Exception("V8 engine context couldn't be created.", ex);
            }

            _context.Execute("");
            _context.SetVariable("console", new EngineConsole());
        }

        /// <summary>
        /// Call a function with the given arguments.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public object CallFunction(string function, params object[] args)
        {
            VerifyNotDisposed();

            var code = $"{function}({string.Join(", ", args.Select(JsonConvert.SerializeObject))})";

            return _context.Execute(code);
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
            VerifyNotDisposed();

            return (T)CallFunction(function, args);
        }

        /// <summary>
        /// Excecutes some JavaScript in the engine.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Execute(string code)
        {
            VerifyNotDisposed();

            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            _context.Execute(code);

        }

        /// <summary>
        /// Load the file and execute it in the engine.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void ExecuteFile(string path, Encoding encoding = null)
        {
            VerifyNotDisposed();

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var code = Utils.GetFileTextContent(path, encoding);
            Execute(code);
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
            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentException(nameof(resourceName) + " is empty.");

            if (type == null)
                throw new ArgumentNullException(nameof(type) + " is null");

            var code = Utils.GetResourceAsString(resourceName, type);
            Execute(code);
        }
        
        /// <summary>
        /// Verifies the not disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        protected void VerifyNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                lock(_lock)
                {
                    if (_disposed) return;
                    _disposed = true;
                }

                _context.Dispose();
            }
        }

        #region Nested Types

        class EngineConsole
        {
            public void log(string args)
            {
                Console.WriteLine("console.log(\"" + args + "\")");
            }

            public void log(dynamic arg)
            {
                Console.WriteLine("console.log(\"" + JsonConvert.SerializeObject(arg) + "\")");
            }
        }

        #endregion
    }
}
