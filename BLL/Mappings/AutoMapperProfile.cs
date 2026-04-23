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
            CreateMap<RegisterRequestDto, Account>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.Trim()))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName.Trim()))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress.Trim().ToLowerInvariant()))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
                    Enum.IsDefined(typeof(Account.GenderType), src.Gender)
                        ? (Account.GenderType)src.Gender
                        : Account.GenderType.Male));

            CreateMap<Account, AccountResponseDto>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<Account, ApiResponseDto>()
                .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src));

            // PlayerProfile mappings
            CreateMap<PlayerProfile, PlayerProfileResponseDto>()
                .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class.ToString()))
                .ForMember(dest => dest.ExperienceToNextLevel, opt => opt.MapFrom(src => CalculateExpForLevel(src.Level + 1)))
                .ForMember(dest => dest.MaxEnergy, opt => opt.MapFrom(src => 100));

            // PlayerStat mappings
            CreateMap<PlayerStat, PlayerStatsResponseDto>()
                .ForMember(dest => dest.StatsId, opt => opt.MapFrom(src => src.Id));

            // Item mappings
            CreateMap<Item, ItemResponseDto>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Rarity, opt => opt.MapFrom(src => src.Rarity.ToString()))
                .ForMember(dest => dest.Slot, opt => opt.MapFrom(src => src.Slot.ToString()));

            CreateMap<Item, ItemDetailResponseDto>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Rarity, opt => opt.MapFrom(src => src.Rarity.ToString()))
                .ForMember(dest => dest.Slot, opt => opt.MapFrom(src => src.Slot.ToString()));

            // EquipmentStats mappings
            CreateMap<EquipmentStats, EquipmentStatsDto>();

            // Skill mappings
            CreateMap<Skill, SkillResponseDto>()
                .ForMember(dest => dest.SkillId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.DamageType, opt => opt.MapFrom(src => src.DamageType.ToString()))
                .ForMember(dest => dest.TargetType, opt => opt.MapFrom(src => src.TargetType.ToString()))
                .ForMember(dest => dest.ClassRequirement, opt => opt.MapFrom(src => src.ClassRequirement.ToString()));
        }

        private static int CalculateExpForLevel(int level)
        {
            return 100 * level * level;
        }
    }
}
