using System.ComponentModel.DataAnnotations;

namespace App.Attributes;

internal class IsSequenceAttribute : ValidationAttribute
{
	public IsSequenceAttribute() { }

	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value == null) return ValidationResult.Success;

		string? str = value.ToString()?.ToLower();

		if (str == null)
			return new ValidationResult("Invalid sequence");

		for (int i = 0; i < str.Length; i++)
		{
			if (str[i] != 'a' && str[i] != 'c' && str[i] != 'g' && str[i] != 't')
				return new ValidationResult(string.Format("Invalid base: {0}", str[i]));
		}

		return ValidationResult.Success;
	}
}
