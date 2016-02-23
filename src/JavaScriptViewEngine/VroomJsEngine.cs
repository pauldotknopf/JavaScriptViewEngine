using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VroomJs;

namespace JavaScriptViewEngine
{
    public class VroomJsEngine : IJsEngine
    {
        private static readonly Lazy<JsEngine> JsEngine = new Lazy<JsEngine>(() => new JsEngine());
        private readonly JsContext _context;
        private readonly object _lock = new object();
        private bool _disposed = false;
        
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
        }

        public object CallFunction(string function, params object[] args)
        {
            VerifyNotDisposed();

            var code = $"{function}({string.Join(", ", args.Select(JsonConvert.SerializeObject))})";

            return _context.Execute(code);
        }

        public T CallFunction<T>(string function, params object[] args)
        {
            VerifyNotDisposed();

            return (T)CallFunction(function, args);
        }

        public void Execute(string code)
        {
            VerifyNotDisposed();

            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            _context.Execute(code);
        }

        public void ExecuteFile(string path, Encoding encoding = null)
        {
            VerifyNotDisposed();

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var code = Utils.GetFileTextContent(path, encoding);
            Execute(code);
        }

        public void ExecuteResource(string resourceName, Type type)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentException(nameof(resourceName) + " is empty.");

            if (type == null)
                throw new ArgumentNullException(nameof(type) + " is null");

            var code = Utils.GetResourceAsString(resourceName, type);
            Execute(code);
        }

        protected void VerifyNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());
        }

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
    }
}
