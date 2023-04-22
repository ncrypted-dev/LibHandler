using LibHandler.Models;
using LibHandler.Util;
using System.Text.Json;

namespace LibHandler
{
    public static class LibraryHandler
    {
        private static readonly BaseApiHandler handler = new BaseApiHandler();

        /// <summary>
        /// Returns the IDs of the books searched for.
        /// </summary>
        /// <param name="request">Request string. Can be constructed with GetRequestFormat()</param>
        /// <returns></returns>
        public static List<string> GetIDs(string request)
            => handler.GetIDs(request).Result;

        /// <summary>
        /// Returns JSON data of the books with the provided IDs.
        /// </summary>
        /// <param name="ids"> IDs can be provided manually or obtained with GetIDs() </param>
        /// <returns></returns>
        public static string GetJSONData(List<string> ids)
            => handler.GetJSONData(ids).Result;

        /// <summary>
        /// Returns the download url for a given md5 hash.
        /// </summary>
        /// <param name="md5">MD5 hash of a book</param>
        /// <returns></returns>
        public static string GetDownloadLinkByMD5(string md5)
           => handler.GetDownloadLink(md5).Result;

        /// <summary>
        /// Returns the download url for a book.
        /// </summary>
        /// <param name="book">Book that should be downloaded</param>
        /// <returns></returns>
        public static string GetDownloadLink(Book book)
           => handler.GetDownloadLink(book.md5).Result;

        /// <summary>
        /// Returns a list of searched books.
        /// </summary>
        /// <param name="request">Request string. Can be constructed with GetRequestFormat()</param>
        /// <returns></returns>
        public static List<Book> GetBooks(string request)
        {
            List<string> ids = GetIDs(request);
            string jsonData = GetJSONData(ids);
            return JsonSerializer.Deserialize<List<Book>>(jsonData) ?? new List<Book>();
        }
        /// <summary>
        /// Returns the current mirror.
        /// </summary>
        /// <returns></returns>
        public static Mirror GetCurrentMirror()
            => MirrorHandler.MainMirror;

        /// <summary>
        /// Sets the current mirror.
        /// </summary>
        /// <param name="mirror"></param>
        public static void SetCurrentMirror(Mirror mirror)
            => MirrorHandler.MainMirror = mirror;

        /// <summary>
        /// Generates the search request url
        /// </summary>
        /// <param name="field">Field that should be filtered for.</param>
        /// <param name="value">Value that should be filtered for.</param>
        /// <param name="results">Amount of results (25, 50, 100)</param>
        /// <returns></returns>
        public static string GetRequestFormat(SearchField field, string value, int results) =>
            $"https://libgen.rs/search.php?req={value}&res={results}&column={field.ToString().ToLower()}";

    }

    public enum SearchField
    {
        Title,
        Author,
        Series,
        Publisher,
        Year,
        ISBN,
        Language,
        MD5,
        Tags,
        Extension
    }
}
