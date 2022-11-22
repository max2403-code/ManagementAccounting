using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting.Interfaces.Common
{
    public interface IEmptyCalculationChecker
    {
        Task<bool> IsCalculationNotEmpty(ICalculation calculation);
    }
}
