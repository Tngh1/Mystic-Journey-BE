using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BLL.Validations
{
    // Executes validation attribute operation.
    public class PasswordAttribute : ValidationAttribute
    {
        private const int MinPasswordLength = 6;
        private const int MaxPasswordLength = 100;
        private const string PasswordPattern = "^(?=.*[A-Za-z])(?=.*\\d).+$";

        // Initializes a new default instance of the PasswordAttribute class.
        public PasswordAttribute()
        {
            ErrorMessage = "Password is invalid.";
        }

        // Initializes a new instance of PasswordAttribute with dependencies: errorMessage.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PasswordAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        // Executes is valid operation.
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

        // Executes is valid operation.
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string password)
            {
                return ValidationResult.Success;
            }

            if (string.IsNullOrWhiteSpace(password))  // Mandatory string argument is blank — fail fast
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
