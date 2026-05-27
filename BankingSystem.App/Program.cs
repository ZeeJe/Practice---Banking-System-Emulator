using System;
using BankingSystem.Domain.Entities;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДЕМОНСТРАЦІЯ ЖИТТЄВОГО ЦИКЛУ ОБ'ЄКТА ===\n");

        // 1. Створення через конструктор з параметрами всередині using (Dispose викличеться автоматично)
        using (Account acc1 = new Account("Іван Іваненко", 1500.00m))
        {
            // 2. Створення копії через копіювальний конструктор
            Account accCopy = new Account(acc1);
            Console.WriteLine($"Копія має баланс: {accCopy.Balance}");
        } // Тут для acc1 автоматично спрацює Dispose()

        Console.WriteLine("\nБлок using завершився.");
        Console.ReadLine();
    }
}