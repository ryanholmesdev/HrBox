using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HrBoxApi.Models.DB
{
  public class UserToken : BaseEntity
  {
    [Required]
    public int UserID { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public string RefreshToken { get; set; }

    [Required]
    public DateTime TokenExpiryDateUtc { get; set; }

    [Required]
    public DateTime RefreshTokenExpiryDateUtc { get; set; }

    public User User { get; set; }
  }
}
