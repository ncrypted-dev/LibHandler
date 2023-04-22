using System.Text.Json;
using System.Net.NetworkInformation;
using System.Reflection;

namespace LibHandler.Util
{
    public class Mirror
    {
        public string Url { get; set; }
        public string FullUrl { get; set; }
        public string DownloadUrl { get; set; }
        public long LastResponseTime { get; set; }

        public Mirror()
        {
            Url = String.Empty;
            FullUrl = String.Empty;
            DownloadUrl = String.Empty;
            LastResponseTime = 10000;
        }
    }

    internal static class MirrorHandler
    {
        private static List<Mirror> Mirrors { get; set; }
        public static Mirror MainMirror { get; set; }

        static MirrorHandler()
        {
            StreamReader reader = new StreamReader(
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(DataHandler.MIRROR_CONFIG_PATH) ?? Stream.Null);

            //string config = File.ReadAllText(DataHandler.MIRROR_CONFIG_PATH);
            string config = reader.ReadToEnd();
            Mirrors = JsonSerializer.Deserialize<List<Mirror>>(config) ?? new List<Mirror>();

            MainMirror = new Mirror();
            MainMirror = GetMainMirror();
        }

        public static void RemoveMirror(Mirror m)
            => Mirrors.Remove(m);
        public static void RemoveMirror(int m)
            => Mirrors.RemoveAt(m);
        public static void ReplaceMainMirror()
        {
            Mirrors.Remove(MainMirror);
            MainMirror = GetMainMirror(true);
        }

        public static Mirror GetMainMirror(bool refresh = false)
        {
            if (MainMirror.Url != String.Empty && !refresh) return MainMirror;

            Ping ping = new Ping();

            Mirror optimalMirror = new Mirror();

            for (int i = 0; i < Mirrors.Count; i++)
            {
                PingReply reply = ping.Send(Mirrors[i].Url);
                if (reply.Status != IPStatus.Success) continue;

                if (reply.RoundtripTime < optimalMirror.LastResponseTime)
                {
                    Mirrors[i].LastResponseTime = reply.RoundtripTime;
                    optimalMirror = Mirrors[i];
                }
            }

            return optimalMirror;
        }
    }
}
