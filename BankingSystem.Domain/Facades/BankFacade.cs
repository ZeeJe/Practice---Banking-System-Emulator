using System;
using BankingSystem.Domain.Composites;
using BankingSystem.Domain.Decorators;

namespace BankingSystem.Domain.Facades
{
    // Фасад приховує складну ініціалізацію підсистем Composite та Decorator
    public class BankFacade
    {
        public void ShowClientPortfolio()
        {
            Console.WriteLine("--- 1. Ініціалізація підсистеми Composite (Дерево активів) ---");
            
            var mainPortfolio = new AccountGroup("Головний Портфель Клієнта");
            
            var fiatGroup = new AccountGroup("Фіатні рахунки");
            fiatGroup.Add(new AccountLeaf("Зарплатна картка", 25000m));
            fiatGroup.Add(new AccountLeaf("Депозит 'Скарбничка'", 100000m));

            var cryptoGroup = new AccountGroup("Крипто-активи");
            cryptoGroup.Add(new AccountLeaf("Bitcoin Гаманець", 150000m));
            cryptoGroup.Add(new AccountLeaf("Ethereum Гаманець", 45000m));

            // Збираємо гілки до купи
            mainPortfolio.Add(fiatGroup);
            mainPortfolio.Add(cryptoGroup);

            // Виводимо все дерево одним викликом
            mainPortfolio.Display(0);
            Console.WriteLine();
        }

        public void IssuePremiumCard()
        {
            Console.WriteLine("--- 2. Ініціалізація підсистеми Decorator (Збірка картки) ---");
            
            // Динамічно загортаємо картку у потрібні декоратори, як матрьошку
            IBankCard myCard = new StandardCard();
            myCard = new CashbackDecorator(myCard); // Огорнули кешбеком
            myCard = new PayPassDecorator(myCard);  // Огорнули NFC

            Console.WriteLine($"Випущено нову картку: [{myCard.GetDescription()}]\n");
        }
    }
}