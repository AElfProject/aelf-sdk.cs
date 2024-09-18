using Google.Protobuf;

namespace AElf.Client.Dto
{
    public class GenerateTransactionInput
    {
        /// <summary>
        /// from address
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// to address
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// contract method name
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// contract method parameters
        /// </summary>
        public IMessage Params { get; set; }

        public string ClientUrl { get; set; } = string.Empty;
        public int ChainId { get; set; }
    }
}