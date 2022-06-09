namespace AElf.Client.Options;

public class AElfClientConfigOptions
{
    public string UseClientAlias { get; set; } = "TestNetSidechain";
    public string UseMainChainClientAlias { get; set; } = "TestNetMainChain";
    public string UseSidechainClientAlias { get; set; } = "TestNetSidechain";
    public string UseAccountAlias { get; set; } = "Default";
    public bool UseCamelCase { get; set; } = false;
}