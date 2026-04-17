using BLL.DTOs;
using System;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<ApiResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<ApiResponseDto> ChangePasswordAsync(Guid accountId, ChangePasswordRequestDto request);
    }
}