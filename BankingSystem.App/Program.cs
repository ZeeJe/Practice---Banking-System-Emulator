using System;
using BankingSystem.Domain.Facades;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: СТРУКТУРНІ ПАТЕРНИ (ЛР 12 / СР 12) ===\n");

        // Фасад бере на себе взаємодію зі складними підсистемами
        var bankSystem = new BankFacade();

        // Тестуємо Компонувальник (Composite)
        bankSystem.ShowClientPortfolio();

        // Тестуємо Декоратор (Decorator)
        bankSystem.IssuePremiumCard();

        Console.ReadLine();
    }
}