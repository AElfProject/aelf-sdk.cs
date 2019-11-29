using AElf.WebApp.SDK.Web.Service;

namespace AElf.WebApp.SDK.Web
{
    public class AElfWebAppClient
    {
        public static IWebAppService GetClientByUrl(string url, int retryTimes = 3, int timeout = 60)
        {
            var httpService = new HttpService(timeout, retryTimes);
            
            var blockService = new BlockService(httpService, url);
            var chainService = new ChainService(httpService, url);
            var transactionService = new TransactionService(httpService, url);
            var netService = new NetService(httpService, url);

            return new WebAppService(chainService, netService, blockService, transactionService);
        }
    }
}