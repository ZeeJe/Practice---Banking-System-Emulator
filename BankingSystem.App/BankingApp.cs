using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Factories;
using BankingSystem.Domain.Services;
using BankingSystem.Domain.Exceptions;
using BankingSystem.Domain.Facades;
using BankingSystem.Domain.Strategies;
using BankingSystem.Domain.DTOs;
using BankingSystem.Domain.Composites;
using BankingSystem.Domain.Decorators;

namespace BankingSystem.App
{
    /// <summary>
    /// ІНТЕРАКТИВНА БАНКІВСЬКА СИСТЕМА
    /// Дозволяє користувачу:
    /// - Створювати нових клієнтів
    /// - Управляти їхніми рахунками (CRUD)
    /// - Виконувати фінансові операції
    /// - Переглядати портфоліо
    /// - Демонструвати всі паттерни в дії
    /// 
    /// Демонструє паттерни:
    /// ✓ Factory Method (AccountFactory)
    /// ✓ Singleton (CentralBank)
    /// ✓ Strategy (TransferStrategies)
    /// ✓ Observer (Events)
    /// ✓ Decorator (Card options)
    /// ✓ Composite (Portfolio tree)
    /// ✓ Facade (BankFacade)
    /// </summary>
    public class BankingApp
    {
        private readonly List<Customer> _customers = new();
        private Customer? _currentCustomer;
        private readonly JsonStorageService _storageService = new();
        private readonly BankFacade _bankFacade = new();
        private const string CustomersFilePath = "bank_customers.json";

        public void Run()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();

            // Завантажуємо збережені дані
            LoadCustomers();

            bool running = true;
            while (running)
            {
                ShowMainMenu();
                string? choice = Console.ReadLine()?.Trim();

                try
                {
                    running = HandleMainMenuChoice(choice);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n❌ Помилка: {ex.Message}\n");
                    Console.ResetColor();
                }
            }

            SaveCustomers();
            Console.WriteLine("\n✓ До побачення! Дані збережено.");
        }

        private void ShowMainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.WriteLine("║     🏦 ІНТЕРАКТИВНА БАНКІВСЬКА СИСТЕМА 🏦        ║");
            Console.WriteLine("╚══════════════════════════════════════════════════╝\n");
            Console.ResetColor();

