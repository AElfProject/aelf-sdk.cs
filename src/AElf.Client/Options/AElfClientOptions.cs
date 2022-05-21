namespace AElf.Client.Options;

public class AElfClientOptions
{
    public Dictionary<string, ClientConfig> ClientConfigs { get; set; } = new();
}

public class ClientConfig
{
    public string Endpoint { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int Timeout { get; set; } = 60;
}