using System;

namespace BankingSystem.Domain.Services
{
    // Дали унікальну назву, щоб не ламати твої Email та Sms сервіси
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
            if (amount <= 0)
                throw new ArgumentException("Сума має бути більшою за нуль.");

            if (amount > Balance)
                throw new InvalidOperationException("Недостатньо коштів.");

            Balance -= amount;
            _notifier.SendSms($"Успішно знято {amount} UAH. Залишок: {Balance} UAH");
        }
    }
}