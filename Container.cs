using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Classes.Common;
using ManagementAccounting.Classes.ItemCreators;
using ManagementAccounting.Interfaces.Common;
using ManagementAccounting.Interfaces.Factory;
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

            container.Bind<BlockItemsCollectionCreator>().To<MaterialCollectionCreator>().WithConstructorArgument("lengthOfItemsList", 5);
            container.Bind<BlockItemsCollectionCreator>().To<CalculationCollectionCreator>().WithConstructorArgument("lengthOfItemsList", 5);
            container.Bind<BlockItemsCollectionCreator>().To<PreOrderCollectionCreator>().WithConstructorArgument("lengthOfItemsList", 5);
            container.Bind<BlockItemsCollectionCreator>().To<OrderCollectionCreator>().WithConstructorArgument("lengthOfItemsList", 5);




            //container.Bind<BlockItemsCollectionCreator>().To<MaterialReceivingCollectionCreator>();
            //container.Bind<BlockItemsCollectionCreator>().To<MaterialReceivingNotEmptyCollectionCreator>();
            //container.Bind<ICalculationItemCollectionCreator>().To<CalculationItemCollectionCreator>();

            //container.Bind<AddMaterialForm>().ToSelf();

            //container.Bind<IProgramBlock>().To<PreOrders>();


            container.Bind<IMaterial>().To<Material>();
            container.Bind<IMaterialReceiving>().To<MaterialReceiving>();
            container.Bind<ICalculation>().To<Calculation>();
            container.Bind<ICalculationItem>().To<CalculationItem>();
            container.Bind<IPreOrder>().To<PreOrder>();
            container.Bind<IPreOrderItem>().To<PreOrderItem>();
            container.Bind<IOrder>().To<Order>();
            container.Bind<IOrderItem>().To<OrderItem>();
            container.Bind<IOrderMaterialReceiving>().To<OrderMaterialReceiving>();

            container.Bind<IFromPreOrderToOrderConverter>().To<FromPreOrderToOrderConverter>();
            container.Bind<IOrderCostPrice>().To<OrderCostPrice>();
            container.Bind<IOrderItemCostPrice>().To<OrderItemCostPrice>();
            container.Bind<IOrderItemOperations>().To<OrderItemOperations>();
            container.Bind<IPreOrderCostPrice>().To<PreOrderCostPrice>();
            container.Bind<ISystemMaterialReceivingOperations>().To<SystemMaterialReceivingOperations>();
            container.Bind<ISystemOrderItemOperations>().To<SystemOrderItemOperations>();
            container.Bind<ISystemOrderOperations>().To<SystemOrderOperations>();
            container.Bind<ISignIn>().To<SignIn>().InSingletonScope();
            container.Bind<IExceptionChecker>().To<NpgsqlExceptionChecker>();
            container.Bind<IEmptyCalculationChecker>().To<EmptyCalculationChecker>();
            //container.Bind<IFromPreOrderToOrderConverter>().To<FromPreOrderToOrderConverter>();





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
