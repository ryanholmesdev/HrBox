using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using HrBoxApi.Data;
using HrBoxApi.Models.DB;
using HrBoxApi.Models;
using HrBoxApi.Filters;
using Microsoft.AspNetCore.Mvc;
using HrBoxApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace HrBoxApi.Controllers
{
  [Route("api/user")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly AppDbContext _context;
    IUserService _userService;

    public UserController(AppDbContext context, IUserService userService)
    {
      _context = context;
      _userService = userService;
    }

    [HttpGet("test"), Authorize]
    public IActionResult test()
    {
      return Ok("hello");
    }

    // POST: api/User
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
    // more details see https://aka.ms/RazorPagesCRUD.
    [HttpPost("CreateUser"), ModelValidator]
    public async Task<ActionResult<Response>> CreateUser([FromForm]User user)
    {
      Response response = await _userService.CreateUserAsync(user);
      return response; 
    }

    [HttpGet("VerifyEmail")]
    public async Task<ActionResult<Response>> VerifyEmail([FromForm]string email, string verifyCode)
    {
      var response = _userService.VerifyUser(email, verifyCode);
      return Ok(response);
    }

    [HttpPost("ResendVerificationEmail")]
    public IActionResult ResendVerificationEmail([FromForm] string email, string resendCode)
    {
      Response response = new Response(true);
      return Ok(response);
    }
  }
}
