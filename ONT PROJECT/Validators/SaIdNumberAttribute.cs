using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ONT_PROJECT.Validators
{
    public class SaIdNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string id = value as string;
            if (string.IsNullOrEmpty(id))
                return ValidationResult.Success; // Required is handled separately

            // Must be exactly 13 digits
            if (!Regex.IsMatch(id, @"^\d{13}$"))
                return new ValidationResult(ErrorMessage ?? "ID number must be 13 digits");

            // Validate date part (first 6 digits: YYMMDD)
            string birthDatePart = id.Substring(0, 6);
            DateTime birthDate;

            if (!DateTime.TryParseExact(birthDatePart, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                return new ValidationResult(ErrorMessage ?? "Invalid birth date in ID number");

            // Optional: Luhn check for SA ID
            if (!IsValidSaIdNumber(id))
                return new ValidationResult(ErrorMessage ?? "Invalid South African ID number");

            return ValidationResult.Success;
        }

        private bool IsValidSaIdNumber(string id)
        {
            // Luhn algorithm for validation
            int sum = 0;
            for (int i = 0; i < id.Length; i++)
            {
                int digit = int.Parse(id[i].ToString());
                if (i % 2 == 1)
                {
                    int dbl = digit * 2;
                    sum += dbl > 9 ? dbl - 9 : dbl;
                }
                else
                {
                    sum += digit;
                }
            }
            return sum % 10 == 0;
        }
    }
}
