using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Parser.Models;

namespace Parser
{
    //public enum FraudPattern
    //{
    //    DescendingWithdrawalPattern = 1,
    //    DescendingPaymentPattern,
    //    TryPattern,
    //    FrequentPattern,
    //    EqualPattern
    //}

    public partial class Transactions
    {
        [JsonProperty("transactions")]
        public Dictionary<string, Transaction> _Transactions { get; set; }
    }

    public partial class Transaction
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("card")]
        public string Card { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("account_valid_to")]
        public DateTimeOffset AccountValidTo { get; set; }

        [JsonProperty("client")]
        public string Client { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("patronymic")]
        public string Patronymic { get; set; }

        [JsonProperty("date_of_birth")]
        public DateTimeOffset DateOfBirth { get; set; }

        [JsonProperty("passport")]
        public long Passport { get; set; }

        [JsonProperty("passport_valid_to")]
        public DateTimeOffset PassportValidTo { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("oper_type")]
        public string OperType { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("oper_result")]
        public string OperResult { get; set; }

        [JsonProperty("terminal")]
        public string Terminal { get; set; }

        [JsonProperty("terminal_type")]
        public string TerminalType { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        public string TransactionId { get; set; } = "";

        public List<FraudPattern> FraudPatterns = new List<FraudPattern>();

        public bool InGroup = false;

        public override string ToString()
        {
            return 
                "Date: " + Date.Date.ToShortDateString() + 'T' + Date.DateTime.ToLongTimeString() + '\n' +
                "Card: " + Card + '\n' +
                //" -Card Type: " + Card.GetType() + '\n' +
                "Account: " + Account + '\n' +
                "Account Valid To: " + AccountValidTo.Date.Date.ToShortDateString() + '\n' +
                "Client: " + Client + '\n' +
                "Firstname: " + FirstName + '\n' +
                "Lastname: " + LastName + '\n' +
                "Patronymic: " + Patronymic + '\n' +
                "Passport: " + Passport + '\n' +
                "Passprot Valid To: " + PassportValidTo.Date.Date.ToShortDateString() + '\n' +
                "Oper Type: " + OperType + '\n' +
                "Oper Result: " + OperResult + '\n' +
                "Amount: " + Amount + '\n' +
                "Terminal: " + TerminalType + '\n' +
                "City: " + City + '\n' + 
                "Address: " + Address + '\n' +
                "Frauds: " + frauds();
        }

        string frauds()
        {
            string res = "";
            foreach (var i in FraudPatterns)
                res += i.ToString() + ' ';
            return res;
        }
    }
}
