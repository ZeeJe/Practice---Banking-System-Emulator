using System;
using BankingSystem.Domain.Entities;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДЕМОНСТРАЦІЯ ІНКАПСУЛЯЦІЇ ТА АГРЕГАЦІЇ ===\n");

        try
        {
            // 1. Створення рахунку
            Account myAccount = new Account("Олександр", 1000.00m);
            Console.WriteLine($"Рахунок створено. Власник: {myAccount.OwnerName}, Баланс: {myAccount.Balance} UAH");

            // 2. Тест перевантаження операторів (+ та -)
            Console.WriteLine("\n--- Тестування операторів + та - ---");
            myAccount = myAccount + 500.00m; // Використання оператора +
            Console.WriteLine($"Після поповнення (+ 500): {myAccount.Balance} UAH");

            myAccount = myAccount - 300.00m; // Використання оператора -
            Console.WriteLine($"Після зняття (- 300): {myAccount.Balance} UAH");

            // 3. Тест індексатора (Перегляд історії транзакцій)
            Console.WriteLine("\n--- Тестування індексатора (Історія транзакцій) ---");
            Console.WriteLine($"Всього транзакцій: {myAccount.TransactionsCount}");
            Console.WriteLine($"Перша транзакція через індексатор [0]: {myAccount[0]}");
            Console.WriteLine($"Друга транзакція через індексатор [1]: {myAccount[1]}");

            // 4. Тест валідації інваріантів (Захист даних від зламу)
            Console.WriteLine("\n--- Тестування захисту інкапсуляції (Спроба зняти забагато) ---");
            myAccount = myAccount - 2000.00m; // Очікуємо помилку, бо баланс стане менше 0
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Помилка валідації]: {ex.Message}");
            Console.ResetColor();
        }

        Console.ReadLine();
    }
}