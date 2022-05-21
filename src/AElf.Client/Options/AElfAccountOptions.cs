namespace AElf.Client.Options;

public class AElfAccountOptions
{
    public string KeyDirectory { get; set; }
    public Dictionary<string, AccountConfig> AccountConfigs { get; set; }
}

public class AccountConfig
{
    public string Password { get; set; }
    public string PrivateKey { get; set; }
}