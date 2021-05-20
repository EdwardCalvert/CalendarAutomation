using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GoogleCalendar
{

    public static class URIParser 
    { 
        /// <summary>
        /// Matches could be null- double check
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<URI> ReadText(string text)
        {
            List <URI> uRIs= new List<URI>();
            //https://regexr.com/2vtcc
            //https://regexr.com/5tbqr
            Regex urlRx = new Regex(@"(file:///|(http|ftp|https)://)([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", RegexOptions.IgnoreCase);
            MatchCollection UrlMatches = urlRx.Matches(text);

            //What about blank matches & the same matches??

            if (UrlMatches != null && UrlMatches.Count > 0)
            {
                foreach (Match match in UrlMatches)
                {
                    if (match != null)
                    {
                        uRIs.Add(new URI(match.Value));
                    }

                }
            }
 

            
            return uRIs.Distinct().ToList();
        }

    }
}
