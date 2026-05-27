using System;
using System.Collections.Generic;
using System.Linq;

namespace BankingSystem.Domain.Composites
{
    // Спільний інтерфейс для окремого рахунку (Листок) та цілої групи (Вузол)
    public interface IPortfolioComponent
    {
        decimal GetTotalBalance();
        void Display(int depth);
    }

    // Leaf (Листок) - окремий, кінцевий рахунок
    public class AccountLeaf : IPortfolioComponent
    {
        private string _name;
        private decimal _balance;

        public AccountLeaf(string name, decimal balance)
        {
            _name = name;
            _balance = balance;
        }

        public decimal GetTotalBalance() => _balance;

        public void Display(int depth)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(new string(' ', depth * 2) + $"- [Рахунок] {_name}: {_balance} UAH");
            Console.ResetColor();
        }
    }

    // Composite (Вузол) - група рахунків (може містити як окремі рахунки, так і інші підгрупи)
    public class AccountGroup : IPortfolioComponent
    {
        private string _groupName;
        private List<IPortfolioComponent> _components = new List<IPortfolioComponent>();

        public AccountGroup(string groupName)
        {
            _groupName = groupName;
        }

        public void Add(IPortfolioComponent component) => _components.Add(component);
        public void Remove(IPortfolioComponent component) => _components.Remove(component);

        // Рекурсивно збираємо баланс з усіх вкладених елементів
        public decimal GetTotalBalance() => _components.Sum(c => c.GetTotalBalance());

        public void Display(int depth)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string(' ', depth * 2) + $"+ [Група] {_groupName} (Загальний баланс: {GetTotalBalance()} UAH)");
            Console.ResetColor();
            
            // Просимо всіх "дітей" відобразити себе
            foreach (var component in _components)
            {
                component.Display(depth + 1);
            }
        }
    }
}