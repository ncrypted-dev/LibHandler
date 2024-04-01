using HtmlAgilityPack;
using System.Net.Http.Headers;
using LibHandler.Models;

namespace LibHandler.Util
{
    internal class BaseApiHandler
    {
        HttpClient httpClient = new HttpClient();
        HttpClient httpDownload = new HttpClient();
        
        public BaseApiHandler()
        {
            ReloadUrls();
        }

        public async Task<List<string>> GetIDs(string request)
        {
            ReloadUrls();

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
            ReloadUrls();

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
            Mirror m = MirrorHandler.MainDownloadMirror;

            ReloadUrls();

            HttpResponseMessage response = await httpDownload.GetAsync(m.Path + md5);
            
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(res);

                if (m.Url.Equals("library.lol"))
                {
                    HtmlNode download = html.DocumentNode.SelectSingleNode($"//div[@id='download']");
                    return download.SelectSingleNode("h2/a").Attributes["href"].Value;
                }
                else
                {
                    HtmlNode download = html.DocumentNode.SelectSingleNode($"//td[@bgcolor='#A9F5BC']/a");
                    return m.FullUrl + "/" + download.Attributes["href"].Value;
                }
            }
            else
            {
                return $"Error Getting Downloadlink.({response.Content})";
            }   
        }

        public void ReloadUrls()
        {

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(MirrorHandler.MainSearchMirror.FullUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

            httpDownload = new HttpClient();
            httpDownload.BaseAddress = new Uri(MirrorHandler.MainDownloadMirror.FullUrl);
            httpDownload.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        }

    }

}
