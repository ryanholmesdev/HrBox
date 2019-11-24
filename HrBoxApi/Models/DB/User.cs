using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrBoxApi.Models.DB
{
  public class User : BaseEntity
  {
    [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required.")]
    [MinLength(2, ErrorMessage = "First name must be minimum 2 characters long.")]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    [RegularExpression(@"^\S*$", ErrorMessage = "First name must not have white spaces.")]
    public string FirstName { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required.")]
    [MinLength(2, ErrorMessage = "Last name must be minimum 2 characters long.")]
    [MaxLength(80, ErrorMessage = "Last name cannot exceed 80 characters.")]
    [RegularExpression(@"^\S*$", ErrorMessage = "Last name must not have white spaces.")]
    public string LastName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [MinLength(2, ErrorMessage = "Email must be minimum 2 characters long.")]
    [MaxLength(300, ErrorMessage = "Email must not exceed 300 characters.")]
    [RegularExpression(@"^\S*$", ErrorMessage =  "Email must not have white spaces.")]
    public string Email { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MinLength(6, ErrorMessage = "Password must be minimum 5 characters.")]
    [MaxLength(200, ErrorMessage = "Password must not exceed 200 characters.")]
    [RegularExpression(@"^\S*$", ErrorMessage = "The password must not have white spaces.")]
    public string Password { get; set; }

    [NotMapped]
    public string FullName
    {
      get
      {
        return $"{FirstName} {LastName}";
      }
    }

    public DateTime? DateOfBirth { get; set; }

    public DateTime CreatedUtc { get; set; }

    public ICollection<UserToken> UserTokens { get; set; }
  }
}
