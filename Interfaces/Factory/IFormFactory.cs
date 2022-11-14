using System;
using System.Collections.Generic;
using System.Text;
using ManagementAccounting.Classes.ItemCreators;
using ManagementAccounting.Forms.CalculationsForms;
using ManagementAccounting.Forms.OrdersForms;
using ManagementAccounting.Forms.PreOrdersForms;
using ManagementAccounting.Forms.RemaindersForms;
using ManagementAccounting.Interfaces.Items;

namespace ManagementAccounting.Interfaces.Factory
{
    public interface IFormFactory
    {
        LoginForm CreateLoginForm(MainForm mainForm);
        AddMaterialForm CreateAddMaterialForm();
        AddMaterialReceivingForm CreateAddMaterialReceivingForm(IMaterial material);
        EditMaterialForm CreateEditMaterialForm(IMaterial material);
        EditMaterialReceivingForm CreateEditMaterialReceivingForm(IMaterialReceiving materialReceiving, MaterialReceivingForm materialReceivingForm);
        MaterialForm CreateMaterialForm(IMaterial material);
        MaterialReceivingForm CreateMaterialReceivingForm(IMaterialReceiving materialReceiving);


        AddCalculationForm CreateAddCalculationForm();
        AddCalculationItemForm CreateAddCalculationItemForm(ICalculation calculation);
        EditCalculationForm CreateEditCalculationForm(ICalculation calculation);
        CalculationForm CreateCalculationForm(ICalculation calculation);

        CalculationItemForm CreateCalculationItemForm(ICalculationItem calculationItem);
        EditCalculationItemForm CreateEditCalculationItemForm(ICalculationItem calculationItem);

        EditPreOrderForm CreateEditPreOrderForm(IPreOrder preOrder);
        AddPreOrderForm CreateAddPreOrderForm();
        PreOrderForm CreatePreOrderForm(IPreOrder preOrder);
        CreateOrderForm CreateCreateOrderForm(IOrder order, PreOrderForm preOrderForm);

        OrderForm CreateOrderForm(IOrder order);
    }
}
