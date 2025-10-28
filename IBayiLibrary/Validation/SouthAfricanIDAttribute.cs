using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace IBayiLibrary.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SouthAfricanIDAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var idNumber = value as string;

            if (string.IsNullOrEmpty(idNumber))
                return ValidationResult.Success; // Required attribute will handle empty

            if (idNumber.Length < 6)
                return new ValidationResult("ID Number must have at least 6 digits.");

            string dobPart = idNumber.Substring(0, 6);

            string yearPart = dobPart.Substring(0, 2);
            string monthPart = dobPart.Substring(2, 2);
            string dayPart = dobPart.Substring(4, 2);

            if (!int.TryParse(yearPart, out int year) ||
                !int.TryParse(monthPart, out int month) ||
                !int.TryParse(dayPart, out int day))
            {
                return new ValidationResult("ID Number contains invalid digits for date of birth.");
            }

            // Determine century
            int currentYearTwoDigits = DateTime.Now.Year % 100;
            year += year > currentYearTwoDigits ? 1900 : 2000;

            try
            {
                var dob = new DateTime(year, month, day);
                if (dob > DateTime.Now || dob < DateTime.Now.AddYears(-120))
                    return new ValidationResult("ID Number contains an invalid date of birth.");
            }
            catch
            {
                return new ValidationResult("ID Number contains an invalid date of birth.");
            }

            return ValidationResult.Success;
        }
    }
}
