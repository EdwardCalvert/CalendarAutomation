using System;

namespace GoogleCalendar
{
    public class URI : IEquatable<URI>
    {
        private string _uri;
        public bool validUri { get
            {
                if (!Uri.IsWellFormedUriString(_uri, UriKind.Absolute))
                    return false;
                if (!Uri.TryCreate(_uri, UriKind.Absolute, out Uri tmp))
                    return false;
                return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps; ;
            } }

        private bool _alreadyLaunched;

        public URI(string uri)
        {
            _uri = uri;

        }

        public enum ResultCode
        {
            Sucess,
            URIStarted,
            Win32Exception,
            ObjectDisposedException,
            FileNotFoundException,
        }

        public ResultCode LaunchURI()
        {
            try
            {

                if (!_alreadyLaunched && validUri)
                {
                    System.Diagnostics.Process.Start(_uri);
                    _alreadyLaunched = true;
                    return ResultCode.URIStarted;
                }
                return ResultCode.Sucess;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return ResultCode.Win32Exception;
            }
            catch (ObjectDisposedException)
            {
                return ResultCode.ObjectDisposedException;
            }
            catch (System.IO.FileNotFoundException)
            {
                return ResultCode.FileNotFoundException;
            }
        }

        public void SetUri(string uri)
        {
            _uri = uri;
        }

        public string GetUri()
        {
            return _uri;
        }

        public bool Equals(URI uri)
        {
            if (this._uri == uri._uri)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return  _uri == null ? 0 :23* _uri.GetHashCode();

            
        }
    }
}
