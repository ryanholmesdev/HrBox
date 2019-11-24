using System.ComponentModel.DataAnnotations;

namespace HrBoxApi.Models.Requests
{
  public class LoginRequest
  {
    [Required(AllowEmptyStrings = false)]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [MinLength(2, ErrorMessage = "Email must be minimum 2 characters long.")]
    [MaxLength(300, ErrorMessage = "Email must not exceed 300 characters.")]
    [RegularExpression(@"^\S*$", ErrorMessage = "Email must not have white spaces.")]
    public string Email { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MinLength(6, ErrorMessage = "Password must be minimum 5 characters.")]
    [MaxLength(200, ErrorMessage = "Password must not exceed 200 characters.")]
    [RegularExpression(@"^\S*$", ErrorMessage = "The password must not have white spaces.")]
    public string Password { get; set; }
  }
}
