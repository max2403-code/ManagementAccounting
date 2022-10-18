using System;
using System.Collections.Generic;
using System.Text;
using ManagementAccounting.Classes.ItemCreators;
using ManagementAccounting.Forms.RemaindersForms;

namespace ManagementAccounting.Interfaces.Factory
{
    public interface IFormFactory
    {
        AddMaterialForm CreateAddMaterialForm();
        AddMaterialReceivingForm CreateAddMaterialReceivingForm(IMaterial material);
        EditMaterialForm CreateEditMaterialForm(IMaterial material);
        EditMaterialReceivingForm CreateEditMaterialReceivingForm(IMaterialReceiving materialReceiving);
        MaterialForm CreateMaterialForm(IMaterial material, MaterialReceivingCollectionCreator creator, MaterialReceivingNotEmptyCollectionCreator creatorNotEmpty);
        MaterialReceivingForm CreateMaterialReceivingForm(IMaterialReceiving materialReceiving);

    }
}
