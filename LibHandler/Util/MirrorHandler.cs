using System.Text.Json;
using System.Net.NetworkInformation;
using System.Reflection;
using LibHandler.Models;
using System.Net;

namespace LibHandler.Util
{
    internal static class MirrorHandler
    {
        private static List<Mirror> SearchMirrors { get; set; }
        private static List<Mirror> DownloadMirrors { get; set; }

        public static Mirror MainSearchMirror { get; set; }
        public static Mirror MainDownloadMirror { get; set; }

        static MirrorHandler()
        {
            StreamReader reader = new StreamReader(
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(DataHandler.MIRROR_CONFIG_PATH) ?? Stream.Null);

            string config = reader.ReadToEnd();
            List<Mirror> mirror = JsonSerializer.Deserialize<List<Mirror>>(config) ?? new List<Mirror>();

            SearchMirrors = mirror.Where(m => m.MirrorType.Equals(MirrorType.SearchMirror)).ToList();
            DownloadMirrors = mirror.Where(m => m.MirrorType.Equals(MirrorType.DownloadMirror)).ToList();

            SetMirrorsResponseTimes(SearchMirrors);
            SetMirrorsResponseTimes(DownloadMirrors);

            MainSearchMirror = GetOptimalMirror(SearchMirrors);
            MainDownloadMirror = GetOptimalMirror(DownloadMirrors);
        }

        public static void RemoveMirror(Mirror m)
        {
            if (m.MirrorType == MirrorType.SearchMirror) SearchMirrors.Remove(m);
            else if (m.MirrorType == MirrorType.DownloadMirror) DownloadMirrors.Remove(m);
        }

        public static void SetMainMirror(Mirror m)
        {
            if (m.MirrorType.Equals(MirrorType.SearchMirror)) MainSearchMirror = m;
            else if (m.MirrorType.Equals(MirrorType.DownloadMirror)) MainDownloadMirror = m;

        }
    
        public static void ReplaceMainMirror(MirrorType type)
        {
            switch (type)
            {
                case MirrorType.SearchMirror:
                    SearchMirrors.Remove(MainSearchMirror);
                    MainSearchMirror = GetOptimalMirror(SearchMirrors);
                    break;
                case MirrorType.DownloadMirror:
                    DownloadMirrors.Remove(MainDownloadMirror);
                    MainDownloadMirror = GetOptimalMirror(DownloadMirrors);
                    break;
            }
        }

        public static Mirror GetMainMirror(MirrorType type)
        {
            Mirror m = type switch
            {
                MirrorType.SearchMirror => MainSearchMirror,
                MirrorType.DownloadMirror => MainDownloadMirror,
                _ => throw new ArgumentOutOfRangeException(nameof(type), "Please provide a valid MirrorType")
            };

            return m;
        }

        public static void SetMirrorsResponseTimes(List<Mirror> mirrors)
        {
            Ping ping = new Ping();

            for (int i = 0; i < mirrors.Count; i++)
            {
                PingReply reply = ping.Send(mirrors[i].Url);
                Console.WriteLine(reply.Status);
                if (reply.Status != IPStatus.Success)
                {
                    mirrors[i].LastResponseTime = TryIPPing(mirrors[i].Url);
                    continue;
                }
                mirrors[i].LastResponseTime = reply.RoundtripTime;
            }
        }

        public static long TryIPPing(string Url)
        {
            IPHostEntry host = Dns.GetHostEntry(Url);
            Ping p = new Ping();

            foreach(IPAddress ip in host.AddressList)
            {
                PingReply rep = p.Send(ip);
                if (rep.Status == IPStatus.Success)
                    return rep.RoundtripTime;
            }

            return 10000;
        }

        public static Mirror GetOptimalMirror(List<Mirror> mirrors)
        {
            long min = mirrors.Min(m => m.LastResponseTime);
            return mirrors.First(m => m.LastResponseTime == min);
        }
    }
}
