using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.Validations
{
    // Executes validation attribute operation.
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        // Initializes a new instance of MinimumAgeAttribute with dependencies: minimumAge.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
            ErrorMessage = $"You must be at least {minimumAge} years old and birthday cannot be in the future.";
        }

        // Initializes a new instance of MinimumAgeAttribute with dependencies: minimumAge, errorMessage.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public MinimumAgeAttribute(int minimumAge, string errorMessage)
        {
            _minimumAge = minimumAge;
            ErrorMessage = errorMessage;
        }

        // Executes is valid operation.
        public override bool IsValid(object? value)
        {
            if (value is null)
            {
                return true;
            }

            DateOnly birthDate;

            if (value is DateOnly dateOnly)
            {
                birthDate = dateOnly;
            }
            else if (value is DateTime dateTime)
            {
                birthDate = DateOnly.FromDateTime(dateTime.Date);
            }
            else
            {
                return false;
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            if (birthDate > today)
            {
                return false;
            }

            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            return age >= _minimumAge;
        }

        // Executes is valid operation.
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (!IsValid(value))
            {
                var memberNames = validationContext.MemberName is null
                    ? null
                    : new[] { validationContext.MemberName };

                return new ValidationResult(ErrorMessageString, memberNames);
            }

            return ValidationResult.Success;
        }
    }
}
