using System.Collections.Generic;

namespace AElf.Net.SDK.Infrastructure.Dto
{
    public class MerklePathDto
    {
        public List<MerklePathNodeDto> MerklePathNodes;
    }

    public class MerklePathNodeDto
    {
        public string Hash { get; set; }
        public bool IsLeftChildNode { get; set; }
    }
}