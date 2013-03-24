using System.IO;
using System.Net;
using System.Text;

namespace Web_Scrape
{
    public class Scraper
    {
        private readonly string _userAgent;
        private CookieCollection _cookies;

        public Scraper(string userAgent)
        {
            _userAgent = userAgent;
            _cookies = new CookieCollection();
        }

        public HttpWebResponse HttpGet(string url)
        {
            return Request(url, string.Empty, string.Empty, HttpMethod.GET);
        }

        public HttpWebResponse HttpPost(string url, string postData)
        {
            return Request(url, string.Empty, postData, HttpMethod.POST);
        }

        private HttpWebResponse Request(string url, string referer, string postData, HttpMethod method)
        {
            var http = (HttpWebRequest) WebRequest.Create(url);
            http.AllowAutoRedirect = true;
            http.Method = method.ToString();
            http.UserAgent = _userAgent;
            http.CookieContainer = new CookieContainer();
            http.CookieContainer.Add(_cookies);
            http.Referer = referer;
            http.AllowAutoRedirect = false;
            switch (method)
            {
                case HttpMethod.POST:
                    {
                        http.ContentType = "application/x-www-form-urlencoded";
                        var dataBytes = Encoding.UTF8.GetBytes(postData);
                        http.ContentLength = dataBytes.Length;
                        using (var postStream = http.GetRequestStream())
                        {
                            postStream.Write(dataBytes, 0, dataBytes.Length);
                        }
                    }
                    break;
            }
            var httpResponse = (HttpWebResponse) http.GetResponse();

            if (httpResponse.Cookies.Count > 0)
                _cookies = httpResponse.Cookies;

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.Redirect:
                    return Request(httpResponse.Headers["Location"], url, string.Empty, HttpMethod.GET);
            }
            return httpResponse;
        }

        public void DownloadFile(string url, string downloadPath, string filename)
        {
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.AllowAutoRedirect = true;
            http.Method = "GET";
            http.UserAgent = _userAgent;
            http.CookieContainer = new CookieContainer();
            http.CookieContainer.Add(_cookies);
            http.Referer = string.Empty;
            http.AllowAutoRedirect = false;
            var response = http.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                var buffer = new byte[32 * 1024]; //32Kb chunks
                using (var filestream = File.Create(Path.Combine(downloadPath, filename)))
                {
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        filestream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }

    public enum HttpMethod
    {
        GET,
        POST
    }
}
