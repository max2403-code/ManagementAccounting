using System.Threading.Tasks;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting.Classes.Common
{
    public class EmptyCalculationChecker : IEmptyCalculationChecker
    {
        private ICreatorFactory CreatorFactory { get; }

        public EmptyCalculationChecker(ICreatorFactory creatorFactory)
        {
            CreatorFactory = creatorFactory;
        }

        public async Task<bool> IsCalculationNotEmpty(ICalculation calculation)
        {
            var calculationCreator = CreatorFactory.CreateCalculationItemCollectionCreator(calculation, 1);
            var resultOfGettingItemsList = await calculationCreator.GetItemsList(0, "");
            var itemsList = resultOfGettingItemsList.Item1;
            return itemsList.Count != 0;
        }
    }
}
