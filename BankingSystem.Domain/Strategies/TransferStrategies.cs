using System;

namespace BankingSystem.Domain.Strategies
{
    // Спільний інтерфейс для всіх алгоритмів переказу
    public interface ITransferStrategy
    {
        decimal CalculateFee(decimal amount);
        string GetTransferType();
    }

    // Алгоритм 1: Внутрішній переказ (Без комісії)
    public class LocalTransferStrategy : ITransferStrategy
    {
        public decimal CalculateFee(decimal amount) => 0m;
        public string GetTransferType() => "Внутрішній переказ";
    }

    // Алгоритм 2: Міжнародний SWIFT-переказ (5% + 50 UAH фіксовано)
    public class SwiftTransferStrategy : ITransferStrategy
    {
        public decimal CalculateFee(decimal amount) => (amount * 0.05m) + 50m;
        public string GetTransferType() => "Міжнародний SWIFT-переказ";
    }
}