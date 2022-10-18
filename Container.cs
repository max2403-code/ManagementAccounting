using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Classes.ItemCreators;
using ManagementAccounting.Interfaces.Factory;
using ManagementAccounting.Interfaces.ItemCreators;
using ManagementAccounting.Interfaces.Items;
using Ninject;
using Ninject.Extensions.Factory;

namespace ManagementAccounting
{
    public static class Container
    {
        public static MainForm CreateMainForm()
        {
            return ConfigureContainer().Get<MainForm>();
        }

        public static StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();
            container.Bind<IDataBase>().To<DataBase>().InSingletonScope();

            container.Bind<BlockItemsCollectionCreator>().To<MaterialCollectionCreator>();

            //container.Bind<BlockItemsCollectionCreator>().To<MaterialReceivingCollectionCreator>();
            //container.Bind<BlockItemsCollectionCreator>().To<MaterialReceivingNotEmptyCollectionCreator>();
            //container.Bind<ICalculationItemCollectionCreator>().To<CalculationItemCollectionCreator>();

            //container.Bind<AddMaterialForm>().ToSelf();

            //container.Bind<IProgramBlock>().To<PreOrders>();


            //container.Bind<IMaterial>().To<Material>();
            //container.Bind<IMaterialReceiving>().To<MaterialReceiving>();
            //container.Bind<Calculation>().To<Calculation>();
            //container.Bind<ICalculationItem>().To<CalculationItem>();
            //container.Bind<IPreOrder>().To<PreOrder>();
            //container.Bind<IPreOrderItem>().To<PreOrderItem>();


            container.Bind<ICreatorFactory>().ToFactory();
            container.Bind<IItemsFactory>().ToFactory();
            container.Bind<IFormFactory>().ToFactory();



            container.Bind<IOperationsWithUserInput>().To<OperationsWithUserInput>().InSingletonScope();
            
            container.Bind<Dictionary<string, string>>().ToSelf().WhenInjectedExactlyInto<OperationsWithUserInput>()
                .WithConstructorArgument
                (
                    "collection",
                    new[]
                    {
                        new KeyValuePair<string, string>("Fabric", "Ткань"),
                        new KeyValuePair<string, string>("Accessories", "Фурнитура"),
                        new KeyValuePair<string, string>("Other", "Прочее"),
                        new KeyValuePair<string, string>("Ткань", "Fabric"),
                        new KeyValuePair<string, string>("Фурнитура", "Accessories"),
                        new KeyValuePair<string, string>("Прочее", "Other"),
                        new KeyValuePair<string, string>("g", "гр"),
                        new KeyValuePair<string, string>("m2", "м2"),
                        new KeyValuePair<string, string>("pcs", "шт"),
                        new KeyValuePair<string, string>("гр", "g"),
                        new KeyValuePair<string, string>( "м2", "m2"),
                        new KeyValuePair<string, string>( "шт", "pcs")
                    }
                );

            return container;
        }
    }
}
