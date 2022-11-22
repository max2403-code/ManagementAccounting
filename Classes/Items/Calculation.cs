using System.Data.Common;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Items;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class Calculation : EditingBlockItemDB, ICalculation
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        private IItemsFactory ItemsFactory { get; }

        public Calculation(string calculationName, int index, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exceptionChecker) : base(dataBase, exceptionChecker)
        {
            Name = calculationName;
            Index = index;
            ItemsFactory = itemsFactory;
        }
        private protected override string GetAddItemCommandText(bool isPreviouslyExistingItem = false)
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

        private protected override T GetCopyItem<T>()
        {
            return (T)ItemsFactory.CreateCalculation(Name, Index);
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

        private protected override void UndoValues<T>(T copyItem)
        {
            var copyCalculation = copyItem as ICalculation;
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
