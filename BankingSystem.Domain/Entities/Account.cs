using System;
using System.Collections.Generic;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Domain.Entities
{
    /// <summary>
    /// Абстрактний базовий клас для всіх типів банківських рахунків.
    /// Забезпечує спільну функціональність: управління балансом, історія транзакцій, сповіщення.
    /// Реалізує IDisposable для коректної очистки ресурсів.
    /// 
    /// Паттерни:
    /// - Наслідування (Практична 3): Base class для CheckingAccount, Deposit
    /// - Поліморфізм (Практична 3): Virtual методи DepositMoney, WithdrawMoney
    /// - Инкапсуляция (Практична 2): Private поля з властивостями-валідаторами
    /// - Контрактне програмування (Практична 4): INotificationService для слабкого зв'язку
    /// </summary>
    public abstract class Account : IDisposable
    {
        private bool _disposed = false;
        private decimal _balance;
        private string _ownerName = "Unknown";
        private readonly List<Transaction> _transactions = new List<Transaction>();

        /// <summary>Унікальний номер рахунку у форматі "UA + 8 символів GUID".</summary>
        public string AccountNumber { get; private set; }
        
        /// <summary>
        /// Сервіс сповіщень для інформування власника про операції.
        /// Реалізує паттерн Dependency Injection для слабкого зв'язку.
        /// </summary>
        public INotificationService? Notifier { get; set; }

        /// <summary>
        /// Ім'я власника рахунку. Валідується: не може бути порожнім або null.
        /// </summary>
        /// <exception cref="ArgumentException">Якщо значення порожнє або null</exception>
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

        /// <summary>
        /// Поточний баланс рахунку. Ніколи не може бути від'ємним (овердрафт заборонено).
        /// Зміни балансу відбуваються тільки через методи DepositMoney та WithdrawMoney.
        /// </summary>
        /// <exception cref="InvalidOperationException">Якщо спроба встановити від'ємне значення</exception>
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

        /// <summary>
        /// Індексатор для доступу до історії транзакцій за індексом.
        /// Дозволяє застосовувати синтаксис account[0] для отримання першої транзакції.
        /// </summary>
        /// <param name="index">Індекс транзакції в історії (0-based)</param>
        /// <returns>Об'єкт Transaction за запитаним індексом</returns>
        /// <exception cref="IndexOutOfRangeException">Якщо індекс поза межами історії</exception>
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
        // КОНСТРУКТОРИ І ЖИТТЄВИЙ ЦИКЛ (Практична 1 / Самостійна 1)
        // =========================================================
        
        /// <summary>
        /// Конструктор за замовчуванням. Ініціалізує випадковий номер рахунку та нульовий баланс.
        /// Protected, оскільки клас абстрактний.
        /// </summary>
        protected Account()
        {
            AccountNumber = "UA" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            _balance = 0.0m;
        }

        /// <summary>
        /// Конструктор з параметрами.
        /// </summary>
        /// <param name="ownerName">Ім'я власника рахунку</param>
        /// <param name="initialBalance">Початковий баланс</param>
        protected Account(string ownerName, decimal initialBalance) : this()
        {
            OwnerName = ownerName; 
            Balance = initialBalance; 
        }

        /// <summary>
        /// Конструктор копіювання (Copy Constructor).
        /// Створює новий рахунок, копіюючи дані з існуючого (Самостійна 1).
        /// </summary>
        /// <param name="other">Рахунок для копіювання</param>
        /// <exception cref="ArgumentNullException">Якщо other є null</exception>
        protected Account(Account other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            
            AccountNumber = other.AccountNumber + "-COPY";
            _balance = other._balance;
            _ownerName = other._ownerName;
        }

        // =========================================================
        // МЕТОДИ БІЗНЕС-ЛОГІКИ ТА ПОЛІМОРФІЗМУ (Практична 3, 4)
        // =========================================================

        /// <summary>
        /// Поповнює рахунок на вказану суму.
        /// Додає запис до історії транзакцій та відправляє сповіщення власнику.
        /// </summary>
        /// <param name="amount">Сума для поповнення (має бути > 0)</param>
        /// <exception cref="ArgumentException">Якщо сума <= 0</exception>
        public virtual void DepositMoney(decimal amount)
        {
            if (amount <= 0) 
                throw new ArgumentException("Сума поповнення має бути більшою за нуль!");
            
            Balance += amount;
            _transactions.Add(new Transaction(amount, "Deposit"));

            // СР 4: Виклик методу через інтерфейс контракту (якщо підключено)
            Notifier?.SendNotification($"Рахунок {AccountNumber} поповнено на +{amount} UAH. Баланс: {Balance} UAH.");
        }

        /// <summary>
        /// Знімає кошти з рахунку на вказану суму.
        /// Додає запис до історії транзакцій та відправляє сповіщення власнику.
        /// </summary>
        /// <param name="amount">Сума для зняття (має бути > 0 і <= поточному балансу)</param>
        /// <exception cref="ArgumentException">Якщо сума <= 0</exception>
        /// <exception cref="InvalidOperationException">Якщо сума > поточного балансу</exception>
        public virtual void WithdrawMoney(decimal amount)
        {
            if (amount <= 0) 
                throw new ArgumentException("Сума зняття має бути більшою за нуль!");
            
            Balance -= amount; 
            _transactions.Add(new Transaction(amount, "Withdraw"));

            // СР 4: Виклик методу через інтерфейс контракту (якщо підключено)
            Notifier?.SendNotification($"З рахунку {AccountNumber} знято -{amount} UAH. Баланс: {Balance} UAH.");
        }

        /// <summary>
        /// Повертає детальну інформацію про рахунок.
        /// Метод віртуальний — переозначується в дочірніх класах (Практична 3).
        /// </summary>
        /// <returns>Строка з типом та номером рахунку</returns>
        public virtual string GetAccountDetails() => $"[Базовий рахунок] № {AccountNumber}";

        /// <summary>
        /// Повертає умови використання рахунку.
        /// </summary>
        /// <returns>Строка з основними умовами</returns>

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