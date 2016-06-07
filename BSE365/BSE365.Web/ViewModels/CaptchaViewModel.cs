using Newtonsoft.Json;

namespace BSE365.ViewModels
{
    public class CaptchaViewModel
    {
        [JsonProperty("challenge")]
        public string Challenge { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }
    }
}