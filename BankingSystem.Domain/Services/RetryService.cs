using System;
using System.Threading;
using BankingSystem.Domain.Exceptions;

namespace BankingSystem.Domain.Services
{
    public static class RetryService
    {
        // Універсальний метод, що приймає будь-яку функцію і виконує її з політикою повторів
        public static T ExecuteWithRetry<T>(Func<T> operation, int maxAttempts = 3, int baseDelayMs = 200)
        {
            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    return operation(); // Спроба виконати основну логіку
                }
                catch (NetworkGlitchException ex) // Повторюємо спроби ТІЛЬКИ при тимчасових збоях мережі
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[LOG - {DateTime.Now:HH:mm:ss}]: Спроба {attempt} невдала через збій мережі: {ex.Message}");
                    Console.ResetColor();

                    if (attempt >= maxAttempts)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[LOG]: Вичерпано ліміт спроб ({maxAttempts}). Відновлення неможливе.");
                        Console.ResetColor();
                        throw; // Якщо всі спроби вичерпано — прокидаємо помилку далі
                    }

                    // Алгоритм експоненційної затримки (Exponential Backoff): 200мс -> 400мс -> 800мс...
                    int delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                    Console.WriteLine($"[Retry Policy]: Очікування {delay} мс перед спробою №{attempt + 1}...");
                    
                    Thread.Sleep(delay); // Безпечне засинання потоку між спробами
                }
            }
        }
    }
}