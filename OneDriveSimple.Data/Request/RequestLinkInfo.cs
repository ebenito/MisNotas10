using Newtonsoft.Json;

namespace OneDriveSimple.Request
{
    public class RequestLinkInfo
    {
        [JsonProperty("type")]
        public string Type
        {
            get;
            set;
        }
    }
}