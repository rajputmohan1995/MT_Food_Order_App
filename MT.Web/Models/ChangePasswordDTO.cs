using System.ComponentModel.DataAnnotations;

namespace MT.Web.Models;

public class ChangePasswordDTO
{
    public string UserID { get; set; }

    [Required(ErrorMessage = "Current password is required")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "New password is required")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password must be between 6 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Re-enter new password is required")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "New password and Re-enter new password is not matching")]
    public string ReenterNewPassword { get; set; }
}