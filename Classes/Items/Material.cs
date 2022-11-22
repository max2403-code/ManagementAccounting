using System.Data.Common;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class Material : EditingBlockItemDB, IMaterial
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        public UnitOfMaterial Unit { get; private set; }
        public MaterialType MaterialType { get; private set; }
        private IItemsFactory ItemsFactory { get; }

        public Material(MaterialType materialType, string name, UnitOfMaterial unit, int index, IDataBase dataBase, IItemsFactory itemsFactory, IExceptionChecker exceptionChecker) : base(dataBase, exceptionChecker)
        {
            Name = name;
            Index = index;
            Unit = unit;
            MaterialType = materialType;
            ItemsFactory = itemsFactory;
        }

        private protected override string GetAddItemCommandText(bool isPreviouslyExistingItem = false)
        {
            return "INSERT INTO materials (materialtypem, materialnamem, unitm) VALUES (@materialtypem, @materialnamem, @unitm) RETURNING IdM";
        }

        private protected override string GetAddExceptionMessage()
        {
            return "Материал с таким именем уже существует";
        }

        private protected override void AssignParametersToAddCommand(NpgsqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("materialtypem", NpgsqlDbType.Integer, (int)MaterialType);
            cmd.Parameters.AddWithValue("materialnamem", NpgsqlDbType.Varchar, 50, Name);
            cmd.Parameters.AddWithValue("unitm", NpgsqlDbType.Integer, (int)Unit);
        }

        private protected override void AssignIndex(DbDataRecord index)
        {
            Index = (int)index["IdM"];
        }

        private protected override T GetCopyItem<T>()
        {
            return (T)ItemsFactory.CreateMaterial(MaterialType, Name, Unit, Index);
        }

        private protected override string GetEditItemCommandText()
        {
            return $"UPDATE materials SET materialtypem = @materialtypem, materialnamem = @materialnamem WHERE idm = {Index}";
        }

        private protected override void UpdateItem(object[] parameters)
        {
            MaterialType = (MaterialType)parameters[0];
            Name = (string)parameters[1];
            Unit = (UnitOfMaterial)parameters[2];
        }

        private protected override string GetEditExceptionMessage()
        {
            return "Материал с таким именем уже существует";
        }

        private protected override void AssignParametersToEditCommand(NpgsqlCommand cmd)
        {
            AssignParametersToAddCommand(cmd);
        }

        private protected override void UndoValues<T>(T copyItem)
        {
            var material = copyItem as IMaterial;
            Name = material.Name;
            MaterialType = material.MaterialType;
            Unit = material.Unit;
        }

        private protected override string GetRemoveItemCommandText()
        {
            return $"DELETE FROM materials WHERE idm = {Index}";
        }

        private protected override string GetRemoveExceptionMessage()
        {
            return "Материал используется в других блоках";
        }
    }
}
