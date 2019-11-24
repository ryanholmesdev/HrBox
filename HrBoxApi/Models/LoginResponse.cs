namespace HrBoxApi.Models
{
  public class LoginResponse
  {
    public Response Response { get; set; }
    public TokenResponse Token { get; set; }

    public LoginResponse(bool success, string errorMsg = null, string token = null, string refreshToken = null)
    {
      Response = new Response(success, errorMsg);
      if (token != null && refreshToken != null)
      {
        Token = new TokenResponse(token, refreshToken);
      }
    }
  }
}
