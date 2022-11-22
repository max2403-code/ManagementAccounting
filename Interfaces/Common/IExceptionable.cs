using System;

namespace ManagementAccounting
{
    public interface IExceptionable
    {
        //public delegate void ExceptionEvent();
        protected Action ExceptionEvent { get; set; }


        //public event Action ExceptionEvent;
        //int Index { get; }

    }
}
