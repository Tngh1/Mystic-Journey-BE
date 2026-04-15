using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequestDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Confirm password does not match password.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public int Gender { get; set; } = 0;
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
    }

    public class ForgotPasswordRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequestDto
    {
        [Required]
        public string PasswordResetToken { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Confirm password does not match new password.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public Guid? AccountId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Role { get; set; }

        public string? AccessToken { get; set; }
        public DateTime? AccessTokenExpiresAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
    }
}