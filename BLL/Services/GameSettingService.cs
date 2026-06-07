using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BLL.Services
{
    public class GameSettingService : IGameSettingService
    {
        private readonly IGameSettingRepository _repository;
        private readonly IMapper _mapper;

        public GameSettingService(IGameSettingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GameSettingResponseDto?> GetSettingById(int id)
        {
            var setting = await _repository.GetGameSettingById(id);
            if (setting == null)
                return null;

            return MapToResponseDto(setting);
        }

        public async Task<GameSettingResponseDto?> GetSettingByKey(string key)
        {
            var setting = await _repository.GetByName(key);
            if (setting == null)
                return null;

            return MapToResponseDto(setting);
        }

        public async Task<GameSettingResponseDto> UpdateSetting(string key, UpdateGameSettingRequestDto request, Guid? updatedByAccountId = null)
        {
            var setting = await _repository.GetByName(key)
                ?? throw new KeyNotFoundException($"Game setting with key '{key}' not found.");

            if (request.Value != null)
                setting.Value = request.Value;

            if (request.Description != null)
                setting.Description = request.Description;

            setting.IsActive = request.IsActive;
            setting.UpdatedAt = DateTime.UtcNow;
            setting.UpdatedByAccountId = updatedByAccountId;

            var updated = await _repository.UpdateGameSetting(setting);
            return MapToResponseDto(updated);
        }

        public async Task<PagedResultDto<GameSettingResponseDto>> GetSettingsPaged(int page, int pageSize, string? search)
        {
            var (totalCount, items) = await _repository.GetSettingsPaged(page, pageSize, search);

            var dtos = items.Select(MapToResponseDto).ToList();
            return new PagedResultDto<GameSettingResponseDto>(totalCount, dtos);
        }

        private static GameSettingResponseDto MapToResponseDto(GameSetting setting)
        {
            return new GameSettingResponseDto
            {
                Id = setting.GameSettingId,
                Key = setting.Name,
                Value = setting.Value,
                Description = setting.Description,
                IsActive = setting.IsActive,
                CreatedAt = setting.CreatedAt,
                UpdatedAt = setting.UpdatedAt,
                UpdatedBy = setting.UpdatedByAccount?.UserName
            };
        }
    }
}
