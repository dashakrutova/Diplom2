using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.ViewModels.Users;

public class EditUserFormModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string LastName { get; set; }

    [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters.")]
    public string? MiddleName { get; set; }

    public string Login { get; set; }

    //[Required(ErrorMessage = "Password is required.")]
    //[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    [OptionalMinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    //[RegularExpression(@"^\+?[1-9][0-9]{7,14}$", ErrorMessage = "Invalid phone number.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string Number { get; set; }

    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(CreateUserFormModel), nameof(ValidateDateOfBirth))]
    public DateTime DateOfBirth { get; set; }


    public static ValidationResult? ValidateDateOfBirth(DateTime date, ValidationContext context)
    {
        if (date > DateTime.Now.AddYears(-18)) // Ensures user is at least 18 years old
        {
            return new ValidationResult("User must be at least 18 years old.");
        }
        return ValidationResult.Success;
    }
}

public class OptionalMinLengthAttribute : ValidationAttribute
{
    private readonly int _minLength;

    public OptionalMinLengthAttribute(int minLength)
    {
        _minLength = minLength;
        ErrorMessage = $"Password must be at least {_minLength} characters long or be empty.";
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var str = value as string;

        if (string.IsNullOrEmpty(str))
            return ValidationResult.Success;

        if (str.Length >= _minLength)
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage);
    }
}
