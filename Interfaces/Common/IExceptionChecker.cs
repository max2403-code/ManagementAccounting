namespace ManagementAccounting.Interfaces.Common
{
    public interface IExceptionChecker
    {
        public bool IsExceptionHappened { get; set; }

        public void DoException(string message);
    }
}
