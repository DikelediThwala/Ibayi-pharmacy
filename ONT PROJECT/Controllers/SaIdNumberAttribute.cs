using System;
using System.ComponentModel.DataAnnotations;

public class SaIdNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string id = value as string;
        if (string.IsNullOrWhiteSpace(id))
            return new ValidationResult("ID Number is required");

        if (id.Length != 13 || !long.TryParse(id, out _))
            return new ValidationResult("ID Number must be 13 digits");

        // Extract date parts
        int year = int.Parse(id.Substring(0, 2));
        int month = int.Parse(id.Substring(2, 2));
        int day = int.Parse(id.Substring(4, 2));

        // Determine century (basic assumption: 00–current year = 2000s, else 1900s)
        int fullYear = (year > DateTime.Now.Year % 100) ? 1900 + year : 2000 + year;

        // Validate real date
        try
        {
            DateTime dob = new DateTime(fullYear, month, day);
        }
        catch
        {
            return new ValidationResult("Invalid date in ID Number");
        }

        return ValidationResult.Success;
    }
}
