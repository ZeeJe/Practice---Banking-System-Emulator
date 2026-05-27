using System;

namespace BankingSystem.Domain.Entities
{
    // Наслідування (Deposit є різновидом Account)
    public class Deposit : Account
    {
        public DateTime EndDate { get; private set; }
        public decimal InterestRate { get; private set; }

        // Використання 'base' для виклику конструктора батьківського класу (Account)
        public Deposit(string ownerName, decimal initialBalance, DateTime endDate, decimal interestRate) 
            : base(ownerName, initialBalance) 
        {
            EndDate = endDate;
            InterestRate = interestRate;
        }

        // ==========================================
        // ПРАКТИЧНА 3: Поліморфізм та використання base
        // ==========================================
        public override void WithdrawMoney(decimal amount)
        {
            if (DateTime.Now < EndDate)
            {
                throw new InvalidOperationException($"Заборонено знімати кошти з депозиту до {EndDate.ToShortDateString()}!");
            }
            
            // Якщо дата пройшла - викликаємо оригінальну логіку батька через 'base'
            base.WithdrawMoney(amount); 
        }

        // ==========================================
        // САМОСТІЙНА 3: override vs new
        // ==========================================
        
        // 1. Правильний підхід: Перевизначення (override)
        public override string GetAccountDetails()
        {
            return $"[Депозит] № {AccountNumber} під {InterestRate}%. До {EndDate.ToShortDateString()}";
        }

        // 2. Поганий підхід: Приховування (new)
        public new string GetTerms()
        {
            return $"[Умови депозиту] Гроші заморожені до завершення терміну.";
        }
    }
}