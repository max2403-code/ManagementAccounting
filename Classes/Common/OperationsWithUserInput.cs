using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ManagementAccounting
{
    public class OperationsWithUserInput : IOperationsWithUserInput
    {
        private Dictionary<string, string> _translateTypes { get; }

        public OperationsWithUserInput(Dictionary<string, string> translateTypes)
        {
            _translateTypes = translateTypes;
        }

        public bool IsNameCorrect(string name)
        {
            var pattern = @"\S+";
            return Regex.IsMatch(name, pattern);
        }

        public string GetNotEmptyName(string input, int inputMaxLength)
        {
            var pattern = @"\S+";
            if (!Regex.IsMatch(input, pattern) || input.Length > inputMaxLength) throw new Exception($"Введены некорретные данные: {input}");
            return input;
        }

        public decimal GetPositiveDecimal(string input)
        {
            var result = decimal.Parse(input);
            if (result <= 0) throw new Exception($"Введены некорретные данные: {input}");

            return result;
        }

        public decimal GetPositiveDecimalorZero(string input)
        {
            var result = decimal.Parse(input);
            if (result < 0) throw new Exception($"Введены некорретные данные: {input}");

            return result;
        }

        public int GetPositiveInt(string input)
        {
            var result = int.Parse(input);
            if (result <= 0) throw new Exception($"Введены некорретные данные: {input}");

            return result;
        }

        public DateTime GetCorrectData(string input)
        {
            var dateArray = input.Split(".", StringSplitOptions.RemoveEmptyEntries);
            var date = new DateTime(int.Parse(dateArray[2]), int.Parse(dateArray[1]), int.Parse(dateArray[0]));

            return date;
        }

        public string TranslateType(string type)
        {
            return _translateTypes[type];
        }


        public string[] GetTranslateTypesNames(string[] types)
        {
            var result = new string[types.Length];

            for (var i = 0; i < types.Length; i++)
            {
                result[i] = _translateTypes[types[i]];
            }

            return result;
        }

    }
}
