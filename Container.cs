using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
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

            container.Bind<IProgramBlock>().To<Remainders>();
            container.Bind<IProgramBlock>().To<Calculations>();
            container.Bind<IProgramBlock>().To<PreOrders>();


            container.Bind<IMaterial>().To<Material>();
            container.Bind<IMaterialReceiving>().To<MaterialReceiving>();
            container.Bind<ICalculation>().To<Calculation>();
            container.Bind<ICalculationItem>().To<CalculationItem>();
            container.Bind<IPreOrder>().To<PreOrder>();
            container.Bind<IPreOrderItem>().To<PreOrderItem>();



            container.Bind<IBlockItemsFactory>().ToFactory();
            container.Bind<IBlockItemFormsCollection>().To<BlockItemFormsCollection>().InSingletonScope();
            container.Bind<Dictionary<Type, Func<object, IOperationsWithUserInput, Form>>>().ToSelf().WhenInjectedExactlyInto<BlockItemFormsCollection>()
                .WithConstructorArgument
                (
                    "collection",
                    new[]
                    {
                        new KeyValuePair<Type, Func<object, IOperationsWithUserInput, Form>>(typeof(Material), (obj, inputOp) => new MaterialForm((IMaterial)obj, inputOp))
                    }
                );

            container.Bind<Dictionary<string, Func<IProgramBlock, IOperationsWithUserInput, Form>>>().ToSelf().WhenInjectedExactlyInto<BlockItemFormsCollection>()
                .WithConstructorArgument
                (
                    "collection",
                    new[]
                    {
                        new KeyValuePair<string, Func<IProgramBlock, IOperationsWithUserInput, Form>>("material", (block, inputOp) => new AddMaterialForm(block, inputOp))
                    }
                );
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
                    }
                );

            return container;
        }
    }
}
