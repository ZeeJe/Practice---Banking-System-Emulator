using System;

namespace BankingSystem.Domain.Events
{
    // Кастомний клас для передачі деталей події підписникам
    public class TransactionEventArgs : EventArgs
    {
        public string FromAccount { get; }
        public string ToAccount { get; }
        public decimal Amount { get; }
        public decimal Fee { get; }

        public TransactionEventArgs(string from, string to, decimal amount, decimal fee)
        {
            FromAccount = from;
            ToAccount = to;
            Amount = amount;
            Fee = fee;
        }
    }
}