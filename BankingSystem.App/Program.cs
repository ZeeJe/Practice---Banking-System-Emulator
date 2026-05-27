using System;
using System.Collections.Generic;
using System.Diagnostics;
using BankingSystem.Domain.Entities;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: КОЛЕКЦІЇ .NET ТА ОПТИМІЗАЦІЯ (ЛР 6 / СР 6) ===\n");

        int itemsCount = 50000;
        Console.WriteLine($"Генеруємо {itemsCount} тестових рахунків для перевірки швидкодії...\n");

        // Генеруємо масив даних заздалегідь, щоб не враховувати час їх створення у тестах
        var dummyAccounts = new Account[itemsCount];
        for (int i = 0; i < itemsCount; i++)
        {
            // Використовуємо CheckingAccount, оскільки базовий Account - абстрактний
            dummyAccounts[i] = new CheckingAccount($"Клієнт {i}", 100m);
        }

        // Обираємо рахунок десь із середини списку для тестування пошуку
        Account targetAccount = dummyAccounts[itemsCount / 2]; 
        string targetId = targetAccount.AccountNumber;

        // =======================================================
        // 1. Тестування List<T>
        // =======================================================
        var list = new List<Account>();
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < itemsCount; i++) list.Add(dummyAccounts[i]);
        sw.Stop();
        long listInsertTime = sw.ElapsedMilliseconds;

        sw.Restart();
        // Пошук у списку (лінійний O(N))
        bool listFound = list.Find(a => a.AccountNumber == targetId) != null;
        sw.Stop();
        long listSearchTime = sw.ElapsedTicks; // Використовуємо Ticks, бо O(1) мілісекунди будуть 0

        // =======================================================
        // 2. Тестування Dictionary<TKey, TValue>
        // =======================================================
        var dict = new Dictionary<string, Account>();
        sw.Restart();
        // Ключ - номер рахунку, Значення - сам рахунок
        for (int i = 0; i < itemsCount; i++) dict.Add(dummyAccounts[i].AccountNumber, dummyAccounts[i]);
        sw.Stop();
        long dictInsertTime = sw.ElapsedMilliseconds;

        sw.Restart();
        // Пошук у словнику за ключем (хешування O(1))
        bool dictFound = dict.ContainsKey(targetId);
        sw.Stop();
        long dictSearchTime = sw.ElapsedTicks;

        // =======================================================
        // 3. Тестування HashSet<T>
        // =======================================================
        var hashSet = new HashSet<Account>();
        sw.Restart();
        for (int i = 0; i < itemsCount; i++) hashSet.Add(dummyAccounts[i]);
        sw.Stop();
        long hashSetInsertTime = sw.ElapsedMilliseconds;

        sw.Restart();
        // Пошук у хеш-множині (хешування O(1))
        bool hashSetFound = hashSet.Contains(targetAccount);
        sw.Stop();
        long hashSetSearchTime = sw.ElapsedTicks;

        // =======================================================
        // ВИВІД РЕЗУЛЬТАТІВ
        // =======================================================
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('-', 65));
        Console.WriteLine($"{"Структура",-15} | {"Час вставки (мс)",-20} | {"Час пошуку (тіки)",-20}");
        Console.WriteLine(new string('-', 65));
        Console.WriteLine($"{"List<T>",-15} | {listInsertTime,-20} | {listSearchTime,-20}");
        Console.WriteLine($"{"Dictionary",-15} | {dictInsertTime,-20} | {dictSearchTime,-20}");
        Console.WriteLine($"{"HashSet<T>",-15} | {hashSetInsertTime,-20} | {hashSetSearchTime,-20}");
        Console.WriteLine(new string('-', 65));
        Console.ResetColor();

        // ОБҐРУНТУВАННЯ (Вимога Практичної 6)
        Console.WriteLine("\n=== ОБҐРУНТУВАННЯ ТА ВИСНОВКИ ===");
        Console.WriteLine("1. List<T>: Найшвидший для додавання (в кінець), але найповільніший для пошуку за властивістю (лінійний пошук O(N)). Ідеально для збереження історії транзакцій.");
        Console.WriteLine("2. Dictionary<K,V>: Найкращий для пошуку за унікальним ключем (O(1)). Витрачає трохи більше часу на вставку через виділення пам'яті під бакети хеш-таблиці. Ідеально для бази рахунків банку.");
        Console.WriteLine("3. HashSet<T>: Швидкий пошук (O(1)) та 100% гарантія унікальності. Вимагає коректних Equals() та GetHashCode() (які ми реалізували раніше). Ідеально для списку унікальних ID клієнтів.");

        Console.ReadLine();
    }
}