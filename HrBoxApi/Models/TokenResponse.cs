using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HrBoxApi.Models
{
  public class TokenResponse
  {
    public string Token { get; set; }
    public string RefreshToken { get; set; }

    public TokenResponse(string token, string refreshToken)
    {
      Token = token;
      RefreshToken = refreshToken;
    }

    public TokenResponse() { }
  }
}