            if (_currentCustomer != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"👤 Поточний клієнт: {_currentCustomer.FullName}");
                Console.WriteLine($"📊 Рахунків: {_currentCustomer.Accounts.Count}\n");
                Console.ResetColor();
            }

            Console.WriteLine("═══ ГОЛОВНЕ МЕНЮ ═══");
            Console.WriteLine("1. 👤 Створити нового клієнта");
            Console.WriteLine("2. 🔄 Вибрати існуючого клієнта");
            Console.WriteLine("3. 📋 Список всіх клієнтів");
            
            if (_currentCustomer != null)
            {
                Console.WriteLine("\n═══ ОПЕРАЦІЇ З РАХУНКАМИ ═══");
                Console.WriteLine("4. ➕ Відкрити новий рахунок");
                Console.WriteLine("5. 💰 Поповнити рахунок");
                Console.WriteLine("6. 💸 Зняти кошти");
                Console.WriteLine("7. 🔄 Переказ між рахунками");
                Console.WriteLine("8. 👁️  Переглянути рахунки");
                Console.WriteLine("9. 🎯 Показати портфоліо (Composite)");
                Console.WriteLine("10. 🎁 Випустити преміум-картку (Decorator)");
            }

            Console.WriteLine("\n═══ СЕРВІС ═══");
            Console.WriteLine("11. 💾 Зберегти дані");
            Console.WriteLine("12. 📂 Завантажити дані");
            Console.WriteLine("13. ❌ Вихід");
            Console.WriteLine("\nВиберіть опцію: ");
        }

        private bool HandleMainMenuChoice(string? choice)
        {
            if (string.IsNullOrWhiteSpace(choice))
            {
                PrintError("Невірний вибір!");
                return true;
            }

            // Перевіримо, чи потрібен клієнт
            if ("456789".Contains(choice) && _currentCustomer == null)
            {
                PrintError("Виберіть клієнта першим!");
                return true;
            }

            return choice switch
            {
                "1" => CreateNewCustomer(),
                "2" => SelectCustomer(),
                "3" => ListAllCustomers(),
                "4" => OpenNewAccount(),
                "5" => DepositMoney(),
                "6" => WithdrawMoney(),
                "7" => TransferMoney(),
                "8" => ShowAccounts(),
                "9" => ShowPortfolio(),
                "10" => IssuePremiumCard(),
                "11" => SaveAndReturn(),
                "12" => LoadAndReturn(),
                "13" => false,
                _ => DefaultOption()
            };
        }

        private bool DefaultOption()
        {
            PrintError("Невірний вибір!");
            return true;
        }

        private bool SaveAndReturn()
        {
            SaveCustomers();
            PrintSuccess("✓ Дані збережено!");
            return true;
        }

        private bool LoadAndReturn()
        {
            LoadCustomers();
            PrintSuccess("✓ Дані завантажено!");
            return true;
        }

        // ════════════════════════════════════════════════════════════════
        // 1️⃣ УПРАВЛІННЯ КЛІЄНТАМИ
        // ════════════════════════════════════════════════════════════════

        private bool CreateNewCustomer()
        {
            Console.WriteLine("\n═══ СТВОРЕННЯ НОВОГО КЛІЄНТА ═══");
            Console.Write("Введіть ім'я клієнта: ");
            string? name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                PrintError("Ім'я не може бути порожнім!");
                return true;
            }

            var customer = new Customer { FullName = name };
            _customers.Add(customer);
            _currentCustomer = customer;

            PrintSuccess($"✓ Клієнта '{name}' успішно створено!");
            return true;
        }

        private bool SelectCustomer()
        {
            if (_customers.Count == 0)
            {
                PrintError("Немає жодного клієнта!");
                return true;
            }

            Console.WriteLine("\n═══ ВИБІР КЛІЄНТА ═══");
            for (int i = 0; i < _customers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_customers[i].FullName} ({_customers[i].Accounts.Count} рахунків)");
            }

            Console.Write("Виберіть номер клієнта: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= _customers.Count)
            {
                _currentCustomer = _customers[choice - 1];
                PrintSuccess($"✓ Вибрано клієнта: {_currentCustomer.FullName}");
                return true;
            }

            PrintError("Невірний вибір!");
            return true;
        }

        private bool ListAllCustomers()
        {
            Console.WriteLine("\n═══ СПИСОК ВСІХ КЛІЄНТІВ ═══");
            
            if (_customers.Count == 0)
            {
                PrintError("Немає жодного клієнта!");
                return true;
            }

            for (int i = 0; i < _customers.Count; i++)
            {
                var customer = _customers[i];
                decimal totalBalance = customer.Accounts.Sum(a => a.Balance);
                string marker = _currentCustomer == customer ? " 👈" : "";
                
                Console.WriteLine($"{i + 1}. {customer.FullName}{marker}");
                Console.WriteLine($"   📊 Рахунків: {customer.Accounts.Count}");
                Console.WriteLine($"   💰 Загальний баланс: {totalBalance:C}");
                
                if (customer.Accounts.Count > 0)
                {
                    foreach (var account in customer.Accounts)
                    {
                        Console.WriteLine($"     • {account.AccountNumber}: {account.Balance:C}");
                    }
                }
            }

            Console.WriteLine("\nНатисніть Enter для продовження...");
            Console.ReadLine();
            return true;
        }

        // ════════════════════════════════════════════════════════════════
        // 2️⃣ УПРАВЛІННЯ РАХУНКАМИ (Factory Method паттерн)
        // ════════════════════════════════════════════════════════════════

        private bool OpenNewAccount()
        {
            Console.WriteLine("\n═══ ВІДКРИТТЯ НОВОГО РАХУНКУ ═══");
            Console.WriteLine("Тип рахунку:");
            Console.WriteLine("1. Checking (Поточний)");
            Console.WriteLine("2. Deposit (Депозит)");
            Console.Write("Виберіть тип: ");

            string? typeChoice = Console.ReadLine();
            string accountType = typeChoice switch
            {
                "1" => "checking",
                "2" => "deposit",
                _ => ""
            };

            if (string.IsNullOrEmpty(accountType))
            {
                PrintError("Невірний вибір!");
                return true;
            }

            Console.Write("Введіть початковий баланс: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal initialBalance) || initialBalance < 0)
            {
                PrintError("Невірна сума!");
                return true;
            }

            // 🏭 FACTORY METHOD ПАТТЕРН
            // AccountFactory створює правильний тип рахунку на основі параметра
            try
            {
                var account = AccountFactory.CreateAccount(accountType, _currentCustomer!.FullName, initialBalance);
                _currentCustomer!.Accounts.Add(account);
                
                PrintSuccess($"✓ Рахунок {account.AccountNumber} успішно відкрито!");
                Console.WriteLine($"  Тип: {account.GetType().Name}");
                Console.WriteLine($"  Баланс: {account.Balance:C}\n");
            }
            catch (Exception ex)
            {
                PrintError($"Помилка при створенні рахунку: {ex.Message}");
            }

            return true;
        }

        // ════════════════════════════════════════════════════════════════
        // 3️⃣ ОПЕРАЦІЇ З ГРОШИМА
        // ════════════════════════════════════════════════════════════════

        private bool DepositMoney()
        {
            if (_currentCustomer!.Accounts.Count == 0)
            {
                PrintError("У клієнта немає рахунків!");
                return true;
            }

            Console.WriteLine("\n═══ ПОПОВНЕННЯ РАХУНКУ ═══");
            var account = SelectAccount("для поповнення");
            if (account == null) return true;

            Console.Write("Сума внеску: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                PrintError("Невірна сума!");
                return true;
            }

            try
            {
                account.DepositMoney(amount);
                PrintSuccess($"✓ Поповнено на {amount:C}");
                Console.WriteLine($"  Новий баланс: {account.Balance:C}\n");
            }
            catch (Exception ex)
            {
                PrintError($"Помилка: {ex.Message}");
            }

            return true;
        }

        private bool WithdrawMoney()
        {
            if (_currentCustomer!.Accounts.Count == 0)
            {
                PrintError("У клієнта немає рахунків!");
                return true;
            }

            Console.WriteLine("\n═══ ЗНЯТТЯ КОШТІВ ═══");
            var account = SelectAccount("для зняття");
            if (account == null) return true;

            Console.Write("Сума зняття: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                PrintError("Невірна сума!");
                return true;
            }

            try
            {
                account.WithdrawMoney(amount);
                PrintSuccess($"✓ Знято {amount:C}");
                Console.WriteLine($"  Новий баланс: {account.Balance:C}\n");
            }
            catch (InsufficientFundsException ex)
            {
                PrintError($"❌ Недостатньо коштів: {ex.Message}");
            }
            catch (Exception ex)
            {
                PrintError($"Помилка: {ex.Message}");
            }

            return true;
        }

        private bool TransferMoney()
        {
            if (_currentCustomer!.Accounts.Count < 2)
            {
                PrintError("Потрібно мінімум 2 рахунки!");
                return true;
            }

            Console.WriteLine("\n═══ ПЕРЕКАЗ МІЖ РАХУНКАМИ ═══");
            var fromAccount = SelectAccount("для переказу (від)");
            if (fromAccount == null) return true;

            var toAccount = SelectAccount("для отримання (до)");
            if (toAccount == null) return true;

            if (fromAccount == toAccount)
            {
                PrintError("Вибіріть різні рахунки!");
                return true;
            }

            Console.Write("Сума переказу: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                PrintError("Невірна сума!");
                return true;
            }

            Console.WriteLine("\nВиберіть стратегію переказу:");
            Console.WriteLine("1. Local (без комісії)");
            Console.WriteLine("2. Swift (комісія 5% + 50 UAH)");
            Console.Write("Виберіть: ");

            // 📊 STRATEGY ПАТТЕРН
            // Вибираємо стратегію для розрахунку комісії
            ITransferStrategy strategy = Console.ReadLine() switch
            {
                "1" => new LocalTransferStrategy(),
                "2" => new SwiftTransferStrategy(),
                _ => new LocalTransferStrategy()
            };

            try
            {
                decimal fee = strategy.CalculateFee(amount);
                decimal totalAmount = amount + fee;

                if (fromAccount.Balance < totalAmount)
                {
                    PrintError($"Недостатньо коштів (потрібно: {totalAmount:C}, є: {fromAccount.Balance:C})");
                    return true;
                }

                fromAccount.WithdrawMoney(totalAmount);
                toAccount.DepositMoney(amount);

                PrintSuccess($"✓ Переказ успішно виконано!");
                Console.WriteLine($"  Сума: {amount:C}");
                Console.WriteLine($"  Комісія: {fee:C}");
                Console.WriteLine($"  Разом: {totalAmount:C}\n");
            }
            catch (Exception ex)
            {
                PrintError($"Помилка при переказі: {ex.Message}");
            }

            return true;
        }

        // ════════════════════════════════════════════════════════════════
        // 4️⃣ ДЕМОНСТРАЦІЯ ПАТТЕРНІВ
        // ════════════════════════════════════════════════════════════════

        private bool ShowAccounts()
        {
            Console.WriteLine("\n═══ РАХУНКИ КЛІЄНТА ═══");
            
            if (_currentCustomer!.Accounts.Count == 0)
            {
                PrintError("У клієнта немає рахунків!");
                return true;
            }

            for (int i = 0; i < _currentCustomer.Accounts.Count; i++)
            {
                var account = _currentCustomer.Accounts[i];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n{i + 1}. {account}");
                Console.ResetColor();
            }

            Console.WriteLine("\nНатисніть Enter для продовження...");
            Console.ReadLine();
            return true;
        }

        private bool ShowPortfolio()
        {
            // 🎯 COMPOSITE ПАТТЕРН
            // Демонструємо ієрархічну структуру активів
            Console.WriteLine("\n═══ ПОРТФОЛІО КЛІЄНТА (Composite) ═══\n");
            
            var mainPortfolio = new AccountGroup($"Портфель {_currentCustomer!.FullName}");
            var fiatGroup = new AccountGroup("📊 Фіатні рахунки");
            var cryptoGroup = new AccountGroup("🔐 Крипто-активи");

            // Додаємо реальні рахунки клієнта
            foreach (var account in _currentCustomer.Accounts)
            {
                string type = account.GetType().Name == "Deposit" ? "🏦 Депозит" : "💳 Поточний";
                fiatGroup.Add(new AccountLeaf($"{type} {account.AccountNumber}", account.Balance));
            }

            // Додаємо демонстраційні крипто-активи
            cryptoGroup.Add(new AccountLeaf("₿ Bitcoin Гаманець", 150000m));
            cryptoGroup.Add(new AccountLeaf("Ξ Ethereum Гаманець", 45000m));

            mainPortfolio.Add(fiatGroup);
            mainPortfolio.Add(cryptoGroup);

            // Виводимо весь портфоліо
            mainPortfolio.Display(0);

            Console.WriteLine("\nНатисніть Enter для продовження...");
            Console.ReadLine();
            return true;
        }

        private bool IssuePremiumCard()
        {
            // 🎁 DECORATOR ПАТТЕРН
            // Динамічно "огортаємо" картку додатковими послугами
            Console.WriteLine("\n═══ ВИПУСК ПРЕМІУМ-КАРТКИ (Decorator) ═══\n");

            Console.WriteLine("Виберіть додаткові послуги:");
            Console.WriteLine("1. Кешбек (2% на кожну покупку)");
            Console.WriteLine("2. PayPass (NFC платежі)");
            Console.WriteLine("3. Кешбек + PayPass");
            Console.WriteLine("4. Звичайна картка");
            Console.Write("Виберіть: ");

            IBankCard card = new StandardCard();

            string? choice = Console.ReadLine();
            if (choice == "1" || choice == "3")
            {
                card = new CashbackDecorator(card);
            }
            if (choice == "2" || choice == "3")
            {
                card = new PayPassDecorator(card);
            }

            PrintSuccess("✓ Картка успішно випущена!");
            Console.WriteLine($"\n📋 Опис картки:");
            Console.WriteLine($"   {card.GetDescription()}\n");

            Console.WriteLine("Натисніть Enter для продовження...");
            Console.ReadLine();
            return true;
        }

        // ════════════════════════════════════════════════════════════════
        // 5️⃣ ЗБЕРЕЖЕННЯ / ЗАВАНТАЖЕННЯ
        // ════════════════════════════════════════════════════════════════

        private void SaveCustomers()
        {
            try
            {
                var dtos = _customers.Select(c => new CustomerDto
                {
                    FullName = c.FullName,
                    Accounts = c.Accounts.Select(a => new AccountDto
                    {
                        AccountId = a.AccountNumber,
                        Balance = a.Balance
                    }).ToList()
                }).ToList();

                _storageService.SaveToFile(CustomersFilePath, dtos);
            }
            catch (Exception ex)
            {
                PrintError($"Помилка при збереженні: {ex.Message}");
            }
        }

        private void LoadCustomers()
        {
            try
            {
                if (!File.Exists(CustomersFilePath))
                    return;

                var dtos = _storageService.LoadFromFile<List<CustomerDto>>(CustomersFilePath);
                if (dtos == null) return;

                _customers.Clear();
                foreach (var dto in dtos)
                {
                    var customer = new Customer { FullName = dto.FullName };
                    
                    foreach (var accountDto in dto.Accounts)
                    {
                        // Визначаємо тип рахунку за номером
                        var account = new CheckingAccount(dto.FullName, accountDto.Balance);
                        account.GetType().GetProperty("AccountNumber")?.SetValue(account, accountDto.AccountId);
                        customer.Accounts.Add(account);
                    }

                    _customers.Add(customer);
                }

                PrintSuccess($"✓ Завантажено {_customers.Count} клієнтів");
            }
            catch (Exception ex)
            {
                PrintError($"Помилка при завантаженні: {ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════════
        // ДОПОМІЖНІ МЕТОДИ
        // ════════════════════════════════════════════════════════════════

        private Account? SelectAccount(string purpose)
        {
            Console.WriteLine($"\nВиберіть рахунок {purpose}:");
            for (int i = 0; i < _currentCustomer!.Accounts.Count; i++)
            {
                var account = _currentCustomer.Accounts[i];
                Console.WriteLine($"{i + 1}. {account.AccountNumber}: {account.Balance:C}");
            }

            Console.Write("Номер рахунку: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= _currentCustomer.Accounts.Count)
            {
                return _currentCustomer.Accounts[choice - 1];
            }

            PrintError("Невірний вибір!");
            return null;
        }

        private void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {message}");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Простий клас Customer для управління клієнтами
    /// </summary>
    public class Customer
    {
        public string FullName { get; set; } = string.Empty;
        public List<Account> Accounts { get; set; } = new();
    }
}
