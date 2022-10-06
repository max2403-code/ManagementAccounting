using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ManagementAccounting
{
    public interface IBlockItemFormsCollection
    {
        public Form GetItemForm(Type type, object obj, IOperationsWithUserInput inputOperations);
        public Form GetAddItemForm(string typeOfMaterial, IProgramBlock block, IOperationsWithUserInput inputOperations);
    }
}
