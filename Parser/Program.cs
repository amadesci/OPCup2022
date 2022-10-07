using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Parser.Models;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "D:/Илья/Desktop/transactions.json";

            UseDetectorConsole(path);

            Console.Read();
        }

        static void UseDetectorConsole(string path)
        {
            var data = Read(path);
            var repeating = FindRepeating(data);

            var ans = Check(repeating);

            var equals = Detector.EqualPattern(data, 180);

            if (equals.Count > 0)
                ans.Add(equals);

            Cout(ans);
        }

        static List<List<Transaction>> Check(List<List<Transaction>> rep)
        {
            List<List<Transaction>> ans = new List<List<Transaction>>();

            foreach (var i in rep)
            {
                var srt = SortByDate(i);
                byte 
                    pt_result1 = 0,
                    pt_result2 = 0,
                    pt_result3 = 0,
                    pt_result4 = 0;

                pt_result1 = Detector.DescendingPattern(srt, OperType.Withdrawal);
                if(pt_result1 == 1) Mark(srt, FraudPattern.DescendingWithdrawalPattern);

                pt_result2= Detector.DescendingPattern(srt, OperType.Payment);
                if (pt_result2 == 1) Mark(srt, FraudPattern.DescendingPaymentPattern);

                pt_result3 = Detector.TryPattern(srt, OperType.Withdrawal);
                if (pt_result3 == 1) Mark(srt, FraudPattern.TryPattern);

                pt_result4 = Detector.FrequentPattern(srt, OperType.Withdrawal);
                if (pt_result4 == 1) Mark(srt, FraudPattern.FrequentPattern);
                //if (pt_result4 == 1) print(srt);

                if ((pt_result1 | pt_result2 | pt_result3 | pt_result4) == 1)
                {
                    ans.Add(srt);
                }
            }

            return ans;
        }

        //static void print(List<Transaction> group)
        //{
        //    foreach(var i in group)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Gray;
        //        Console.WriteLine(i.Card);
        //        Console.ForegroundColor = ConsoleColor.White;
        //        Console.WriteLine(i.Date.DateTime.ToShortDateString() + 'T' + i.Date.DateTime.ToShortTimeString());
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine(i.Amount);
        //        Console.ForegroundColor = ConsoleColor.Gray;
        //        Console.WriteLine(i.Terminal);
        //        Console.WriteLine(i.City);
        //        Console.ForegroundColor = ConsoleColor.DarkGray;
        //        Console.WriteLine(i.OperType);
        //        Console.ForegroundColor = ConsoleColor.DarkGreen;
        //        Console.WriteLine(i.OperResult);
        //        Console.ForegroundColor = ConsoleColor.Gray;
        //    }
        //    Console.WriteLine("\n==============================\n");
        //}

        static void Mark(List<Transaction> transactions, FraudPattern pattern)
        {
            foreach(var i in transactions)
            {
                i.FraudPatterns.Add(pattern);
            }
        }

        static List<Transaction> Read(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                List<Transaction> transactions = new List<Transaction>();

                var data = JsonConvert.DeserializeObject<Transactions>(json);

                foreach (var i in data._Transactions)
                {
                    var ts = i.Value;
                    ts.TransactionId = i.Key;

                    transactions.Add(ts);
                }

                return transactions;
            }
        }

        static List<List<Transaction>> FindRepeating(List<Transaction> data)
        {
            List<List<Transaction>> repTransactions = new List<List<Transaction>>();
            var sorted = (from entry in data orderby entry.Card ascending select entry).ToList();

            var len = sorted.Count;
            bool eq = false;

            var subTransactions = new List<Transaction>();
            for (int i = 1; i < len; i++)
            {
                if (sorted[i].Card == sorted[i - 1].Card)
                {
                    if (eq == false)
                    {
                        eq = true;

                        subTransactions = new List<Transaction>();

                        subTransactions.Add(sorted[i]);
                        subTransactions.Add(sorted[i - 1]);
                    }
                    else
                    {
                        subTransactions.Add(sorted[i]);
                    }
                }
                if (sorted[i].Card != sorted[i - 1].Card && eq)
                {
                    eq = false;

                    repTransactions.Add(subTransactions);
                    subTransactions = new List<Transaction>();
                }

            }

            return repTransactions;
        }

        static List<Transaction> SortByDate(List<Transaction> data)
        {
            var sorted = (from entry in data orderby entry.Date.DateTime ascending select entry).ToList();
            return sorted;
        }

        static void Cout(List<List<Transaction>> transactionGroups)
        {
            foreach(var g in transactionGroups)
            {
                foreach(var t in g)
                {
                    Console.WriteLine(t.ToString());
                    Console.WriteLine();
                }
                Console.WriteLine("\n-group-\n");
            }

            Microsoft.Office.Interop.Excel.Application oXL;
            Microsoft.Office.Interop.Excel._Workbook oWB;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;
            Microsoft.Office.Interop.Excel.Range oRng;
            object misvalue = System.Reflection.Missing.Value;
            try
            {
                //Start Excel and get Application object.
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

                //Add table headers going cell by cell.
                oSheet.get_Range("A1", "V1").EntireColumn.NumberFormat = "@";
                var prs = typeof(Transaction).GetProperties();
                int len = prs.Count();
                oSheet.Cells[1, 1] = "Fraud";
                for (int i = 0; i < len; i++)
                {
                    oSheet.Cells[1, i + 2] = prs[i].Name;
                }

                //Format A1:D1 as bold, vertical alignment = center.
                oSheet.get_Range("A1", "V1").Font.Bold = true;
                oSheet.get_Range("A1", "V1").VerticalAlignment =
                    Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                int ROW = 2;

                for (int i = 0; i < transactionGroups.Count; i++)
                {
                    for (int j = 0; j < transactionGroups[i].Count; j++)
                    {
                        for (int m = 0; m < transactionGroups[i][j].FraudPatterns.Count; m++)
                        {
                            oSheet.Cells[ROW, 1] = transactionGroups[i][j].FraudPatterns[m].Value;
                            var props = typeof(Transaction).GetProperties();

                            for (int p = 0; p < props.Length; p++)
                            {
                                if(props[p].Name == "Date")
                                    oSheet.Cells[ROW, p + 2] = transactionGroups[i][j].Date.DateTime.ToString();
                                else if(props[p].Name == "AccountValidTo")
                                    oSheet.Cells[ROW, p + 2] = transactionGroups[i][j].AccountValidTo.DateTime.ToString();
                                else if(props[p].Name == "DateOfBirth")
                                    oSheet.Cells[ROW, p + 2] = transactionGroups[i][j].DateOfBirth.DateTime.ToString();
                                else if(props[p].Name == "PassportValidTo")
                                    oSheet.Cells[ROW, p + 2] = transactionGroups[i][j].PassportValidTo.DateTime.ToString();
                                else
                                    oSheet.Cells[ROW, p + 2] = props[p].GetValue(transactionGroups[i][j]).ToString();
                            }
                            ROW++;
                        }
                    }
                }

                //AutoFit columns A:D.
                oSheet.get_Range("A1", "V1").EntireColumn.AutoFit();

                oXL.Visible = false;
                oXL.UserControl = false;
                oWB.SaveAs(
                    "D:\\Илья\\Desktop\\output1.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault,
                    Type.Missing, Type.Missing,
                    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing
                    );

                oWB.Close();
                oXL.Quit();

                //...
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
