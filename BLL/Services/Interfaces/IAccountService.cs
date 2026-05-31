using BLL.DTOs;
using System;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponseDto> LoginAsync(LoginRequestDto request);
        Task<ApiResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<ApiResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<ApiResponseDto> ChangePasswordAsync(Guid accountId, ChangePasswordRequestDto request);
        Task<ApiResponseDto> UpdateProfileAsync(Guid accountId, UpdateProfileRequestDto request);
    }
}
