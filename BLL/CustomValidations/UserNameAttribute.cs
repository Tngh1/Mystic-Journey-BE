using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BLL.Validations
{
    // Executes validation attribute operation.
    public class UserNameAttribute : ValidationAttribute
    {
        private const int MinUserNameLength = 3;
        private const int MaxUserNameLength = 100;
        private const string UserNamePattern = "^[a-zA-Z0-9._-]+$";

        // Initializes a new default instance of the UserNameAttribute class.
        public UserNameAttribute()
        {
            ErrorMessage = "Username is invalid.";
        }

        // Initializes a new instance of UserNameAttribute with dependencies: errorMessage.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public UserNameAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        // Executes is valid operation.
        public override bool IsValid(object? value)
        {
            if (value is not string userName)
            {
                return value is null;
            }

            var trimmed = userName.Trim();
            return trimmed.Length >= MinUserNameLength &&
                   trimmed.Length <= MaxUserNameLength &&
                   Regex.IsMatch(trimmed, UserNamePattern);
        }

        // Executes is valid operation.
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string userName)
            {
                return ValidationResult.Success;
            }

            var trimmed = userName.Trim();

            var memberNames = validationContext.MemberName is null
                ? null
                : new[] { validationContext.MemberName };

            if (string.IsNullOrWhiteSpace(userName))  // Mandatory string argument is blank — fail fast
            {
                return new ValidationResult("Username cannot be empty.", memberNames);
            }

            if (trimmed.Length < MinUserNameLength)
            {
                return new ValidationResult($"Username must be at least {MinUserNameLength} characters.", memberNames);
            }

            if (trimmed.Length > MaxUserNameLength)
            {
                return new ValidationResult($"Username must not exceed {MaxUserNameLength} characters.", memberNames);
            }

            if (!Regex.IsMatch(trimmed, UserNamePattern))
            {
                return new ValidationResult("Username can only contain letters, numbers, dot (.), underscore (_), and hyphen (-).", memberNames);
            }

            return ValidationResult.Success;
        }
    }
}
