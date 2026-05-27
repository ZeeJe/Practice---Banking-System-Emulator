using System;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Interfaces;
using BankingSystem.Domain.Services;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: ПРИНЦИПИ SOLID (ЛР 9 / СР 9) ===\n");

        // 1. Створюємо інфраструктурні залежності (можемо легко замінити ConsoleLogger на FileLogger)
        ILogger logger = new ConsoleLogger();

        // 2. Створюємо двох різних клієнтів
        var standardAccount = new CheckingAccount("Олександр (Стандарт)", 2000m);
        var vipAccount = new CheckingAccount("Марія (VIP)", 50000m);

        // =======================================================
        // ТЕСТ 1: Клієнт зі стандартною комісією
        // =======================================================
        Console.WriteLine("--- 1. Обробка зі стандартною стратегією (1% комісії) ---");
        ITransactionFeeStrategy standardStrategy = new StandardFeeStrategy();
        
        // Впроваджуємо залежності (DIP)
        var standardProcessor = new TransactionProcessor(logger, standardStrategy);
        
        // Знімаємо 1000 UAH (має зняти 1000 + 10 UAH комісії)
        standardProcessor.ProcessWithdrawal(standardAccount, 1000m);

        Console.WriteLine();

        // =======================================================
        // ТЕСТ 2: Клієнт із VIP комісією
        // =======================================================
        Console.WriteLine("--- 2. Обробка з VIP стратегією (0% комісії) ---");
        ITransactionFeeStrategy vipStrategy = new VipFeeStrategy();
        
        // LSP: Ми підмінили стратегію, і TransactionProcessor продовжує працювати без змін коду!
        var vipProcessor = new TransactionProcessor(logger, vipStrategy);
        
        // Знімаємо 5000 UAH (без комісії)
        vipProcessor.ProcessWithdrawal(vipAccount, 5000m);

        Console.WriteLine();

        // =======================================================
        // ТЕСТ 3: Перевірка захисту (LSP + Exception Handling)
        // =======================================================
        Console.WriteLine("--- 3. Перевірка відмови транзакції (Брак коштів) ---");
        // Намагаємося зняти більше, ніж є
        standardProcessor.ProcessWithdrawal(standardAccount, 5000m);

        Console.ReadLine();
    }
}