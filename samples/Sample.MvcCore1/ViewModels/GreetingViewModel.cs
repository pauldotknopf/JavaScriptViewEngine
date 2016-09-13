using Newtonsoft.Json;

namespace Sample.MvcCore1.ViewModels
{
    public class GreetingViewModel
    {
        [JsonProperty("greeting")]
        public string Greeting { get; set; }
    }
}
