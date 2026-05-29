using System;
using BankingSystem.Domain.Common;

namespace BankingSystem.Domain.Services
{
    public interface IWithdrawalNotificationService
    {
        bool SendSms(string message);
    }

    public class WithdrawalService
    {
        private readonly IWithdrawalNotificationService _notifier;
        public decimal Balance { get; private set; }

        public WithdrawalService(IWithdrawalNotificationService notifier, decimal initialBalance)
        {
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            Balance = initialBalance;
        }

        public void Withdraw(decimal amount)
        {
            // Перевірка через глобальну константу ліміту
            if (amount < BankConstants.MinWithdrawalAmount)
                throw new ArgumentException($"Сума має бути не меншою за {BankConstants.MinWithdrawalAmount} UAH.");

            if (amount > Balance)
                throw new InvalidOperationException("Недостатньо коштів.");

            Balance -= amount;
            _notifier.SendSms($"Успішно знято {amount} UAH. Залишок: {Balance} UAH");
        }
    }
}