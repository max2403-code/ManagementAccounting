using System;
using System.Collections.Generic;

namespace ManagementAccounting
{
    public class OperationsWithUserInput : IOperationsWithUserInput
    {
        private Dictionary<string, string> TranslateTypes { get; }

        public OperationsWithUserInput(Dictionary<string, string> translateTypes)
        {
            TranslateTypes = translateTypes;
        }
        
        public bool TryGetNotEmptyName(string input, int inputMaxLength, out string result)
        {
            var isCorrectValue = input.Length <= inputMaxLength && !string.IsNullOrWhiteSpace(input);
            result = isCorrectValue ? input : null;
            return isCorrectValue;
        }

        public bool TryGetPositiveDecimal(string input, out decimal result)
        {
            var isCorrectValue = decimal.TryParse(input, out result) && result > 0;
            return isCorrectValue;
        }

        public bool TryGetPositiveDecimalOrZero(string input, out decimal result)
        {
            var isCorrectValue = decimal.TryParse(input, out result) && result >= 0;
            return isCorrectValue;
        }

        public bool TryGetPositiveInt(string input, out int result)
        {
            var isCorrectValue = int.TryParse(input, out result) && result > 0;
            return isCorrectValue;
        }

        public bool TryGetCorrectData(string input, out DateTime result)
        {
            var isCorrectValue = input.Length == 10 & DateTime.TryParse(input, out result);
            return isCorrectValue;
        }
        
        public string TranslateType(string type)
        {
            return TranslateTypes[type];
        }
        
        public string[] GetTranslateTypesNames(string[] types)
        {
            var result = new string[types.Length];

            for (var i = 0; i < types.Length; i++)
            {
                result[i] = TranslateTypes[types[i]];
            }

            return result;
        }
    }
}
