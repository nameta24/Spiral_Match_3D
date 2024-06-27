using System;
using System.Net;

namespace Voodoo.Analytics
{
    class VoodooWebProxy : IWebProxy
    {
        private IWebProxy _wrappedProxy;
        private ICredentials _credentials;
        private string _proxyServer;
        
        private void Init()
        {
            _wrappedProxy = null;
            _credentials = CredentialCache.DefaultCredentials;
        }
        public VoodooWebProxy(string proxyServer)
        {
            _proxyServer = proxyServer;
            Init();
        }

        public VoodooWebProxy(IWebProxy theWrappedProxy)
        {
            Init();
            _wrappedProxy = theWrappedProxy;
        }
        public ICredentials Credentials
        {
            get
            {
                if (_wrappedProxy != null)
                {
                    return _wrappedProxy.Credentials;
                }
                else
                {
                    return _credentials;
                }
            }
            set
            {
                if (_wrappedProxy != null)
                {
                    _wrappedProxy.Credentials = value;
                }
                else
                {
                    _credentials = value;
                }

            }
        }

        public Uri GetProxy(Uri destination)
        {
            if (_wrappedProxy != null /* todo or Uri == certain Uri */)
            {
                return _wrappedProxy.GetProxy(destination);
            }
            else
            {
                // hardcoded proxy here..
                return new Uri($"http://{_proxyServer}:8888");
            }
        }

        public bool IsBypassed(Uri host)
        {
            if (_wrappedProxy != null)
            {
                return _wrappedProxy.IsBypassed(host);
            }
            else
            {
                return false;
            }

        }
    }
}