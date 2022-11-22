using System.Linq;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Common;
using ManagementAccounting.Classes.ItemCreators;
using Ninject;
using NUnit.Framework;

namespace ManagementAccounting.Classes.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public async Task AddMaterialTest()
        {
            var itemsFactory = Container.ConfigureContainer().Get<IItemsFactory>(); 
            var exChecker = new NpgsqlExceptionChecker();
            var dataBase = new DataBase();
            var materialName = "ТестТкань1";
            var materialType = MaterialType.Fabric;
            var unitOfMaterial = UnitOfMaterial.m2;
            
            await dataBase.SignInAsync(exChecker, "postgres", "user");

            var material = new Material(materialType, materialName, unitOfMaterial, -1, dataBase, itemsFactory,
                exChecker);
            var materialCreator = new MaterialCollectionCreator(1, dataBase, itemsFactory, exChecker);

            await material.AddItemToDataBase();

            var creatorResult = await materialCreator.GetItemsList(0, materialName.ToLower());
            var resultMaterial = creatorResult.Item1.FirstOrDefault() as Material;
            await material.RemoveItemFromDataBase();

            Assert.AreEqual(materialName, resultMaterial.Name);
        }
    }
}
