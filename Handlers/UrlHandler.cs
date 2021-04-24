using System;

namespace Handlers
{
    public static class URLHandler
    {



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
