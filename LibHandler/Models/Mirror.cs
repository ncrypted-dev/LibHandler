using System.Text.Json.Serialization;

namespace LibHandler.Models
{
    public class Mirror
    {
        public string Url { get; set; }
        public string FullUrl { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MirrorType MirrorType { get; set; }
        public string Path { get; set; }
        public long LastResponseTime { get; set; }

        public Mirror()
        {
            Url = String.Empty;
            FullUrl = String.Empty;
            MirrorType = MirrorType.None;
            Path = String.Empty;
            LastResponseTime = 10000;
        }
    }

    public enum MirrorType
    {
        SearchMirror,
        DownloadMirror,
        None
    }

}