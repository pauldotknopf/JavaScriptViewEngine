using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JavaScriptViewEngine.Babel
{
    public interface IBabelEngine : IDisposable
    {
        string Transform(string code, BabelConfig config, string fileName = "unknown");
    }
}
