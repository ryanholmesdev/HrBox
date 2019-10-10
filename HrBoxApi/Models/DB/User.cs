using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HrBoxApi.Models.DB
{
  public class User : BaseEntity
  {
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    [NotMapped]
    public string FullName
    {
      get
      {
        return $"{FirstName} {LastName}";
      }
    }

    public DateTime DateOfBirth { get; set; }
    public DateTime DateCreated { get; set; }
  }
}
