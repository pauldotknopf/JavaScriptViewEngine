using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// The options to use when using the node renderer.
    /// </summary>
    public class NodeRenderEngineOptions
    {
        /// <summary>
        /// The project directory that node will be started in.
        /// If you are using npm modules, the modules must be in
        /// this directory.
        /// </summary>
        public string ProjectDirectory { get; set; }

        /// <summary>
        /// Areas in MVC are translated to the file to invoke in node.
        /// "default" is default. If you would like to invoke another
        /// script based on a route value, implement that logic here.
        /// </summary>
        public Func<string, string> GetArea = (area) => area;
    }
}
