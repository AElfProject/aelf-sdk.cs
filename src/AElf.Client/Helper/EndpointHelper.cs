using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace AElf.Client.Helper
{
    public static class EndpointHelper
    {
        public static bool TryParse(string endpointString, out DnsEndPoint? endpoint,
            int defaultPort = 6800)
        {
            endpoint = null;

            if (string.IsNullOrEmpty(endpointString) || endpointString.Trim().Length == 0)
                return false;

            if (defaultPort != -1 && (defaultPort < IPEndPoint.MinPort || defaultPort > IPEndPoint.MaxPort))
                return false;

            var values = endpointString.Split(new[] {':'});
            string host;
            var port = -1;

            if(values.Length <= 2)
            {
                // ipv4 or hostname
                host = values[0];

                if (values.Length == 1)
                {
                    port = defaultPort;
                }
                else
                {
                    var parsedPort = GetPort(values[1]);

                    if (parsedPort == 0)
                        return false;

                    port = parsedPort;
                }
            }
            //ipv6
            //could be [a:b:c]:d
            else if(values.Length > 2 && (values[0].StartsWith("[") && values[^2].EndsWith("]")))
            {
                host = string.Join(":", values.Take(values.Length - 1).ToArray());
                var parsedPort = GetPort(values[^1]);

                if (parsedPort == 0)
                    return false;
            }
            // [a:b:c] or a:b:c
            else
            {
                host = endpointString;
                port = defaultPort;
            }

            if (port == -1)
                return false;

            // we leave semantic analysis of the ip/hostname to lower levels.
            endpoint = new PeerEndpoint(host, port);

            return true;
        }

        private static int GetPort(string p)
        {
            if (!int.TryParse(p, out int port) || port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                return 0;

            return port;
        }
    }

    public class PeerEndpoint : DnsEndPoint
    {
        public PeerEndpoint(string host, int port)
            : base(host, port, AddressFamily.Unspecified)
        {
        }

        public override string ToString()
        {
            return Host + ":" + Port;
        }
    }
}