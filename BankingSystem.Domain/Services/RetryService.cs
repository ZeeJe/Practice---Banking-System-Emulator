using System;
using System.Threading;
using BankingSystem.Domain.Common;
using BankingSystem.Domain.Exceptions;

namespace BankingSystem.Domain.Services
{
    /// <summary>
    /// Сервіс для виконання операцій з автоматичним повторенням у разі тимчасових помилок.
    /// Використовує експоненціальну затримку (Exponential Backoff) для запобігання перевантаженню.
    /// </summary>
    public static class RetryService
    {
        /// <summary>
        /// Виконує операцію з автоматичним повторенням при NetworkGlitchException.
        /// </summary>
        /// <typeparam name="T">Тип результату операції</typeparam>
        /// <param name="operation">Функція для виконання</param>
        /// <param name="maxAttempts">Максимальна кількість спроб (за умовч. 3)</param>
        /// <param name="baseDelayMs">Базова затримка в мс між спробами (за умовч. 200)</param>
        /// <returns>Результат успішного виконання операції</returns>
        /// <exception cref="NetworkGlitchException">Коли вичерпані всі спроби повторення</exception>
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
                    // Формула: baseDelayMs * (exponentialBase ^ (attempt - 1))
                    int delay = baseDelayMs * (int)Math.Pow(BankConstants.RetryExponentialBase, attempt - 1);
                    Console.WriteLine($"[Retry Policy]: Очікування {delay} мс перед спробою №{attempt + 1}...");
                    
                    Thread.Sleep(delay); // Безпечне засинання потоку між спробами
                }
            }
        }
    }
}