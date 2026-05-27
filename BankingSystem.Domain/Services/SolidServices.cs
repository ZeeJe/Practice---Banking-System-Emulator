using System;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Domain.Services
{
    // SRP: Цей клас відповідає ТІЛЬКИ за відображення даних у консоль.
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message) => Console.WriteLine($"[INFO]: {message}");
        
        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR]: {message}");
            Console.ResetColor();
        }
    }

    // OCP & LSP: Стандартна комісія (1%).
    public class StandardFeeStrategy : ITransactionFeeStrategy
    {
        public decimal CalculateFee(decimal amount) => amount * 0.01m;
    }

    // OCP & LSP: VIP комісія (0%). Можемо безпечно підміняти стратегії завдяки LSP.
    public class VipFeeStrategy : ITransactionFeeStrategy
    {
        public decimal CalculateFee(decimal amount) => 0m;
    }

    // DIP (Dependency Inversion): TransactionProcessor залежить від АБСТРАКЦІЙ (ILogger, ITransactionFeeStrategy),
    // а не від конкретних реалізацій (ConsoleLogger чи VipFeeStrategy).
    public class TransactionProcessor
    {
        private readonly ILogger _logger;
        private readonly ITransactionFeeStrategy _feeStrategy;

        // Залежності інжектяться через конструктор
        public TransactionProcessor(ILogger logger, ITransactionFeeStrategy feeStrategy)
        {
            _logger = logger;
            _feeStrategy = feeStrategy;
        }

        public void ProcessWithdrawal(Account account, decimal amount)
        {
            try
            {
                decimal fee = _feeStrategy.CalculateFee(amount);
                decimal totalDeduction = amount + fee;

                // Виконуємо бізнес-логіку
                account.WithdrawMoney(totalDeduction);
                
                // Делегуємо вивід іншому сервісу
                _logger.LogInfo($"Успішно знято {amount} UAH (Комісія: {fee} UAH). Залишок на рахунку {account.AccountNumber}: {account.Balance} UAH.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Транзакція відхилена: {ex.Message}");
            }
        }
    }
}