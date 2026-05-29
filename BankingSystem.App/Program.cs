using System;
using System.IO;
using BankingSystem.Domain.DTOs;
using BankingSystem.Domain.Services;
using BankingSystem.App;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        // Меню для вибору
        Console.WriteLine("=== BANKING SYSTEM - Меню ===\n");
        Console.WriteLine("1. DTO та JSON Серіалізація (ЛР 13)");
        Console.WriteLine("2. Design Patterns Демонстрація (ПР 16)");
        Console.WriteLine("Виберіть опцію (1 або 2): ");
        
        string? choice = Console.ReadLine();
        
        if (choice == "2")
        {
            var demoPatterns = new DemoPatterns();
            demoPatterns.RunInteractiveMenu();
            return;
        }
        
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: ЗБЕРЕЖЕННЯ СТАНУ ТА DTO (ЛР 13 / СР 13) ===\n");

        string filePath = "customer_state.json";
        var storageService = new JsonStorageService();

        // 1. СТВОРЕННЯ СКЛАДНОГО ОБ'ЄКТА З ЦИКЛІЧНИМИ ПОСИЛАННЯМИ В ДОМЕНІ
        var domainCustomer = new DomainCustomer { FullName = "Віталій Бутерін" };
        
        var acc1 = new DomainAccount { AccountId = "UA-CRYPTO-777", Balance = 540000m, Owner = domainCustomer };
        var acc2 = new DomainAccount { AccountId = "UA-FIAT-888", Balance = 12000m, Owner = domainCustomer };
        
        domainCustomer.Accounts.Add(acc1);
        domainCustomer.Accounts.Add(acc2);

        // Демонстрація зацикленості в пам'яті:
        Console.WriteLine($"[Домен]: Клієнт -> {domainCustomer.FullName}");
        Console.WriteLine($"   Рахунок 1: {domainCustomer.Accounts[0].AccountId}");
        Console.WriteLine($"   Власник Рахунку 1 назад у пам'яті: {domainCustomer.Accounts[0].Owner.FullName}");
        Console.WriteLine("   -> Зв'язок зациклений. Пряма серіалізація небезпечна.\n");

        // 2. БЕЗПЕЧНЕ ПЕРЕВЕДЕННЯ В DTO ТА СЕРІАЛІЗАЦІЯ
        Console.WriteLine("--- 2. Маппінг у DTO та збереження в JSON ---");
        CustomerDto dtoToSave = CustomerMapper.ToDto(domainCustomer);

        // Зберігаємо ізольовану структуру DTO
        storageService.SaveToFile(filePath, dtoToSave);

        // Виведемо вміст файлу прямо в консоль, щоб перевірити красу JSON
        Console.WriteLine("\n[Вміст створеного файлу JSON]:");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(File.ReadAllText(filePath));
        Console.ResetColor();

        // 3. ДЕСЕРІАЛІЗАЦІЯ ТА ВІДНОВЛЕННЯ СТАНУ
        Console.WriteLine("--- 3. Відновлення стану (JSON -> DTO -> Домен) ---");
        CustomerDto? LoadedDto = storageService.LoadFromFile<CustomerDto>(filePath);

        if (LoadedDto != null)
        {
            // Перетворюємо назад у повноцінну доменну модель
            DomainCustomer restoredCustomer = CustomerMapper.ToDomain(LoadedDto);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"-> Успішно відновлено клієнта: {restoredCustomer.FullName}");
            Console.WriteLine($"-> Кількість рахунків: {restoredCustomer.Accounts.Count}");
            
            // Перевіряємо чи відновився цикл взаємозв'язків у пам'яті об'єктів
            Console.WriteLine($"-> Перевірка циклічності після завантаження: Рахунок '{restoredCustomer.Accounts[1].AccountId}' належить клієнту '{restoredCustomer.Accounts[1].Owner.FullName}'");
            Console.ResetColor();
        }

        // Очищаємо за собою сліди (опціонально)
        // if (File.Exists(filePath)) File.Delete(filePath);  // ЗАКОМЕНТОВАНО: Файл залишається для демонстрації

        Console.ReadLine();
    }
}