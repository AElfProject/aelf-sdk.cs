namespace AElf.Client.Dto
{
    public class CreateRawTransactionInput
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
        /// refer block height
        /// </summary>
        public long RefBlockNumber { get; set; }

        /// <summary>
        /// refer block hash
        /// </summary>
        public string RefBlockHash { get; set; }

        /// <summary>
        /// contract method name
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// contract method parameters
        /// </summary>
        public string Params { get; set; }
    }
}