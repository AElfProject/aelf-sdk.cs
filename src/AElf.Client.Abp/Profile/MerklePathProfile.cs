using AElf.Client.Dto;
using AutoMapper;

namespace AElf.Client.Abp;

public class MerklePathProfile : Profile
{
    public MerklePathProfile()
    {
        CreateMap<MerklePath, MerklePathDto>();

        CreateMap<MerklePathNode, MerklePathNodeDto>();
    }
}