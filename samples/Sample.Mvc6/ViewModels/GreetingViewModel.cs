using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sample.Mvc6.ViewModels
{
    public class GreetingViewModel
    {
        [JsonProperty("greeting")]
        public string Greeting { get; set; }
    }
}
