using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class CalculationItem : EditingBlockItemDB, ICalculationItem
    {
        public string Name { get;  }
        public int Index { get; private set; }
        public int CalculationId { get; }
        public IMaterial Material { get; }
        public decimal Consumption { get; private set; }
        private IItemsFactory ItemsFactory { get; }


        public CalculationItem(IMaterial material,  decimal consumption, int calculationId, int index,  IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exceptionChecker) : base(dataBase, exceptionChecker)
        {
            Index = index;
            Material = material;
            Name = Material.Name;
            CalculationId = calculationId;
            Consumption = consumption;
            ItemsFactory = itemsFactory;
        }
        
        private protected override string GetAddItemCommandText(bool isPreviouslyExistingItem = false)
        {
            return
                "INSERT INTO calculationitems (CalculationIdci, MaterialIdci, ConsumptionCI) VALUES (@CalculationIdci, @MaterialIdci, @ConsumptionCI) RETURNING IdCI";
        }

        private protected override string GetAddExceptionMessage()
        {
            return "Введены некорректные данные";
        }

        private protected override void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("MaterialIdci", NpgsqlDbType.Integer, Material.Index);
            cmd.Parameters.AddWithValue("CalculationIdci", NpgsqlDbType.Integer, CalculationId);
            AssignParametersToEditCommand(cmd);
        }

        private protected override void AssignIndex(DbDataRecord index)
        {
            Index = (int)index["IdCI"];
        }

        private protected override T GetCopyItem<T>()
        {
            return (T) ItemsFactory.CreateCalculationItem(Material, Consumption, CalculationId, Index);
        }

        private protected override string GetEditItemCommandText()
        {
            return $"UPDATE calculationitems SET ConsumptionCI = @ConsumptionCI WHERE idci = {Index};";
        }

        private protected override void UpdateItem(object[] parameters)
        {
            Consumption = (decimal)parameters[0];
        }

        private protected override string GetEditExceptionMessage()
        {
            return "Введены некорректные данные";
        }

        private protected override void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("ConsumptionCI", NpgsqlDbType.Numeric, Consumption);
        }

        private protected override void UndoValues<T>(T copyItem)
        {
            var copyCalculationItem = copyItem as ICalculationItem;
            Consumption = copyCalculationItem.Consumption;
        }

        private protected override string GetRemoveItemCommandText()
        {
            return $"DELETE FROM calculationitems WHERE idci = {Index};";
        }

        private protected override string GetRemoveExceptionMessage()
        {
            return "Проблемы с БД";
        }
    }
}