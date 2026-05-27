using System;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Factories;
using BankingSystem.Domain.Services;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: ПОРОДЖУВАЛЬНІ ПАТЕРНИ (ЛР 10 / СР 10) ===\n");

        // =======================================================
        // ПРАКТИЧНА 10: Singleton
        // =======================================================
        Console.WriteLine("--- 1. Тестування патерну Singleton ---");
        
        var bank1 = CentralBank.Instance;
        Console.WriteLine($"Змінна bank1: {bank1.BankName}, Базова ставка: {bank1.BaseInterestRate}%");
        
        var bank2 = CentralBank.Instance;
        // Змінюємо ставку через другу змінну
        bank2.UpdateInterestRate(15.0m); 
        
        Console.WriteLine($"Змінна bank1 (після оновлення через bank2): {bank1.BaseInterestRate}%");
        
        // Перевіряємо, чи це дійсно одне й те саме місце в пам'яті
        bool isSameInstance = ReferenceEquals(bank1, bank2);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Чи bank1 і bank2 - це один і той самий об'єкт в пам'яті? {isSameInstance}\n");
        Console.ResetColor();

        // =======================================================
        // САМОСТІЙНА 10: Factory Method
        // =======================================================
        Console.WriteLine("--- 2. Тестування Factory Method (Динамічне створення) ---");
        
        // Імітуємо конфігурацію, яку програма отримала з JSON файлу або CLI
        string[] configParams = { "checking", "deposit", "crypto_wallet" };

        foreach (var type in configParams)
        {
            try
            {
                Console.WriteLine($"[Config Parser] Запит на створення типу: '{type}'...");
                
                // Фабрика створює об'єкт, ми навіть не викликаємо 'new' безпосередньо!
                Account newAccount = AccountFactory.CreateAccount(type, "Ілон Маск", 1000m);
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"-> Успіх! Створено {newAccount.GetType().Name} для {newAccount.OwnerName}.");
                
                // Якщо це депозит, перевіримо, чи підтягнулась ставка з Singleton
                if (newAccount is Deposit dep)
                {
                    Console.WriteLine($"   Ставка депозиту (із Singleton): {dep.GetAccountDetails()}");
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-> {ex.Message}\n");
                Console.ResetColor();
            }
        }

        Console.ReadLine();
    }
}