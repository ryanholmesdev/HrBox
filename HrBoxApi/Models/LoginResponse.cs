namespace HrBoxApi.Models
{
  public class LoginResponse
  {
    public Response Response { get; set; }
    public TokenResponse Tokens { get; set; }

    public LoginResponse(bool success, string errorMsg = null, string token = null, string refreshToken = null)
    {
      Response = new Response(success, errorMsg);
      if (token != null && refreshToken != null)
      {
        Tokens = new TokenResponse(token, refreshToken);
      }
    }
  }
}
