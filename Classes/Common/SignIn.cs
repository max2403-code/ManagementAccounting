using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;

namespace ManagementAccounting.Classes.Common
{
    class SignIn : ISignIn
    {
        private IExceptionChecker ExceptionChecker { get; }
        private IDataBase DataBase { get; }

        public SignIn(IDataBase dataBase, IExceptionChecker exceptionChecker)
        {
            ExceptionChecker = exceptionChecker;
            DataBase = dataBase;
        }

        public async Task SigningIn(string login, string password)
        {
            ExceptionChecker.IsExceptionHappened = false;
            await DataBase.SignInAsync(ExceptionChecker, login, password);
            if(ExceptionChecker.IsExceptionHappened) ExceptionChecker.DoException("Неверный логин и(или) пароль");
        }
    }
}
