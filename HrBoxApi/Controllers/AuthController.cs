using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HrBoxApi.Filters;
using HrBoxApi.Models;
using HrBoxApi.Models.Requests;
using HrBoxApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrBoxApi.Controllers
{
  [Route("api/auth")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    IAuthService _authService;

    public AuthController(IAuthService authService)
    {
      _authService = authService;
    }

    [HttpPost("login"), ModelValidator]
    public async Task<LoginResponse> Login([FromForm] LoginRequest request)
    {
      LoginResponse response = _authService.LoginUser(request.Email, request.Password);
      return response;
    }

    [HttpPost("RefreshToken"), AllowAnonymous]
    public async Task<TokenResponse> RefreshToken([FromForm]string token, [FromForm]string refreshToken)
    {
      TokenResponse response = _authService.RefreshToken(token, refreshToken);
      return response;
    }
  }
}