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
    public class SouthAfricanIDAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var id = value as string;

            if (string.IsNullOrEmpty(id))
                return new ValidationResult("ID Number is required.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^\d{13}$"))
                return new ValidationResult("ID Number must be 13 digits.");

            // Extract first 6 digits as YYMMDD
            var dobPart = id.Substring(0, 6);

            // Parse YYMMDD to a valid date
            if (!DateTime.TryParseExact(dobPart, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dob))
            {
                return new ValidationResult("The first 6 digits must represent a valid date (YYMMDD).");
            }

            // Optional: Check if DOB is not in the future
            if (dob > DateTime.Now)
            {
                return new ValidationResult("Date of birth cannot be in the future.");
            }

            return ValidationResult.Success;
        }
    }
}
