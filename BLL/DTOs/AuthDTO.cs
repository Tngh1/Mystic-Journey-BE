using BLL.Validations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BLL.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email or username is required.")]
        [StringLength(255, ErrorMessage = "Email or username is too long.")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        public string ClientType { get; set; } = "Web";

        public string? ClientVersion { get; set; }
    }

    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [UserName]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [StringLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [Password]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare(nameof(Password), ErrorMessage = "Confirm password does not match password.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [StringLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [StringLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Verification code is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must be 6 digits.")]
        public string VerificationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [Password]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Confirm password does not match new password.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [Password]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Confirm password does not match new password.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public int AccountId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public int RoleId { get; set; } = 1;
        public string Role { get; set; } = "Player";
        public bool HasCharacter { get; set; }
        public int? PlayerProfileId { get; set; }
        public string? PlayerDisplayName { get; set; }
        public string PlayerClass { get; set; } = "Knight";
        public int Level { get; set; } = 1;
        public string LastMapName { get; set; } = string.Empty;
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? AccessTokenExpiresAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
    }

    public class MeResponseDto
    {
        public int AccountId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? PlayerProfileId { get; set; }
        public string PlayerClass { get; set; } = "Knight";
        public int Level { get; set; } = 1;
        public string LastMapName { get; set; } = string.Empty;
        public double PositionX { get; set; }
        public double PositionY { get; set; }
    }

    public class SendVerificationCodeRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [StringLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyEmailRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [StringLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Verification code is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must be 6 digits.")]
        public string VerificationCode { get; set; } = string.Empty;
    }

    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
