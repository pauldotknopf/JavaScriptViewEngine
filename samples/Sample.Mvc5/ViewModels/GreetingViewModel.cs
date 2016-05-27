using Newtonsoft.Json;

namespace Sample.Mvc5.ViewModels
{
    public class GreetingViewModel
    {
        [JsonProperty("greeting")]
        public string Greeting { get; set; }
    }
}