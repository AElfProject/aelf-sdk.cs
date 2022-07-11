using AElf.Types;
using Shouldly;

namespace AElf.Client.Test;

public class BasicTests
{
    [Theory]
    [InlineData("04c1b4a75fd9ba37e0a84b9916e517e9c591c5b9efacabf1feb1a3d34f38920a250454dc9dce0f811956d210804c904959be96a75a9d9b1410aa25f7d9e1b6f69c")]
    public void PubkeyToAddressTest(string pubkey)
    {
        var address = Address.FromPublicKey(ByteArrayHelper.HexStringToByteArray(pubkey));
        address.ShouldNotBeNull();
    }
}