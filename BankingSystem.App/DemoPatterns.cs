using System;
using System.Collections.Generic;
using BankingSystem.Domain.Composites;
using BankingSystem.Domain.Decorators;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Factories;
using BankingSystem.Domain.Facades;
using BankingSystem.Domain.Strategies;
using BankingSystem.Domain.Extensions;

namespace BankingSystem.App
{
    /// <summary>
    /// Демонстрація всіх Design Patterns, реалізованих у проекті (ПР 16).
    /// Використання: var demo = new DemoPatterns(); demo.RunInteractiveMenu();
    /// </summary>
    public class DemoPatterns
    {
        public void RunInteractiveMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                DisplayMainMenu();
                string? choice = Console.ReadLine();

                switch (choice?.ToLower())
                {
                    case "1":
                        DemoStrategy();
                        break;
                    case "2":
                        DemoFactory();
                        break;
                    case "3":
                        DemoComposite();
                        break;
                    case "4":
                        DemoDecorator();
                        break;
                    case "5":
                        DemoFacade();
                        break;
                    case "6":
                        DemoLINQ();
                        break;
                    case "0":
                        Console.WriteLine("\nДо побачення!");
                        return;
                    default:
                        Console.WriteLine("\nНевідома команда. Спробуйте ще раз.\n");
                        break;
                }
            }
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ДЕМОНСТРАЦІЯ DESIGN PATTERNS - Banking System");
            Console.ResetColor();
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("1. Strategy Pattern — Різні алгоритми комісій за перекази");
            Console.WriteLine("2. Factory Method — Динамічне створення рахунків");
            Console.WriteLine("3. Composite Pattern — Портфель активів (дерево рахунків)");
            Console.WriteLine("4. Decorator Pattern — Побудова премійної картки");
            Console.WriteLine("5. Facade Pattern — Спрощений інтерфейс до складних підсистем");
            Console.WriteLine("6. LINQ Extensions — Розширені запити до рахунків");
            Console.WriteLine("0. Вихід");
            Console.WriteLine(new string('=', 60));
            Console.Write("Виберіть пункт меню: ");
        }

        private void DemoStrategy()
        {
            Console.WriteLine("\n" + new string('-', 60));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("STRATEGY PATTERN: Порівняння комісій за перекази");
            Console.ResetColor();
            Console.WriteLine(new string('-', 60));

            decimal transferAmount = 10000m;
            Console.WriteLine($"\nПередавленням суму: {transferAmount} UAH\n");

            ITransferStrategy[] strategies = new ITransferStrategy[]
            {
                new LocalTransferStrategy(),
                new SwiftTransferStrategy()
            };

            foreach (var strategy in strategies)
            {
                decimal fee = strategy.CalculateFee(transferAmount);
                decimal netAmount = transferAmount - fee;
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{strategy.GetTransferType()}");
                Console.ResetColor();
                Console.WriteLine($"   Вихідна сума: {transferAmount} UAH");
                Console.WriteLine($"   Комісія: {fee} UAH");
                Console.WriteLine($"   На рахунок прийде: {netAmount} UAH\n");
            }

            Console.WriteLine("Висновок: Strategy дозволяє легко додавати нові алгоритми\n");
            Console.ReadLine();
        }

        private void DemoFactory()
        {
            Console.WriteLine("\n" + new string('-', 60));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("FACTORY METHOD PATTERN: Динамічне створення рахунків");
            Console.ResetColor();
            Console.WriteLine(new string('-', 60));

            var configs = new List<(string Type, string Owner, decimal Balance)>
            {
                ("checking", "Іван Петренко", 5000m),
                ("deposit", "Марія Сідоренко", 50000m),
                ("checking", "Петро Vasylenko", 12000m),
            };

            Console.WriteLine($"\nСтворюємо {configs.Count} рахунків...\n");

            foreach (var (type, owner, balance) in configs)
            {
                try
                {
                    var account = AccountFactory.CreateAccount(type, owner, balance);
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Тип: {type.ToUpper()}");
                    Console.ResetColor();
                    Console.WriteLine($"  {account.GetAccountDetails()}");
                    Console.WriteLine($"  Власник: {owner}");
                    Console.WriteLine($"  Баланс: {balance} UAH\n");
                }
                catch (ArgumentException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Помилка: {ex.Message}\n");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("Висновок: Factory ізолює логіку створення\n");
            Console.ReadLine();
        }

        private void DemoComposite()
        {
            Console.WriteLine("\n" + new string('-', 60));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("COMPOSITE PATTERN: Портфель активів (дерево рахунків)");
            Console.ResetColor();
            Console.WriteLine(new string('-', 60));

            var mainPortfolio = new AccountGroup("Головний Портфель Клієнта");

            var fiatGroup = new AccountGroup("Фіатні Рахунки");
            fiatGroup.Add(new AccountLeaf("Зарплатна картка", 25000m));
            fiatGroup.Add(new AccountLeaf("Депозит 'Скарбничка'", 100000m));

            var cryptoGroup = new AccountGroup("Крипто-активи");
            cryptoGroup.Add(new AccountLeaf("Bitcoin Гаманець", 150000m));
            cryptoGroup.Add(new AccountLeaf("Ethereum Гаманець", 45000m));

            mainPortfolio.Add(fiatGroup);
            mainPortfolio.Add(cryptoGroup);

            Console.WriteLine("\nСтруктура портфеля:\n");
            mainPortfolio.Display(0);

            decimal totalBalance = mainPortfolio.GetTotalBalance();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n---------------------------------------------------\n");
            Console.WriteLine($"ЗАГАЛЬНИЙ БАЛАНС: {totalBalance:N2} UAH\n");
            Console.WriteLine($"---------------------------------------------------\n");
            Console.ResetColor();

            Console.WriteLine("Висновок: Composite приховує ієрархію\n");
            Console.ReadLine();
        }

        private void DemoDecorator()
        {
            Console.WriteLine("\n" + new string('-', 60));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("DECORATOR PATTERN: Побудова премійної банківської картки");
            Console.ResetColor();
            Console.WriteLine(new string('-', 60));

            Console.WriteLine("\nПобудова картки через декоратори (як матрьошка):\n");

            IBankCard card = new StandardCard();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"1. Базова картка:");
            Console.ResetColor();
            Console.WriteLine($"   {card.GetDescription()}\n");

            card = new CashbackDecorator(card);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"2. + Кешбек декоратор:");
            Console.ResetColor();
            Console.WriteLine($"   {card.GetDescription()}\n");

            card = new PayPassDecorator(card);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"3. + PayPass декоратор:");
            Console.ResetColor();
            Console.WriteLine($"   {card.GetDescription()}\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Премійна картка успішно побудована!\n");
            Console.ResetColor();

            Console.WriteLine("Висновок: Decorator дозволяє динамічно розширювати\n");
            Console.ReadLine();
        }

        private void DemoFacade()
        {
            Console.WriteLine("\n" + new string('-', 60));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("FACADE PATTERN: Спрощений інтерфейс до складних підсистем");
            Console.ResetColor();
            Console.WriteLine(new string('-', 60));

            var bankFacade = new BankFacade();

            Console.WriteLine();
            bankFacade.ShowClientPortfolio();

            Console.WriteLine();
            bankFacade.IssuePremiumCard();

            Console.WriteLine("Висновок: Facade приховує складність за простим API\n");
            Console.ReadLine();
        }

        private void DemoLINQ()
        {
            Console.WriteLine("\n" + new string('-', 60));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("LINQ EXTENSIONS: Складні запити до рахунків");
            Console.ResetColor();
            Console.WriteLine(new string('-', 60));

            var accounts = new List<Account>
            {
                new CheckingAccount("Іван", 5000m),
                new CheckingAccount("Марія", 15000m),
                new Deposit("Іван", 50000m, DateTime.Now.AddYears(1), 12.5m),
                new CheckingAccount("Петро", 3000m),
                new Deposit("Марія", 100000m, DateTime.Now.AddYears(1), 12.5m),
            };

            Console.WriteLine("\n1. GroupBy — Групування рахунків за власником:\n");
            var grouped = accounts.GroupByOwner();
            foreach (var group in grouped)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   {group.Key}:");
                Console.ResetColor();
                foreach (var acc in group.Value)
                {
                    Console.WriteLine($"      • {acc.GetAccountDetails()} — {acc.Balance} UAH");
                }
            }

            Console.WriteLine("\n2. Aggregate — Статистика по рахункам:\n");
            var (total, count, avg) = accounts.GetAccountStats();
            Console.WriteLine($"   Загальна кількість: {count}");
            Console.WriteLine($"   Загальний баланс: {total} UAH");
            Console.WriteLine($"   Середній баланс: {avg:N2} UAH");

            Console.WriteLine("\nВисновок: Extension Methods виносять логіку у переюсиві методи\n");
            Console.ReadLine();
        }
    }
}
