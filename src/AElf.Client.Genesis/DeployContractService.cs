using AElf.Client.Consensus.AEDPoS;
using AElf.Client.Core.Infrastructure;
using AElf.Client.Parliament;
using AElf.Standards.ACS0;
using AElf.Standards.ACS3;
using AElf.Types;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AElf.Client.Genesis;

public class DeployContractService : IDeployContractService, ITransientDependency
{
    private readonly IGenesisService _genesisService;
    private readonly IParliamentService _parliamentService;
    private readonly IConsensusService _consensusService;

    private readonly IKeyDirectoryProvider _keyDirectoryProvider;
    private readonly AElfContractOption _contractOption;
    public ILogger<DeployContractService> Logger { get; set; }

    public DeployContractService(
        IGenesisService genesisService, 
        IConsensusService consensusService,
        IParliamentService parliamentService,
        IKeyDirectoryProvider keyDirectoryProvider,
        IOptionsSnapshot<AElfContractOption> contractOption)
    {
        _parliamentService = parliamentService;
        _genesisService = genesisService;
        _consensusService = consensusService;
        _keyDirectoryProvider = keyDirectoryProvider;
        _contractOption = contractOption.Value;

        Logger = NullLogger<DeployContractService>.Instance;
    }

    public async Task<Tuple<Address?, string>> DeployContract(string contractFileName)
    {
        Logger.LogInformation($"Deploy contract: {contractFileName}");
        var input = await ContractDeploymentInput(contractFileName);
        var proposalNewContact = await _genesisService.ProposeNewContract(input);
        Logger.LogInformation("ProposalNewContact: {Result}", proposalNewContact.TransactionResult);
        if (proposalNewContact.TransactionResult.Status != TransactionResultStatus.Mined) 
            return new Tuple<Address?, string>(null, proposalNewContact.TransactionResult.Error);

        var proposalNewLogs = proposalNewContact.TransactionResult.Logs;
        var proposalId = ProposalCreated.Parser
            .ParseFrom(proposalNewLogs.First(l => l.Name.Contains(nameof(ProposalCreated))).NonIndexed)
            .ProposalId;
        var proposalHash = ContractProposed.Parser
            .ParseFrom(proposalNewLogs.First(l => l.Name.Contains(nameof(ContractProposed))).NonIndexed)
            .ProposedContractInputHash;

        var toBeRelease = await ApproveThroughMiners(proposalId);
        if (!toBeRelease)
            return new Tuple<Address?, string>(null, $"Proposal {proposalId} not ready for release");

        var releaseApprovedInput = new ReleaseContractInput
        {
            ProposalId = proposalId,
            ProposedContractInputHash = proposalHash
        };
        var releaseApprovedContract = await _genesisService.ReleaseApprovedContract(releaseApprovedInput);
        Logger.LogInformation("ReleaseApprovedContract: {Result}", releaseApprovedContract.TransactionResult);
        if (releaseApprovedContract.TransactionResult.Status != TransactionResultStatus.Mined)
            return new Tuple<Address?, string>(null, releaseApprovedContract.TransactionResult.Error);
        var releaseApprovedLogs = releaseApprovedContract.TransactionResult.Logs;
        var deployProposalId = ProposalCreated.Parser
            .ParseFrom(releaseApprovedLogs.First(l => l.Name.Contains(nameof(ProposalCreated))).NonIndexed)
            .ProposalId;

        var releaseCodeCheckedInput = new ReleaseContractInput
        {
            ProposalId = deployProposalId,
            ProposedContractInputHash = proposalHash
        };

        if (await CheckProposal(deployProposalId))
        {
            var releaseCodeCheckedResult = await _genesisService.ReleaseCodeCheckedContract(releaseCodeCheckedInput);
            Logger.LogInformation("ReleaseCodeCheckedResult: {Result}", releaseCodeCheckedResult.TransactionResult);
            if (releaseCodeCheckedResult.TransactionResult.Status != TransactionResultStatus.Mined)
                return new Tuple<Address?, string>(null, releaseCodeCheckedResult.TransactionResult.Error);

            var deployAddress = ContractDeployed.Parser.ParseFrom(releaseCodeCheckedResult.TransactionResult.Logs
                .First(l => l.Name.Contains(nameof(ContractDeployed))).NonIndexed).Address;
            return new Tuple<Address?, string>(deployAddress, $"Contract deploy passed authority, contract address: {deployAddress}");;
        }
        return new Tuple<Address?, string>(null, "Contract code didn't pass the code check");
    }

    private async Task<ContractDeploymentInput> ContractDeploymentInput(string name)
    {
        var contractPath = GetFileFullPath(name, _contractOption.ContractDirectory);
        var code = await File.ReadAllBytesAsync(contractPath);
        var checkCode = await CheckCode(code);

        if (checkCode)
        {
            var input = new ContractDeploymentInput
            {
                Category = 0,
                Code = ByteString.CopyFrom(code)
            };
            return input;
        }

        Logger.LogError("The code is already deployed.");
        return new ContractDeploymentInput();
    }

    private async Task<bool> ApproveThroughMiners(Hash proposalId)
    {
        var miners = await _consensusService.GetCurrentMinerList();
        foreach (var minersPubkey in miners.Pubkeys)
        {
            var miner = Address.FromPublicKey(minersPubkey.ToByteArray());
            var approveResult = await _parliamentService.ApproveAsync(proposalId, null, miner.ToBase58());
            Logger.LogInformation("Approve: {Result}", approveResult.TransactionResult);
        }
        var toBeRelease = (await _parliamentService.CheckProposal(proposalId)).ToBeReleased;
        return toBeRelease;
    }

    private async Task<bool> CheckProposal(Hash proposalId)
    {
        var toBeReleased = false;
        var sleepTimes = 0;
        while (!toBeReleased && sleepTimes < 20)
        {
            await Task.Delay(2000);
            toBeReleased = (await _parliamentService.CheckProposal(proposalId)).ToBeReleased;
            Console.Write("\rCheck proposal status");
            sleepTimes++;
        }

        return toBeReleased;
    }

    private string GetFileFullPath(string contractName, string contractFileDirectory)
    {
        var dirPath = GetDirectoryPath(contractFileDirectory);
        var filePath = Path.Combine(dirPath, $"{contractName}.dll");
        var filePathWithExtension = File.Exists(filePath)
            ? filePath
            : Path.Combine(dirPath, $"{contractName}.dll.patched");
        return filePathWithExtension;
    }

    private string GetDirectoryPath(string? configuredKeyDirectory)
    {
        return string.IsNullOrWhiteSpace(configuredKeyDirectory)
            ? Path.Combine(_keyDirectoryProvider.GetAppDataPath(), "contracts")
            : configuredKeyDirectory;
    }

    private async Task<bool> CheckCode(byte[] code)
    {
        var hash = HashHelper.ComputeFrom(code);
        var registration = await _genesisService.GetSmartContractRegistrationByCodeHash(hash);
        return registration.Equals(new SmartContractRegistration());
    }
} 