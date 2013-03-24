using System.IO;
using System.Net;

namespace Web_Scrape
{
    public static class Extensions
    {
        public static string AsString(this HttpWebResponse response)
        {
            if (response == null)
                return string.Empty;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
