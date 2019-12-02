using System;

namespace AElf.WebApp.SDK.Common
{
    public enum NameProvider
    {
        Genesis,
        Election,
        Profit,
        Vote,
        Treasury,
        Token,
        TokenConverter,
        Consensus,
        ParliamentAuth,
        CrossChain,
        AssociationAuth,
        Configuration,
        ReferendumAuth,
    }

    public static class NameProviderExtension
    {
        public static NameProvider ConvertNameProvider(this string name)
        {
            return (NameProvider) Enum.Parse(typeof(NameProvider), name, true);
        }
    }
}