using System;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.Services;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: ОБРОБКА ВИinternalНЯТКІВ ТА RETRY POLICY (ЛР 8 / СР 8) ===\n");

        var account = new CheckingAccount("Олександр", 1000m);

        // =======================================================
        // ПРАКТИЧНА 8: Custom Exceptions та try/catch/finally
        // =======================================================
        Console.WriteLine("--- 1. Тестування бізнес-винятку InsufficientFundsException ---");
        try
        {
            Console.WriteLine($"Поточний баланс рахунку: {account.Balance} UAH.");
            Console.WriteLine("Спроба зняти 5000 UAH...");
            
            if (5000m > account.Balance)
            {
                // Кидаємо наш кастомний виняток із передачею контексту помилки
                throw new InsufficientFundsException("Помилка списання: Недостатньо коштів!", 5000m, account.Balance);
            }
            account.WithdrawMoney(5000m);
        }
        catch (InsufficientFundsException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Перехоплено виняток]: {ex.Message}");
            Console.WriteLine($"Деталі помилки: Запит на {ex.AttemptedAmount} UAH | Доступно лише {ex.CurrentBalance} UAH");
            Console.ResetColor();
        }
        finally
        {
            // Гарантоване виконання очищення або закриття сесій
            Console.WriteLine("[Finally Блок]: Банківську сесію безпечно закрито.");
        }

        Console.WriteLine("\n" + new string('=', 60) + "\n");

        // =======================================================
        // САМОСТІЙНА 8: Retry Policy з експоненційною затримкою
        // =======================================================
        Console.WriteLine("--- 2. Тестування автоматичного відновлення (Retry Policy) ---");

        int simulatedGlitchCounter = 0;

        // Імітуємо хмарний запит до Нацбанку, який перші дві спроби «лежить», а на третю оживає
        Func<string> syncWithCentralBankServer = () =>
        {
            simulatedGlitchCounter++;
            if (simulatedGlitchCounter < 3)
            {
                throw new NetworkGlitchException("Remote server timeout. З'єднання розірвано сервером.");
            }
            return "Успіх! Баланси успішно синхронізовано з центральним сервером.";
        };

        try
        {
            // Виконуємо нестабільну операцію через нашу політику повторів
            string transactionStatus = RetryService.ExecuteWithRetry(syncWithCentralBankServer, maxAttempts: 3, baseDelayMs: 300);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[Основна програма]: Операція завершилась успіхом -> {transactionStatus}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Критична помилка в Main]: Програма не змогла відновитись: {ex.Message}");
        }

        Console.ReadLine();
    }
}