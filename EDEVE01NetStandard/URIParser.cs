using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace EDDEV101.HelperMethods.V2
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
            Regex urlRx = new(@"(((http|ftp|https)://([\w+?\.\w+]))|www\.)+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", RegexOptions.IgnoreCase);
            MatchCollection UrlMatches = urlRx.Matches(text);

            foreach (Match match in UrlMatches)
            {
                uRIs.Add(new URI(match.Value));
            }
            return uRIs;       
        }

    }
}
