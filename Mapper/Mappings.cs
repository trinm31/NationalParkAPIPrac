using AutoMapper;
using NationalParkAPI.Models;
using NationalParkAPI.Models.Dtos;

namespace NationalParkAPI.Mapper
{
    public class Mappings: Profile
    {
        public Mappings()
        {
            CreateMap<NationalPark,NationalParkDto>().ReverseMap();
            CreateMap<Trail,TrailDto>().ReverseMap();
            CreateMap<Trail,TrailUpdateDto>().ReverseMap();
            CreateMap<Trail,TrailCreateDto>().ReverseMap();
        }
    }
}