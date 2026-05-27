using System;
using BankingSystem.Domain.Interfaces;

namespace BankingSystem.Domain.Services
{
    // Імплементація 1: Відправка через SMS (Консоль)
    public class SmsNotificationService : INotificationService
    {
        public void SendNotification(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[SMS Сповіщення]: {message}");
            Console.ResetColor();
        }
    }

    // Імплементація 2: Відправка через Email
    public class EmailNotificationService : INotificationService
    {
        public void SendNotification(string message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[Email Сповіщення]: {message}");
            Console.ResetColor();
        }
    }
}