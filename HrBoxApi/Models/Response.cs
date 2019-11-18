namespace HrBoxApi.Models
{
  public class Response
  {
    public bool Success { get; set; }
    public string Msg { get; set; }

    public Response(bool success, string msg = "")
    {
      Success = success;
      Msg = msg;
    }
  }
}
