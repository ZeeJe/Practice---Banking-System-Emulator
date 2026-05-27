using System;
using System.Collections.Generic;
using System.Linq;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Extensions;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: LINQ ТА МЕТОДИ РОЗШИРЕННЯ (ЛР 7 / СР 7) ===\n");

        // 1. Підготовка даних (рахунки банку)
        var accounts = new List<Account>
        {
            new CheckingAccount("Олександр", 1500m),
            new CheckingAccount("Марія", 8000m),
            new Deposit("Олександр", 20000m, DateTime.Now.AddYears(1), 15m),
            new Deposit("Іван", 300m, DateTime.Now.AddMonths(6), 10m)
        };

        // Імітація зовнішньої бази даних клієнтів (для демонстрації Join)
        var customersInfo = new List<dynamic> 
        { 
            new { Name = "Олександр", City = "Київ" },
            new { Name = "Марія", City = "Львів" },
            new { Name = "Іван", City = "Одеса" }
        };

        // =======================================================
        // ПРАКТИЧНА 7: Method Syntax vs Query Syntax
        // =======================================================
        Console.WriteLine("--- 1. Фільтрація та Проєкція (Select & Where) ---");
        
        // Method Syntax (через крапку і лямбда-вирази)
        var methodSyntaxResult = accounts
            .Where(a => a.Balance > 1000)
            .Select(a => a.OwnerName)
            .Distinct();

        // Query Syntax (схоже на SQL)
        var querySyntaxResult = (from a in accounts
                                 where a.Balance > 1000
                                 select a.OwnerName).Distinct();

        Console.WriteLine($"Method Syntax (Баланс > 1000): {string.Join(", ", methodSyntaxResult)}");
        Console.WriteLine($"Query Syntax  (Баланс > 1000): {string.Join(", ", querySyntaxResult)}\n");


        // =======================================================
        // САМОСТІЙНА 7: Складні запити (GroupBy, Join)
        // =======================================================
        Console.WriteLine("--- 2. Складні запити (GroupBy та Join) ---");

        // GroupBy: Групуємо рахунки за їх типом (Deposit або CheckingAccount)
        var groupedByType = accounts.GroupBy(a => a.GetType().Name);
        Console.WriteLine("[GroupBy Результат]:");
        foreach (var group in groupedByType)
        {
            Console.WriteLine($"Тип рахунку: {group.Key} | Кількість: {group.Count()} шт.");
        }

        // Join: Об'єднуємо список рахунків зі списком міст за ім'ям власника
        var joinedData = accounts.Join(
            customersInfo,
            acc => acc.OwnerName,   // Ключ з колекції рахунків
            cust => cust.Name,      // Ключ з колекції клієнтів
            (acc, cust) => new { acc.OwnerName, acc.Balance, cust.City } // Проєкція результату
        );
        
        Console.WriteLine("\n[Join Результат (Рахунки + Міста)]:");
        foreach (var item in joinedData)
        {
            Console.WriteLine($"{item.OwnerName} (м. {item.City}) має баланс {item.Balance} UAH");
        }


        // =======================================================
        // САМОСТІЙНА 7: Використання методів розширення (Extensions)
        // =======================================================
        Console.WriteLine("\n--- 3. Кастомні Extension Methods та Aggregate ---");
        
        // Викликаємо наші власні методи так, ніби вони є стандартними методами List
        var vipAccounts = accounts.GetVipAccounts(5000m);
        Console.WriteLine($"VIP Клієнти (> 5000 UAH): {string.Join(", ", vipAccounts.Select(a => a.OwnerName).Distinct())}");

        decimal totalLiquidity = accounts.CalculateTotalBalance();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Загальний капітал банку (через кастомний Aggregate): {totalLiquidity} UAH");
        Console.ResetColor();

        Console.ReadLine();
    }
}