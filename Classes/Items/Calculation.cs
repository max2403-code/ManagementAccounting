using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Items;
using Npgsql;
using NpgsqlTypes;
using BlockItemDB = ManagementAccounting.Classes.Abstract.BlockItemDB;

namespace ManagementAccounting
{
    public class Calculation : BlockItemDB
    {
        private IItemsFactory itemsFactory { get; }

        public Calculation(string calculationName, int index, IDataBase dataBase, IItemsFactory itemsFactory) : base(index, calculationName, dataBase)
        {
            this.itemsFactory = itemsFactory;
        }
        private protected override string GetAddItemCommandText()
        {
            return "INSERT INTO calculations (CalculationNamec) VALUES (@CalculationNamec) RETURNING IdC";
        }

        private protected override string GetAddExceptionMessage()
        {
            return "Калькуляция с таким названием уже существует";
        }

        private protected override void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("CalculationNamec", NpgsqlDbType.Varchar, 50, Name);
        }

        private protected override void AssignIndex(DbDataRecord index)
        {
            Index = (int)index["IdC"];
        }

        private protected override IBlockItem GetCopyItem()
        {
            return itemsFactory.CreateCalculation(Name, Index);
        }

        private protected override string GetEditItemCommandText()
        {
            return $"UPDATE calculations SET CalculationNamec = @CalculationNamec WHERE idc = {Index}";
        }

        private protected override void UpdateItem(object[] parameters)
        {
            Name = (string)parameters[0];
        }

        private protected override string GetEditExceptionMessage()
        {
            return "Калькуляция с таким названием уже существует";
        }

        private protected override void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            AssignParametersToAddCommand(cmd);
        }

        private protected override void UndoValues(IBlockItem copyItem)
        {
            var copyCalculation = copyItem as BlockItemDB;
            Name = copyCalculation.Name;

        }

        private protected override string GetRemoveItemCommandText()
        {
            return $"DELETE FROM calculations WHERE idc = {Index}";
        }

        private protected override string GetRemoveExceptionMessage()
        {
            return "Калькуляция используется в предзаказе";
        }


        

        //public void AssignParametersToAddCommand(NpgsqlCommand cmd)
        //{
        //    cmd.Parameters.AddWithValue("CalculationNamec", NpgsqlDbType.Varchar, 50, Name);
        //}

        //public void AssignParametersToEditCommand(NpgsqlCommand cmd)
        //{
        //    cmd.Parameters.AddWithValue("CalculationNamec", NpgsqlDbType.Varchar, 50, Name);
        //}

        //public async Task AddItemToDataBase()
        //{
        //    var commandText = "INSERT INTO calculations (CalculationNamec) VALUES (@CalculationNamec)";
        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Калькуляция с таким названием уже существует", AssignParametersToAddCommand);
        //    ExceptionEvent?.Invoke();
        //}

        //public async Task EditItemInDataBase(params object[] parameters)
        //{
        //    var copyCalculation = itemsFactory.CreateCalculation(Name, Index);
        //    var commandText = $"UPDATE calculations SET CalculationNamec = @CalculationNamec WHERE idc = {Index}";
            
        //    Name = (string)parameters[0];

        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Калькуляция с таким названием уже существует", AssignParametersToEditCommand);

        //    if (ExceptionEvent != null)
        //    {
        //        Name = copyCalculation.Name;
        //        ExceptionEvent.Invoke();
        //    }
        //}

        //public async Task RemoveItemFromDataBase()
        //{
        //    var commandText = $"DELETE FROM calculations WHERE idc = {Index}";
        //    if (ExceptionEvent != null) ExceptionEvent = null;
        //    await dataBase.ExecuteNonQueryAndReaderAsync(this, commandText, "Калькуляция используется в наряд-заказе");
        //    ExceptionEvent?.Invoke();
        //}

    }
}
