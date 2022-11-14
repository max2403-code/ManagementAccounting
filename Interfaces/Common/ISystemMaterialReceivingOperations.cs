using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAccounting.Interfaces.Common
{
    public interface ISystemMaterialReceivingOperations
    {
        Task Insert(IMaterialReceiving materialReceiving, bool isPreviouslyExistingItem = false);
        Task Remove(IMaterialReceiving materialReceiving);
        Task Edit(IMaterialReceiving materialReceiving, IMaterialReceiving newMaterialReceiving);
        Task Default(IMaterialReceiving materialReceiving, IMaterialReceiving previousMaterialReceiving);

    }
}
