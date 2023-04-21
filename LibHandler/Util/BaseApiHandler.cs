using HtmlAgilityPack;
using System.Net.Http.Headers;

namespace LibHandler.Util
{
    internal class BaseApiHandler
    {
        readonly HttpClient httpClient;
        readonly HttpClient httpDownload;
        public BaseApiHandler()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(MirrorHandler.MainMirror.FullUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

            httpDownload = new HttpClient();
            httpDownload.BaseAddress = new Uri(MirrorHandler.MainMirror.DownloadUrl);
            httpDownload.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        }

        public async Task<List<string>> GetIDs(string request)
        {
            HttpResponseMessage response = await httpClient.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(res);

                HtmlNodeCollection tables = html.DocumentNode.SelectNodes("//table");
                HtmlNodeCollection rows = tables[^2].SelectNodes("tr");
                rows.RemoveAt(0);

                List<string> ids = new List<string>();

                foreach (HtmlNode n in rows)
                    ids.Add(n.ChildNodes[0].InnerHtml);

                return ids;
            }
            else
            {
                Console.WriteLine(response.ReasonPhrase);
                return new List<string>();
            }
        }
        public async Task<string> GetJSONData(List<string> ids)
        {
            string idString = string.Join(",", ids);

            HttpResponseMessage response = await httpClient.GetAsync($"json.php?ids={idString}&fields=*");

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                Console.WriteLine(response.ReasonPhrase);
                return string.Empty;
            }
        }
        public async Task<string> GetDownloadLink(string md5)
        {
            HttpResponseMessage response = await httpDownload.GetAsync($"main/{md5}");
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(res);

                HtmlNode download = html.DocumentNode.SelectSingleNode($"//div[@id='download']");

                string mainDownload = download.SelectSingleNode("h2/a").Attributes["href"].Value;
                return mainDownload;
            }
            else
            {
                Console.WriteLine(response.ReasonPhrase);
                return string.Empty;
            }
        }

    }


}
