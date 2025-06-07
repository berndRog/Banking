namespace BankingApi.Core.Dtos;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public class IbanLengthAttribute : ValidationAttribute {
    private readonly int _min;
    private readonly int _max;

    public IbanLengthAttribute(int min, int max) {
        _min = min;
        _max = max;
        ErrorMessage = $"IBAN must be between {_min} and {_max} characters (excluding spaces).";
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext) {
        if (value is not string iban) return ValidationResult.Success!;
        var cleaned = iban.Replace(" ", "");
        if (cleaned.Length < _min || cleaned.Length > _max)
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success!;
    }
}