using HrBoxApi.Filters;
using HrBoxApi.Models;
using HrBoxApi.Models.Requests;
using HrBoxApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HrBoxApi.Controllers
{
  [Route("api/auth")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    IAuthService _authService;
    ITokenService _tokenService;

    public AuthController(IAuthService authService, ITokenService tokenService)
    {
      _authService = authService;
      _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<LoginResponse> Login([FromBody] LoginRequest request)
    {
      LoginResponse response = _authService.LoginUser(request.Email, request.Password);
      return response;
    }

    [HttpGet("logout"), AllowAnonymous]
    public async Task<bool> Logout()
    {
      // DONT HARDCODE THIS
      _authService.LogoutUser(1);

      return true;
    }


    [HttpPost("RefreshToken"), AllowAnonymous, ProducesResponseType(typeof(TokenResponse), 201)]
    public IActionResult RefreshToken([FromBody] TokenResponse tokens)
    {
      TokenResponse response = _tokenService.RefreshToken(tokens);
      if (response == null) return BadRequest("Unable to refresh token");
      return Ok(response);
    }
  }
}