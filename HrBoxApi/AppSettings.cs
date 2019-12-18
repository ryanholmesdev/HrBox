namespace HrBoxApi
{
  public class AppSettings
  {
    public string ConnectionString { get; set; }
    public string CorsDomain { get; set; }
    public string JWTSecret { get; set; }
    public int TokenExpiryMinutes { get; set; } = 30;
    public int RefreshExpiryMinutes { get; set; } = 30;
  }
}
