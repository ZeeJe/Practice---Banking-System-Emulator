using System;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Services;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: АБСТРАКТНІ КЛАСИ ТА ІНТЕРФЕЙСИ ===\n");

        // 1. Створюємо конкретний рахунок (бо Account тепер абстрактний і його створити не можна)
        CheckingAccount myAccount = new CheckingAccount("Олександр", 1000.00m);
        Console.WriteLine(myAccount.GetAccountDetails());

        // 2. Підключаємо першу імплементацію інтерфейсу (SMS)
        Console.WriteLine("\n--- Підключення SMS-сповіщень ---");
        myAccount.Notifier = new SmsNotificationService();
        myAccount.DepositMoney(500.00m);

        // 3. ПІДМІНА КОМПОНЕНТА (Вимога СР 4) - Змінюємо SMS на Email на льоту!
        Console.WriteLine("\n--- Підміна компонента: перемикаємо на Email-сповіщення ---");
        myAccount.Notifier = new EmailNotificationService();
        myAccount.WithdrawMoney(300.00m);

        Console.ReadLine();
    }
}