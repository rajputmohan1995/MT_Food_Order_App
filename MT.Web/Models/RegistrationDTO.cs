using System.ComponentModel.DataAnnotations;

namespace MT.Web.Models;

public class RegistrationDTO
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Name allows characters between 2 to 200")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    [Phone(ErrorMessage = "Invalid Phone No.")]
    public string? PhoneNumber { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 6 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare(nameof(Password), ErrorMessage = "Password and Confirm Password did not match")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    public string? RoleName { get; set; }
}