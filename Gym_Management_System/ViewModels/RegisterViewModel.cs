using System.ComponentModel.DataAnnotations;

namespace GymManagement.ViewModels
{
  public class RegisterViewModel
  {
    [Required(ErrorMessage = "Please enter your username.")]
    [StringLength(255)]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your full name.")]
    [StringLength(255)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a password.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
  }
}
