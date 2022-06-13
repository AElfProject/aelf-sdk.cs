namespace AElf.Client.Options;

public class AElfClientConfigOptions
{
    public string ClientAlias { get; set; } = "TestNetSidechain";
    public string MainChainClientAlias { get; set; } = "TestNetMainChain";
    public string SidechainClientAlias { get; set; } = "TestNetSidechain";
    public string AccountAlias { get; set; } = "Default";
    public bool CamelCase { get; set; } = false;
}