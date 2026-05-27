using System;
using BankingSystem.Domain.Entities;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: OVERRIDE vs NEW (ПОЛІМОРФІЗМ) ===\n");

        // Створюємо депозит на ім'я Анни (закінчується через 1 рік)
        Deposit myDeposit = new Deposit("Анна", 10000.00m, DateTime.Now.AddYears(1), 15.0m);
        
        // Upcasting: Зберігаємо депозит у змінну базового типу Account
        // (Банк часто зберігає всі типи рахунків в одному масиві Account[])
        Account baseReference = myDeposit;

        // ----------------------------------------------------
        // 1. ТЕСТ OVERRIDE (Правильний поліморфізм)
        // ----------------------------------------------------
        Console.WriteLine("--- 1. Виклик методу GetAccountDetails() (використано override) ---");
        Console.WriteLine("Через змінну Deposit: " + myDeposit.GetAccountDetails());
        Console.WriteLine("Через змінну Account: " + baseReference.GetAccountDetails());
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("ВИСНОВОК: override працює ідеально. Програма розуміє, що всередині лежить Депозит, і викликає правильний метод.\n");
        Console.ResetColor();

        // ----------------------------------------------------
        // 2. ТЕСТ NEW (Приховування методів)
        // ----------------------------------------------------
        Console.WriteLine("--- 2. Виклик методу GetTerms() (використано new) ---");
        Console.WriteLine("Через змінну Deposit: " + myDeposit.GetTerms());
        Console.WriteLine("Через змінну Account: " + baseReference.GetTerms());
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("ВИСНОВОК: new ЛАМАЄ поліморфізм! Змінна типу Account викликала старий батьківський метод, проігнорувавши, що це Депозит.\n");
        Console.ResetColor();

        // ----------------------------------------------------
        // 3. ТЕСТ BASE та РОЗШИРЕННЯ ПОВЕДІНКИ
        // ----------------------------------------------------
        Console.WriteLine("--- 3. Спроба достроково зняти кошти (використано override + base) ---");
        try
        {
            baseReference.WithdrawMoney(2000.00m); // Спрацює override метод депозиту
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Захист депозиту]: {ex.Message}");
            Console.ResetColor();
        }

        Console.ReadLine();
    }
}