using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ManagementAccounting
{
    public interface IOperationsWithUserInput
    {
        public bool IsNameCorrect(string name);

        public string TranslateType(string type);

        public string[] GetTranslateTypesNames(string[] types);

        //public int GetLengthOfItemList();
        //public int GetNameItemLength();
    }
}
