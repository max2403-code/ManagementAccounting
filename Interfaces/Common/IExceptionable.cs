using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IExceptionable
    {
        public event Action ExceptionEvent;
        //int Index { get; }

    }
}
