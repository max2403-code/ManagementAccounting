using System;
using System.Collections.Generic;
using System.Text;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Classes.ItemCreators;
using ManagementAccounting.Interfaces.ItemCreators;

namespace ManagementAccounting.Interfaces.Factory
{
    public interface ICreatorFactory
    {
        //MaterialCollectionCreator CreateMaterialCreator();
        MaterialReceivingCollectionCreator CreateMaterialReceivingCreator(BlockItemDB material);
        MaterialReceivingNotEmptyCollectionCreator CreateMaterialReceivingNotEmptyCreator(BlockItemDB material);


        //ICalculationCollectionCreator CreateCalculationCreator();

    }
}
