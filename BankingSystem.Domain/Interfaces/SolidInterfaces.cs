namespace BankingSystem.Domain.Interfaces
{
    // 1. ISP (Interface Segregation): Інтерфейс тільки для логування/відображення.
    public interface ILogger
    {
        void LogInfo(string message);
        void LogError(string message);
    }

    // 2. OCP (Open/Closed): Інтерфейс стратегії для розрахунку комісії. 
    // Ми зможемо створювати нові комісії, не змінюючи старий код.
    public interface ITransactionFeeStrategy
    {
        decimal CalculateFee(decimal amount);
    }
}