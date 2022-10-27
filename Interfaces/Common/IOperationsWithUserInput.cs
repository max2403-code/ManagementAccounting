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

        public string GetNotEmptyName(string input, int inputMaxLength);

        public decimal GetPositiveDecimal(string input);

        public decimal GetPositiveDecimalorZero(string input);

        public int GetPositiveInt(string input);
        
        public DateTime GetCorrectData(string input);

    }
}
