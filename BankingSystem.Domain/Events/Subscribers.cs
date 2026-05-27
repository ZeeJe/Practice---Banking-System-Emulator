using System;

namespace BankingSystem.Domain.Events
{
    // Підписник 1: Система аудиту (логує все мовчки)
    public class AuditLogger
    {
        public void LogTransaction(object? sender, TransactionEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[Audit Log]: {DateTime.Now:HH:mm:ss} | {e.FromAccount} -> {e.ToAccount} | Сума: {e.Amount} | Комісія: {e.Fee}");
            Console.ResetColor();
        }
    }

    // Підписник 2: Система сповіщень користувача (відправляє SMS)
    public class UserNotifier
    {
        public void SendSmsAlert(object? sender, TransactionEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[SMS Сповіщення]: З рахунку {e.FromAccount} списано {e.Amount + e.Fee} UAH (включно з комісією {e.Fee} UAH).");
            Console.ResetColor();
        }
    }
}