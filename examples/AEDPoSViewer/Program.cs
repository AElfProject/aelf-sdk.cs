using AElf;
using AElf.Client;
using AElf.Client.Dto;
using AElf.Contracts.Consensus.AEDPoS;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

var aelfClient = new AElfClientBuilder().UsePublicEndpoint(EndpointType.MainNetMainChain).Build();
var tx = new TransactionBuilder(aelfClient)
    .UsePrivateKey(AElfClientConstants.DefaultPrivateKey)
    .UseSystemContract("AElf.ContractNames.Consensus")
    .UseMethod("GetCurrentRoundInformation")
    .UseParameter(new Empty())
    .Build();

var result = await aelfClient.ExecuteTransactionAsync(new ExecuteTransactionDto
{
    RawTransaction = tx.ToByteArray().ToHex()
});

var round = new Round();
round.MergeFrom(ByteArrayHelper.HexStringToByteArray(result));
Console.WriteLine(round);