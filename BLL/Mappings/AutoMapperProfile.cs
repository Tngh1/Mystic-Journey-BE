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
            CreateMap<Account, AccountResponseDto>();
            CreateMap<Account, ApiResponseDto>();
        }
    }
}
