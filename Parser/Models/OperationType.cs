using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Models
{
    public class OperType
    {
        private OperType(string value) { Value = value; }

        public string Value { get; private set; }

        public static OperType Withdrawal { get { return new OperType("Снятие"); } }
        public static OperType Payment { get { return new OperType("Оплата"); } }
        public static OperType TopUp { get { return new OperType("Пополнение"); } }
    }
}
