using System;
using System.Text;

namespace JavaScriptViewEngine
{
    public interface IJsEngine : IDisposable
    {
        object CallFunction(string function, params object[] args);

        T CallFunction<T>(string function, params object[] args);

        void Execute(string code);

        void ExecuteFile(string path, Encoding encoding = null);

        void ExecuteResource(string resourceName, Type type);
    }
}
