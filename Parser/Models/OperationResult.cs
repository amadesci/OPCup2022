using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Models
{
    public class OperResult
    {
        private OperResult(string value) { Value = value; }

        public string Value { get; private set; }

        public static OperResult Success { get { return new OperResult("Успешно"); } }
        public static OperResult Deny { get { return new OperResult("Отказ"); } }
    }
}
