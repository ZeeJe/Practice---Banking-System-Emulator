using System;

namespace BankingSystem.Domain.Entities
{
    public class Transaction
    {
        public Guid TransactionId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Type { get; private set; } // "Deposit" або "Withdraw"

        public Transaction(decimal amount, string type)
        {
            TransactionId = Guid.NewGuid();
            Amount = amount;
            Timestamp = DateTime.Now;
            Type = type;
        }

        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Type}: {Amount} UAH (ID: {TransactionId.ToString().Substring(0,8)})";
        }
    }
}