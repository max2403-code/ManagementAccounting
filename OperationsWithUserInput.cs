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
