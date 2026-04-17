using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BLL.Validations
{
    public class PasswordAttribute : ValidationAttribute
    {
        private const int MinPasswordLength = 6;
        private const int MaxPasswordLength = 100;
        private const string PasswordPattern = "^(?=.*[A-Za-z])(?=.*\\d).+$";

        public PasswordAttribute()
        {
            ErrorMessage = "Password is invalid.";
        }

        public PasswordAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public override bool IsValid(object? value)
        {
            if (value is not string password)
            {
                return value is null;
            }

            return password.Length >= MinPasswordLength &&
                   password.Length <= MaxPasswordLength &&
                   Regex.IsMatch(password, PasswordPattern);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string password)
            {
                return ValidationResult.Success;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return new ValidationResult("Password cannot be empty.");
            }

            if (password.Length < MinPasswordLength)
            {
                return new ValidationResult($"Password must be at least {MinPasswordLength} characters.");
            }

            if (password.Length > MaxPasswordLength)
            {
                return new ValidationResult($"Password must not exceed {MaxPasswordLength} characters.");
            }

            if (!Regex.IsMatch(password, @"[A-Za-z]"))
            {
                return new ValidationResult("Password must contain at least one letter.");
            }

            if (!Regex.IsMatch(password, @"\d"))
            {
                return new ValidationResult("Password must contain at least one number.");
            }

            return ValidationResult.Success;
        }
    }
}