using System.Collections.Generic;

namespace AElf.Client.Dto
{
    
    public class PeerDto
    {
        public string IpAddress { get; set; }
        public int ProtocolVersion { get; set; }
        public long ConnectionTime { get; set; }
        public string ConnectionStatus { get; set; }
        public bool Inbound { get; set; }
        public int BufferedTransactionsCount { get; set; }
        public int BufferedBlocksCount { get; set; }
        public int BufferedAnnouncementsCount { get; set; }
        public List<RequestMetric> RequestMetrics { get; set; }
        public string NodeVersion { get; set; }
    }
    
    public class RequestMetric
    {
        public long RoundTripTime { get; set; }
        public string MethodName { get; set; }
        public string Info { get; set; }
        public string RequestTime { get; set; }
    }
}
