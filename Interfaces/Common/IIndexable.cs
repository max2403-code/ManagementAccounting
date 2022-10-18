using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementAccounting
{
    public interface IIndexable
    {
        public event Action ExceptionEvent;
        int Index { get; }

    }
}
