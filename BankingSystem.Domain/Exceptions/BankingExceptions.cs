using System;

namespace BankingSystem.Domain.Exceptions
{
    // 1. Базовий виняток для всієї доменної області банку
    public class BankingException : Exception
    {
        public BankingException(string message) : base(message) { }
        public BankingException(string message, Exception innerException) : base(message, innerException) { }
    }

    // 2. Специфічний бізнес-виняток: Недостатньо коштів на рахунку
    public class InsufficientFundsException : BankingException
    {
        public decimal AttemptedAmount { get; }
        public decimal CurrentBalance { get; }

        public InsufficientFundsException(string message, decimal attemptedAmount, decimal currentBalance) 
            : base(message)
        {
            AttemptedAmount = attemptedAmount;
            CurrentBalance = currentBalance;
        }
    }

    // 3. Інфраструктурний виняток: Тимчасовий збій мережі / таймаут (Transient Error)
    // Він потрібен нам для тестування механізму Retry Policy
    public class NetworkGlitchException : BankingException
    {
        public NetworkGlitchException(string message) : base(message) { }
    }
}