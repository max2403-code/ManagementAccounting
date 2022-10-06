using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ManagementAccounting
{
    public class BlockItemFormsCollection : IBlockItemFormsCollection
    {
        private readonly Dictionary<Type, Func<object, IOperationsWithUserInput, Form>> _itemForms;
        private readonly Dictionary<string, Func<IProgramBlock, IOperationsWithUserInput, Form>> _addItemForms;

        public BlockItemFormsCollection(Dictionary<Type, Func<object, IOperationsWithUserInput, Form>> itemForms,
            Dictionary<string, Func<IProgramBlock, IOperationsWithUserInput, Form>> addItemForms)
        {
            _itemForms = itemForms;
            _addItemForms = addItemForms;
        }

        public Form GetItemForm(Type type, object obj, IOperationsWithUserInput inputOperations)
        {
            return _itemForms[type](obj, inputOperations);
        }

        public Form GetAddItemForm(string typeOfMaterial, IProgramBlock block, IOperationsWithUserInput inputOperations)
        {
            return _addItemForms[typeOfMaterial](block, inputOperations);
        }
    }
}
