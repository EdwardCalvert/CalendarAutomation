using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Handlers
{
    public static class URLHandler
    {

        public static List<string> UnpackFrameURL(string frame)
        {

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(frame);
            var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
            if (nodes == null)
            {
                return new List<string>();
            }
            else
            {
                return nodes.ToList().ConvertAll(r => r.Attributes.ToList().ConvertAll(i => i.Value)).SelectMany(j => j).ToList();
            }
        }

        public static bool IsValidUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }

        

        public static void OpenUri(string uri)
        {
            if (IsValidUri(uri))
            {
                System.Diagnostics.Process.Start(uri);
            }
            
        }

    }
}
