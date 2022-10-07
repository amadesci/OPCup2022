using System;
using System.Collections.Generic;
using Parser.Models;

namespace Parser
{
    public static class Detector
    {
        public static byte DescendingPattern(List<Transaction> transactionGroup, OperType operType)
        {
            var len = transactionGroup.Count;

            for (int i = 1; i < len; i++)
            {
                if (transactionGroup[i - 1].OperResult == OperResult.Success.Value &&
                    i - 1 != len - 1)
                    return 0;
                if (transactionGroup[i].Amount >= transactionGroup[i - 1].Amount ||
                    transactionGroup[i].OperType != transactionGroup[i - 1].OperType ||
                    transactionGroup[i].OperType != operType.Value)
                    return 0;
            }

            return 1;
        }

        public static byte TryPattern(List<Transaction> transactionGroup, OperType operType, int minval = 2000) // Use 2000 as default
        {
            if (transactionGroup[0].Amount >= minval ||
                transactionGroup[0].OperResult == OperResult.Deny.Value ||
                transactionGroup[0].OperType == OperType.TopUp.Value)
                return 0;

            for (int i = 1; i < transactionGroup.Count; i++)
            {
                if (transactionGroup[i].OperType != operType.Value ||
                    transactionGroup[i].OperResult == OperResult.Deny.Value)
                    return 0;
            }

            return 1;
        }

        public static byte FrequentPattern(List<Transaction> transactionGroup, OperType operType, int deltaMinutes = 20) // Use 5 as defalut
        {
            var d = CountDelta(transactionGroup[0], transactionGroup[transactionGroup.Count - 1]);
            if (d
                <=
                deltaMinutes * transactionGroup.Count) // Optimal timespan for specific attempts amount
            {
                return 1;
            }
            else
                return 0;
        }

        public static List<Transaction> EqualPattern(List<Transaction> transactions, int deltaMinutes = 90)
        {
            //List<List<Transaction>> res = new List<List<Transaction>>();
            List<Transaction> res = new List<Transaction>();
            List<Transaction> sub;

            for (int i = 0; i < transactions.Count; i++)
            {
                sub = new List<Transaction>();
                sub.Add(transactions[i]);

                for (int j = i + 1; j < transactions.Count; j++)
                {
                    if ((int)transactions[i].Amount == (int)transactions[j].Amount)
                    {
                        sub.Add(transactions[j]);

                        transactions[j].InGroup = true;

                        if(transactions[i].InGroup == false)
                            transactions[i].InGroup = true;
                    }
                }

                if (sub.Count > 1 &&
                    CountDelta(sub[0], sub[sub.Count - 1]) <= deltaMinutes)
                {
                    res.AddRange(sub);
                }
            }

            foreach (var i in res)
                i.FraudPatterns.Add(FraudPattern.EqualPattern);

            return res;
        }

        private static double CountDelta(Transaction firstTransaction, Transaction lastTransaction)
        {
            return (lastTransaction.Date - firstTransaction.Date).TotalMinutes;
        }
    }
}
