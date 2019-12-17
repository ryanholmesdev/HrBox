using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HrBoxApi.Models
{
  public class CustomJwtBearerEvents : JwtBearerEvents
  {
    public override async Task TokenValidated(TokenValidatedContext context)
    {
      // Add the access_token as a claim, as we may actually need it
      var accessToken = context.SecurityToken as JwtSecurityToken;

      Claim IsEmailValid = accessToken.Claims.Where(c => c.Type == "EmailVerified").FirstOrDefault();
      if (IsEmailValid != null)
      {
        if (Convert.ToBoolean(IsEmailValid.Value) == true)
        {
          return;
        }
        else
        {
          throw new SecurityTokenValidationException("Email needs to be verified");
        }
      }
      else
      {
        throw new SecurityTokenValidationException("EmailVerified claim doesnt exist");
      }
    }
  }
}
