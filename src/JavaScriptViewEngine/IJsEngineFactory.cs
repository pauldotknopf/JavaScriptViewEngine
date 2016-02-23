using System;

namespace JavaScriptViewEngine
{
    public interface IJsEngineFactory : IDisposable
    {
		IJsEngine GetEngineForCurrentThread();
        
        void DisposeEngineForCurrentThread();
    }
}
