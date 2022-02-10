using System.Text.Json.Serialization;

namespace Authentication.Lib.Config
{
    public class Response
    {
        [JsonIgnore]
        public int StatusCode { get; set; }
        public string Error { get; set; }
    }
}