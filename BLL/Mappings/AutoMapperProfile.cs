using AutoMapper;
using BLL.DTOs;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterRequestDto, Account>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.Trim()))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Trim()))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress.Trim().ToLowerInvariant()))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
                    Enum.IsDefined(typeof(Account.GenderType), src.Gender)
                        ? (Account.GenderType)src.Gender
                        : Account.GenderType.Male));

            CreateMap<Account, ApiResponseDto>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}
