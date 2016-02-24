using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JavaScriptViewEngine.Babel
{
	public class BabelConfig
    {
        public BabelConfig()
        {
            Presets = new HashSet<string> { "es2015-no-commonjs", "stage-1" };
            Plugins = new HashSet<string>();
        }
        
        public ISet<string> Plugins { get; set; }
        
        public ISet<string> Presets { get; set; }
        
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }
    }
}
