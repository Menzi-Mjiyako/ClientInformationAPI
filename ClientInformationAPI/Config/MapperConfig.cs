using AutoMapper;
using ClientInformation.Domain.Dtos;
using ClientInformation.Domain.Models;

namespace ClientInformationAPI.Config
{
    public class MapperConfig : Profile
    {
        public MapperConfig() 
        {
            CreateMap<ClientInfo, ClientInformationDto>().ReverseMap();
            CreateMap<Address, AddressDto>().ReverseMap();
            CreateMap<ContactInformation, ContactInformationDto>().ReverseMap();
            CreateMap<ClientInfo, ClientInfoDto>().ReverseMap();
        }
    }
}
