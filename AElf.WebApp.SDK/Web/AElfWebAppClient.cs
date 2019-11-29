using AElf.WebApp.SDK.Web.Service;

namespace AElf.WebApp.SDK.Web
{
    public class AElfWebAppClient
    {
        public static AElfWebService GetClientByUrl(string baseUrl, int retryTimes = 3, int timeout = 60)
        {
            var httpService = new HttpService(timeout, retryTimes);
            return new AElfWebService(httpService, baseUrl);
        }
    }
}