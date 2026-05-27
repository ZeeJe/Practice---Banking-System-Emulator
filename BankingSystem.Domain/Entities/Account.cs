using System;

namespace BankingSystem.Domain.Entities
{
    public class Account : IDisposable
    {
        private bool _disposed = false;

        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string OwnerName { get; set; }

        // 1. Основний конструктор (без параметрів / за замовчуванням)
        public Account()
        {
            AccountNumber = "UA" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            Balance = 0.0m;
            OwnerName = "Unknown";
            Console.WriteLine($"[Життєвий цикл] Створено порожній рахунок: {AccountNumber}");
        }

        // 2. Конструктор з параметрами
        public Account(string ownerName, decimal initialBalance) : this()
        {
            OwnerName = ownerName;
            Balance = initialBalance;
            Console.WriteLine($"[Життєвий цикл] Ініціалізовано рахунок для: {OwnerName}, Баланс: {Balance}");
        }

        // 3. Копіювальний конструктор (Спеціальна вимога самостійної роботи!)
        public Account(Account other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            
            // Копіюємо дані з іншого об'єкта
            this.AccountNumber = other.AccountNumber + "-COPY";
            this.Balance = other.Balance;
            this.OwnerName = other.OwnerName;
            Console.WriteLine($"[Життєвий цикл] Створено копію рахунку {other.AccountNumber} -> {this.AccountNumber}");
        }

        // Базові методи бізнес-логіки
        public virtual void DepositMoney(decimal amount) => Balance += amount;
        public virtual void WithdrawMoney(decimal amount) => Balance -= amount;

        // 4. Реалізація життєвого циклу через IDisposable (Очищення ресурсів)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Кажемо збирачу сміття, що деструктор викликати вже не треба
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Тут у майбутньому буде відписка від подій (патерн Observer) 
                    // або закриття потоків збереження даних
                    Console.WriteLine($"[IDisposable] Керовані ресурси рахунку {AccountNumber} очищено.");
                }
                
                _disposed = true;
            }
        }

        // 5. Деструктор (Фіналізатор)
        ~Account()
        {
            Console.WriteLine($"[Деструктор] Об'єкт рахунку {AccountNumber} видалено з пам'яті збирачем сміття (GC).");
            Dispose(false);
        }
    }
}