using System.ComponentModel.DataAnnotations;

namespace App.Attributes
{
	internal class IsIntAttribute : ValidationAttribute
	{
		private readonly int? min;
		private readonly int? max;

		public IsIntAttribute() { }

		public IsIntAttribute(int min)
		{
			this.min = min;
		}

		public IsIntAttribute(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if (value == null) return ValidationResult.Success;

			bool success = int.TryParse(value.ToString(), out int number);
			if (!success)
				return new ValidationResult(string.Format("Invalid number: '{0}'.", value.ToString()));

			if (min != null && number < min.Value)
				return new ValidationResult(string.Format("Number must be greater than {0}", min.Value));

			if (max != null && number > max.Value)
				return new ValidationResult(string.Format("Number must be lower than {0}", max.Value));

			return ValidationResult.Success;
		}
	}
}
