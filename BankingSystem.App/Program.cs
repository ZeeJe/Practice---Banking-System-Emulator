using System;
using BankingSystem.App;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        // Запускаємо новий інтерактивний додаток
        var bankingApp = new BankingApp();
        bankingApp.Run();

        Console.ReadLine();
    }
}