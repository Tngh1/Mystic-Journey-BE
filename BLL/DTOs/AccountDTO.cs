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
    }

    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(200, ErrorMessage = "Full name must not exceed 200 characters.")]
        public string FullName { get; set; } = string.Empty;

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

        public string Gender { get; set; } = "Other";

        [Phone(ErrorMessage = "Phone number format is invalid.")]
        public string? PhoneNumber { get; set; }

        [MinimumAge(13, ErrorMessage = "You must be at least 13 years old.")]
        public DateOnly? Birthday { get; set; }
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

    public class UpdateProfileRequestDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(200, ErrorMessage = "Full name must not exceed 200 characters.")]
        public string FullName { get; set; } = string.Empty;

        public string Gender { get; set; } = "Other";

        [Phone(ErrorMessage = "Phone number format is invalid.")]
        public string? PhoneNumber { get; set; }

        public DateOnly? Birthday { get; set; }
    }

    public class AccountResponseDto
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Gender { get; set; } = "Other";
        public string? PhoneNumber { get; set; }
        public DateOnly? Birthday { get; set; }
        public int RoleId { get; set; } = 1;
        public string? AccessToken { get; set; }
        public DateTime? AccessTokenExpiresAt { get; set; }
    }

    public class ApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AccountResponseDto? Account {  get; set; }
    }
}