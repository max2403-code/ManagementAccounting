using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface ISignIn
    {
        Task SigningIn(string login, string password);
    }
}
