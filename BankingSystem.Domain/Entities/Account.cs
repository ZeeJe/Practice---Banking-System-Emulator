using System;
using System.Collections.Generic;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Domain.Entities
{
    // ПР 4: Клас став абстрактним (забезпечує спільну поведінку для всіх типів рахунків)
    public abstract class Account : IDisposable
    {
        private bool _disposed = false;
        private decimal _balance;
        private string _ownerName = "Unknown";
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public string AccountNumber { get; private set; }
        
        // СР 4: Слабке зв'язування з системою сповіщень через інтерфейс контракту
        public INotificationService? Notifier { get; set; }

        // Властивість із валідацією стану (Лабораторна 2)
        public string OwnerName
        {
            get => _ownerName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ім'я власника рахунку не може бути порожнім!");
                _ownerName = value;
            }
        }

        // Баланс із захистом від прямого зовнішнього редагування (Лабораторна 2)
        public decimal Balance
        {
            get => _balance;
            private set
            {
                if (value < 0)
                    throw new InvalidOperationException("Баланс рахунку не може бути від'ємним (овердрафт не дозволено)!");
                _balance = value;
            }
        }

        // Індексатор для доступу до історії транзакцій за індексом (Самостійна 2)
        public Transaction this[int index]
        {
            get
            {
                if (index < 0 || index >= _transactions.Count)
                    throw new IndexOutOfRangeException("Транзакції за таким індексом не існує.");
                return _transactions[index];
            }
        }

        public int TransactionsCount => _transactions.Count;

        // =========================================================
        // КОНСТРУКТОРЫ И ЖИЗНЕННЫЙ ЦИКЛ (Лабораторна 1 / СР 1)
        // Конструктори абстрактного класу робимо protected
        // =========================================================

        protected Account()
        {
            AccountNumber = "UA" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            _balance = 0.0m;
        }

        protected Account(string ownerName, decimal initialBalance) : this()
        {
            OwnerName = ownerName; 
            Balance = initialBalance; 
        }

        protected Account(Account other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            
            AccountNumber = other.AccountNumber + "-COPY";
            _balance = other._balance;
            _ownerName = other._ownerName;
        }

        // =========================================================
        // МЕТОДИ БІЗНЕС-ЛОГІКИ ТА ПОЛІМОРФІЗМУ (Лабораторна 3, 4)
        // =========================================================

        public virtual void DepositMoney(decimal amount)
        {
            if (amount <= 0) 
                throw new ArgumentException("Сума поповнення має бути більшою за нуль!");
            
            Balance += amount;
            _transactions.Add(new Transaction(amount, "Deposit"));

            // СР 4: Виклик методу через інтерфейс контракту (якщо підключено)
            Notifier?.SendNotification($"Рахунок {AccountNumber} поповнено на +{amount} UAH. Баланс: {Balance} UAH.");
        }

        public virtual void WithdrawMoney(decimal amount)
        {
            if (amount <= 0) 
                throw new ArgumentException("Сума зняття має бути більшою за нуль!");
            
            Balance -= amount; 
            _transactions.Add(new Transaction(amount, "Withdraw"));

            // СР 4: Виклик методу через інтерфейс контракту (якщо підключено)
            Notifier?.SendNotification($"З рахунку {AccountNumber} знято -{amount} UAH. Баланс: {Balance} UAH.");
        }

        public virtual string GetAccountDetails() => $"[Базовий рахунок] № {AccountNumber}";

        public string GetTerms() => $"[Базові умови] Зняття коштів можливе в будь-який момент.";

        // =========================================================
        // ПЕРЕВАНТАЖЕННЯ ОПЕРАТОРІВ (Самостійна 2)
        // =========================================================

        public static Account operator +(Account account, decimal amount)
        {
            account.DepositMoney(amount);
            return account;
        }

        public static Account operator -(Account account, decimal amount)
        {
            account.WithdrawMoney(amount);
            return account;
        }

        public static bool operator ==(Account? left, Account? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.AccountNumber == right.AccountNumber;
        }

        public static bool operator !=(Account? left, Account? right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is Account account && this == account;
        }

        public override int GetHashCode()
        {
            return AccountNumber.GetHashCode();
        }

        // =========================================================
        // ОЧИЩЕННЯ РЕСУРСІВ (IDisposable / Деструктор) (СР 1)
        // =========================================================

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) { }
                _disposed = true;
            }
        }

        ~Account()
        {
            Dispose(false);
        }
    }
}