using System.Threading.Tasks;

namespace HrBoxApi.Jobs.Interfaces
{
  public interface ITokenJob
  {
    Task CleanupUserTokens();
  }
}
