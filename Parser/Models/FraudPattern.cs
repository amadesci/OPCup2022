using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Models
{
    public class FraudPattern
    {
        private FraudPattern(int value) { Value = value; }

        public int Value { get; private set; }

        public static FraudPattern DescendingWithdrawalPattern { get { return new FraudPattern(1); } }
        public static FraudPattern DescendingPaymentPattern { get { return new FraudPattern(2); } }
        public static FraudPattern TryPattern { get { return new FraudPattern(3); } }
        public static FraudPattern FrequentPattern { get { return new FraudPattern(4); } }
        public static FraudPattern EqualPattern { get { return new FraudPattern(5); } }
    }
}
