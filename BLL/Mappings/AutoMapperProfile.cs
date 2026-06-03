using AutoMapper;
using BLL.DTOs;
using DAL.Models;
using System;

namespace BLL.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Account mappings
            CreateMap<RegisterRequestDto, Account>();
            CreateMap<Account, AccountResponseDto>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId));
            CreateMap<Account, ApiResponseDto>();
        }
    }
}
