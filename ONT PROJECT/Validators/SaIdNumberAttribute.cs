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
                return ValidationResult.Success; 

            if (!Regex.IsMatch(id, @"^\d{13}$"))
                return new ValidationResult(ErrorMessage ?? "ID number must be 13 digits");

            string birthDatePart = id.Substring(0, 6);
            DateTime birthDate;

            if (!DateTime.TryParseExact(birthDatePart, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                return new ValidationResult(ErrorMessage ?? "Invalid birth date in ID number");

            birthDate = AdjustCentury(birthDate);

            if (birthDate > DateTime.Today)
                return new ValidationResult(ErrorMessage ?? "Birth date cannot be in the future");

            if (!IsValidSaIdNumber(id))
                return new ValidationResult(ErrorMessage ?? "Invalid South African ID number");

            return ValidationResult.Success;
        }

        private DateTime AdjustCentury(DateTime date)
        {
            int year = date.Year;
            int currentYear = DateTime.Today.Year;

            if (year > currentYear)
            {
                year -= 100;
            }

            return new DateTime(year, date.Month, date.Day);
        }

        private bool IsValidSaIdNumber(string id)
        {
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
