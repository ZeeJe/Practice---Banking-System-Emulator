using System;
using System.Collections.Generic;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Repositories;
using BankingSystem.Domain.Services;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: GENERICS ТА ФУНКЦІОНАЛЬНІ ДЕЛЕГАТИ ===\n");

        // 1. Ініціалізація узагальненого репозиторію для типів Account
        AccountRepository<Account> bankStorage = new AccountRepository<Account>();

        // Створюємо різні типи рахунків
        var checking = new CheckingAccount("Олександр", 2500.00m);
        var deposit = new Deposit("Марія", 10000.00m, DateTime.Now.AddYears(1), 16.5m);

        bankStorage.Add(checking);
        bankStorage.Add(deposit);

        // ----------------------------------------------------
        // ТЕСТ 1: ForEach (Делегат Action)
        // ----------------------------------------------------
        Console.WriteLine("--- 1. Демонстрація роботи ForEach (Друк деталей усіх рахунків) ---");
        // Передаємо лямбду, яка підходить під Action<Account>
        bankStorage.ForEach(acc => Console.WriteLine(acc.GetAccountDetails()));
        Console.WriteLine();

        // ----------------------------------------------------
        // ТЕСТ 2: Map (Делегат Func<T, TResult>)
        // ----------------------------------------------------
        Console.WriteLine("--- 2. Демонстрація роботи Map (Трансформація: витягуємо лише номери рахунків) ---");
        // Трансформуємо список об'єктів Account у список строк string
        List<string> accountNumbers = bankStorage.Map(acc => acc.AccountNumber);
        
        foreach (var num in accountNumbers)
        {
            Console.WriteLine($"Знайдено номер рахунку: {num}");
        }
        Console.WriteLine();

        // ----------------------------------------------------
        // ТЕСТ 3: Reduce (Делегат Func<TAccumulate, T, TAccumulate>)
        // ----------------------------------------------------
        Console.WriteLine("--- 3. Демонстрація роботи Reduce (Агрегація: підрахунок загального капіталу банку) ---");
        // Рахуємо суму балансів усіх рахунків, стартуючи з 0.0m
        decimal totalLiquidity = bankStorage.Reduce(0.0m, (currentSum, acc) => currentSum + acc.Balance);
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Успішно агреговано! Загальний баланс усіх рахунків у банку: {totalLiquidity:N2} UAH");
        Console.ResetColor();

        Console.ReadLine();
    }
}