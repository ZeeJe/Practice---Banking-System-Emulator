using System;
using System.Collections.Generic;

namespace BankingSystem.Domain.Entities
{
    public class Account : IDisposable
    {
        private bool _disposed = false;
        
        // Приватні поля для інкапсуляції та захисту інваріантів
        private decimal _balance;
        private string _ownerName;
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public string AccountNumber { get; private set; }

        // Властивість із валідацією (Інкапсуляція)
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

        // Баланс можна змінювати лише через спеціальні методи або оператори
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

        // Індексатор для агрегації транзакцій (Вимога СР 2)
        public Transaction this[int index]
        {
            get
            {
                if (index < 0 || index >= _transactions.Count)
                    throw new IndexOutOfRangeException("Транзакції за таким індексом не існує.");
                return _transactions[index];
            }
        }

        // Кількість транзакцій для зручності перевірки
        public int TransactionsCount => _transactions.Count;

        // Конструктори (збережені з першого заняття)
        public Account()
        {
            AccountNumber = "UA" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            _balance = 0.0m;
            _ownerName = "Unknown";
        }

        public Account(string ownerName, decimal initialBalance) : this()
        {
            OwnerName = ownerName; // Спрацює валідація сетера
            Balance = initialBalance; // Спрацює валідація сетера
        }

        public Account(Account other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            AccountNumber = other.AccountNumber + "-COPY";
            _balance = other._balance;
            _ownerName = other._ownerName;
        }

        // Бізнес-методи, що гарантують консистентність даних
        public void DepositMoney(decimal amount)
        {
            if (amount <= 0) 
                throw new ArgumentException("Сума поповнення має бути більшою за нуль!");
            
            Balance += amount;
            _transactions.Add(new Transaction(amount, "Deposit"));
        }

        public void WithdrawMoney(decimal amount)
        {
            if (amount <= 0) 
                throw new ArgumentException("Сума зняття має бути більшою за нуль!");
            
            Balance -= amount; // Перевірка на від'ємний баланс виконається всередині властивості Balance
            _transactions.Add(new Transaction(amount, "Withdraw"));
        }

        // =========================================================
        // ПЕРЕВАНТАЖЕННЯ ОПЕРАТОРІВ (Вимога СР 2)
        // =========================================================

        // Оператор + дозволяє поповнювати рахунок прямо кодом: account + 500
        public static Account operator +(Account account, decimal amount)
        {
            account.DepositMoney(amount);
            return account;
        }

        // Оператор - дозволяє знімати кошти прямо кодом: account - 200
        public static Account operator -(Account account, decimal amount)
        {
            account.WithdrawMoney(amount);
            return account;
        }

        // Оператори порівняння рахунків за їхнім номером
        public static bool operator ==(Account left, Account right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.AccountNumber == right.AccountNumber;
        }

        public static bool operator !=(Account left, Account right)
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

        // Реалізація IDisposable (збережено)
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
    }
}