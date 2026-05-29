using System;
using Xunit;
using BankingSystem.Domain.Composites;

namespace BankingSystem.Tests
{
    /// <summary>
    /// Тести для Composite паттерну (Практична 12 / Самостійна 12).
    /// Перевіряє коректність групування та рекурсивної агрегації портфеля активів.
    /// </summary>
    public class CompositePatternTests
    {
        /// <summary>
        /// Перевіряє, що окремий рахунок (лист) повертає власний баланс.
        /// </summary>
        [Fact]
        public void AccountLeaf_GetTotalBalance_ReturnsSingleAccountBalance()
        {
            // Arrange
            var account = new AccountLeaf("Рахунок 1", 1000m);

            // Act
            decimal total = account.GetTotalBalance();

            // Assert
            Assert.Equal(1000m, total);
        }

        /// <summary>
        /// Перевіряє, що група без елементів повертає нульовий баланс.
        /// </summary>
        [Fact]
        public void AccountGroup_EmptyGroup_ReturnZeroBalance()
        {
            // Arrange
            var group = new AccountGroup("Порожна група");

            // Act
            decimal total = group.GetTotalBalance();

            // Assert
            Assert.Equal(0m, total);
        }

        /// <summary>
        /// Перевіряє, що група із одного рахунку повертає правильну суму.
        /// </summary>
        [Fact]
        public void AccountGroup_SingleAccount_ReturnsCorrectSum()
        {
            // Arrange
            var group = new AccountGroup("Група");
            group.Add(new AccountLeaf("Рахунок 1", 2500m));

            // Act
            decimal total = group.GetTotalBalance();

            // Assert
            Assert.Equal(2500m, total);
        }

        /// <summary>
        /// Перевіряє, що група з кількома рахунками агрегує суму коректно.
        /// </summary>
        [Fact]
        public void AccountGroup_MultipleAccounts_ReturnsCorrectSum()
        {
            // Arrange
            var group = new AccountGroup("Основна група");
            group.Add(new AccountLeaf("Рахунок 1", 1000m));
            group.Add(new AccountLeaf("Рахунок 2", 2000m));
            group.Add(new AccountLeaf("Рахунок 3", 500m));

            // Act
            decimal total = group.GetTotalBalance();

            // Assert
            Assert.Equal(3500m, total);
        }

        /// <summary>
        /// Перевіряє вкладену структуру: група містить підгрупи, які містять рахунки.
        /// Це демонструє рекурсивну природу Composite паттерну.
        /// </summary>
        [Fact]
        public void AccountGroup_NestedGroups_ReturnsCorrectSum()
        {
            // Arrange
            var mainPortfolio = new AccountGroup("Головний портфель");
            
            // Підгрупа 1: Фіатні рахунки
            var fiatGroup = new AccountGroup("Фіатні рахунки");
            fiatGroup.Add(new AccountLeaf("Зарплатна картка", 25000m));
            fiatGroup.Add(new AccountLeaf("Депозит", 100000m));

            // Підгрупа 2: Крипто-активи
            var cryptoGroup = new AccountGroup("Крипто-активи");
            cryptoGroup.Add(new AccountLeaf("Bitcoin", 150000m));
            cryptoGroup.Add(new AccountLeaf("Ethereum", 45000m));

            // Збираємо гілки до купи
            mainPortfolio.Add(fiatGroup);
            mainPortfolio.Add(cryptoGroup);

            // Act
            decimal total = mainPortfolio.GetTotalBalance();

            // Assert
            Assert.Equal(320000m, total);
        }

        /// <summary>
        /// Перевіряє, що можна видалити компонент із групи.
        /// </summary>
        [Fact]
        public void AccountGroup_RemoveComponent_BalanceUpdates()
        {
            // Arrange
            var group = new AccountGroup("Тестова група");
            var account1 = new AccountLeaf("Рахунок 1", 1000m);
            var account2 = new AccountLeaf("Рахунок 2", 2000m);
            
            group.Add(account1);
            group.Add(account2);

            // Act
            group.Remove(account1);
            decimal total = group.GetTotalBalance();

            // Assert
            Assert.Equal(2000m, total);
        }

        /// <summary>
        /// Перевіряє, що можна додати одну групу до іншої (композиція груп).
        /// </summary>
        [Fact]
        public void AccountGroup_GroupInGroup_FormsBinaryTree()
        {
            // Arrange
            var parent = new AccountGroup("Батьківська група");
            var child1 = new AccountGroup("Дочірня група 1");
            var child2 = new AccountGroup("Дочірня група 2");

            child1.Add(new AccountLeaf("Рахунок 1", 500m));
            child2.Add(new AccountLeaf("Рахунок 2", 1500m));

            parent.Add(child1);
            parent.Add(child2);

            // Act
            decimal total = parent.GetTotalBalance();

            // Assert
            Assert.Equal(2000m, total);
        }

        /// <summary>
        /// Перевіряє, що Display метод не викликає винятків при виведенні дерева.
        /// </summary>
        [Fact]
        public void AccountGroup_Display_DoesNotThrow()
        {
            // Arrange
            var group = new AccountGroup("Тестова група");
            group.Add(new AccountLeaf("Рахунок 1", 1000m));
            group.Add(new AccountLeaf("Рахунок 2", 2000m));

            // Act & Assert
            group.Display(0); // Не повинна викликати винятків
            Assert.True(true);
        }
    }
}
