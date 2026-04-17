using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.Validations
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
            ErrorMessage = $"You must be at least {minimumAge} years old and birthday cannot be in the future.";
        }

        public MinimumAgeAttribute(int minimumAge, string errorMessage)
        {
            _minimumAge = minimumAge;
            ErrorMessage = errorMessage;
        }

        public override bool IsValid(object? value)
        {
            if (value is not DateTime birthday)
            {
                return value is null;
            }

            var birthDate = birthday.Date;
            var today = DateTime.UtcNow.Date;
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
