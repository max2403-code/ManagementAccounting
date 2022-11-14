using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ManagementAccounting
{
    public interface IOperationsWithUserInput
    {
        //public bool IsNameCorrect(string name);

        public string TranslateType(string type);

        public string[] GetTranslateTypesNames(string[] types);

        public bool TryGetNotEmptyName(string input, int inputMaxLength, out string result);

        public bool TryGetPositiveDecimal(string input, out decimal result);

        public bool TryGetPositiveDecimalOrZero(string input, out decimal result);

        public bool TryGetPositiveInt(string input, out int result);

        public bool TryGetCorrectData(string input, out DateTime result);
    }
}
