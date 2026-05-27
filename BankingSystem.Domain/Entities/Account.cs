using System;
using System.Collections.Generic;

namespace BankingSystem.Domain.Entities
{
    public class Account : IDisposable
    {
        private bool _disposed = false;
        
        // Приватні поля для інкапсуляції (Лабораторна 2)
        private decimal _balance;
        private string _ownerName = "Unknown";
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public string AccountNumber { get; private set; }

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

        // Баланс із захистом від прямого редагування (Лабораторна 2)
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

        // Індексатор для доступу до історії транзакцій (Самостійна 2)
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
        // КОНСТРУКТОРІВ ТА ЖИТТЄВИЙ ЦИКЛ (Лабораторна 1 / СР 1)
        // =========================================================

        // 1. Основний конструктор
        public Account()
        {
            AccountNumber = "UA" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            _balance = 0.0m;
        }

        // 2. Конструктор з параметрами
        public Account(string ownerName, decimal initialBalance) : this()
        {
            OwnerName = ownerName; 
            Balance = initialBalance; 
        }

        // 3. Копіювальний конструктор (Виправлено warning про null)
        public Account(Account other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            
            AccountNumber = other.AccountNumber + "-COPY";
            _balance = other._balance;
            _ownerName = other._ownerName;
        }

        // =========================================================
        // МЕТОДИ БІЗНЕС-ЛОГІКИ ТА ПОЛІМОРФІЗМУ (Лабораторна 3)
        // =========================================================

        // Зроблено virtual, щоб дочірні класи (наприклад, Deposit) могли змінювати поведінку
        public virtual void DepositMoney(decimal amount)
        {
            if (amount <= 0) 
                throw new ArgumentException("Сума поповнення має бути більшою за нуль!");
            
            Balance += amount;
            _transactions.Add(new Transaction(amount, "Deposit"));
        }

        public virtual void WithdrawMoney(decimal amount)
        {
            if (amount <= 0) 
                throw new ArgumentException("Сума зняття має бути більшою за нуль!");
            
            Balance -= amount; 
            _transactions.Add(new Transaction(amount, "Withdraw"));
        }

        // Метод для дослідження override (Самостійна 3)
        public virtual string GetAccountDetails() => $"[Базовий рахунок] № {AccountNumber}";

        // Метод для дослідження приховування new (Самостійна 3)
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

        // Виправлено warning CS8765 за допомогою object?
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
                if (disposing) 
                {
                    // Очищення керованих ресурсів (якщо будуть у майбутньому)
                }
                _disposed = true;
            }
        }

        ~Account()
        {
            Dispose(false);
        }
    }
}