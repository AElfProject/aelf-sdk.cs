using AElf.Client.Dto;
using AutoMapper;
using Google.Protobuf;
using Volo.Abp.AutoMapper;

namespace AElf.Client.Abp;

public class TransactionProfile : Profile
{
    public const string ErrorTrace = "WithMetrics";

    public TransactionProfile()
    {
        CreateMap<Transaction, TransactionDto>();
        CreateMap<TransactionDto, Transaction>();

        CreateMap<TransactionResult, TransactionResultDto>()
            .ForMember(d => d.ReturnValue, opt => opt.MapFrom(s => s.ReturnValue.ToHex(false)))
            .ForMember(d => d.Bloom,
                opt => opt.MapFrom(s =>
                    s.Status == TransactionResultStatus.NotExisted
                        ? null
                        : s.Bloom.Length == 0
                            ? ByteString.CopyFrom(new byte[256]).ToBase64()
                            : s.Bloom.ToBase64()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString().ToUpper()))
            .ForMember(d => d.Error, opt => opt.MapFrom<TransactionErrorResolver>())
            .Ignore(d => d.Transaction)
            .Ignore(d => d.TransactionSize);

        TransactionResultStatus status;
        CreateMap<TransactionResultDto, TransactionResult>()
            .ForMember(d => d.ReturnValue,
                opt => opt.MapFrom(s => ByteString.CopyFrom(ByteArrayHelper.HexStringToByteArray(s.ReturnValue))))
            .ForMember(d => d.Bloom, opt => opt.MapFrom(s =>
                s.Status.ToUpper() == TransactionResultStatus.NotExisted.ToString().ToUpper()
                    ? null
                    : string.IsNullOrEmpty(s.Bloom)
                        ? ByteString.Empty
                        : ByteString.FromBase64(s.Bloom)))
            .ForMember(d => d.Status,
                opt => opt.MapFrom(s =>
                    Enum.TryParse<TransactionResultStatus>(s.Status, out status)
                        ? status
                        : TransactionResultStatus.NotExisted));

        CreateMap<LogEvent, LogEventDto>();
    }
}

public class TransactionErrorResolver : IValueResolver<TransactionResult, TransactionResultDto, string>
{
    public string Resolve(TransactionResult source, TransactionResultDto destination, string destMember,
        ResolutionContext context)
    {
        var errorTraceNeeded = (bool)context.Items[TransactionProfile.ErrorTrace];
        return TakeErrorMessage(source.Error, errorTraceNeeded);
    }

    public static string TakeErrorMessage(string transactionResultError, bool errorTraceNeeded)
    {
        if (string.IsNullOrWhiteSpace(transactionResultError))
            return null;

        if (errorTraceNeeded)
            return transactionResultError;

        using var stringReader = new StringReader(transactionResultError);
        return stringReader.ReadLine();
    }
}