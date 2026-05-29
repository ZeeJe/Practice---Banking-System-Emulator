using System;
using BankingSystem.Domain.Common;

namespace BankingSystem.Domain.Services
{
    // Клас позначено як sealed, щоб уникнути наслідування, яке може зламати Singleton
    public sealed class CentralBank
    {
        // Лінива та потокобезпечна ініціалізація єдиного екземпляра
        private static readonly Lazy<CentralBank> _instance = new Lazy<CentralBank>(() => new CentralBank());

        public static CentralBank Instance => _instance.Value;

        public string BankName { get; private set; }
        public decimal BaseInterestRate { get; private set; }

        // Приватний конструктор: об'єкт неможливо створити через 'new' зовні
        private CentralBank()
        {
            BankName = "Національний Банк (Singleton)";
            BaseInterestRate = BankConstants.DefaultNationalBankRate; // Використовуємо константу
            Console.WriteLine("[Singleton]: Екземпляр CentralBank створено в пам'яті.");
        }

        public void UpdateInterestRate(decimal newRate)
        {
            BaseInterestRate = newRate;
        }
    }
}