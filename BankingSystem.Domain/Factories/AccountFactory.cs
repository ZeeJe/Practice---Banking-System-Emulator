using System;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Services;

namespace BankingSystem.Domain.Factories
{
    public static class AccountFactory
    {
        // Метод динамічного створення сутностей на основі рядкового ідентифікатора
        public static Account CreateAccount(string accountType, string ownerName, decimal initialBalance)
        {
            // switch-вираз у C# для елегантної фабрики
            return accountType.ToLower() switch
            {
                "checking" => new CheckingAccount(ownerName, initialBalance),
                
                // Депозит бере відсоткову ставку безпосередньо з нашого Singleton-конфігу!
                "deposit" => new Deposit(
                    ownerName, 
                    initialBalance, 
                    DateTime.Now.AddYears(1), 
                    CentralBank.Instance.BaseInterestRate),
                
                // Якщо прийшла невідома конфігурація — кидаємо помилку
                _ => throw new ArgumentException($"[Помилка Фабрики] Невідомий тип рахунку: '{accountType}'")
            };
        }
    }
}