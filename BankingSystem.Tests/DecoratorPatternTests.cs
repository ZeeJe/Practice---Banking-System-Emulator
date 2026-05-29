using System;
using Xunit;
using BankingSystem.Domain.Decorators;

namespace BankingSystem.Tests
{
    /// <summary>
    /// Тести для Decorator паттерну (Практична 12 / Самостійна 12).
    /// Перевіряє коректність композиції функціональності картки через декоратори.
    /// </summary>
    public class DecoratorPatternTests
    {
        /// <summary>
        /// Перевіряє, що базова картка повертає правильний опис.
        /// </summary>
        [Fact]
        public void StandardCard_GetDescription_ReturnsBaseDescription()
        {
            // Arrange
            IBankCard card = new StandardCard();

            // Act
            string description = card.GetDescription();

            // Assert
            Assert.Equal("Стандартна банківська картка", description);
        }

        /// <summary>
        /// Перевіряє, що кешбек декоратор додає фунціональність.
        /// </summary>
        [Fact]
        public void CashbackDecorator_GetDescription_AddsCashbackInfo()
        {
            // Arrange
            IBankCard card = new StandardCard();
            card = new CashbackDecorator(card);

            // Act
            string description = card.GetDescription();

            // Assert
            Assert.Contains("Стандартна банківська картка", description);
            Assert.Contains("Кешбек", description);
        }

        /// <summary>
        /// Перевіряє, що PayPass декоратор додає фунціональність.
        /// </summary>
        [Fact]
        public void PayPassDecorator_GetDescription_AddsPayPassInfo()
        {
            // Arrange
            IBankCard card = new StandardCard();
            card = new PayPassDecorator(card);

            // Act
            string description = card.GetDescription();

            // Assert
            Assert.Contains("Стандартна банківська картка", description);
            Assert.Contains("PayPass", description);
            Assert.Contains("NFC", description);
        }

        /// <summary>
        /// Перевіряє, що кілька декораторів можна комбінувати (як матрьошка).
        /// Порядок має значення: Cashback спочатку, потім PayPass.
        /// </summary>
        [Fact]
        public void MultipleDecorators_CashbackThenPayPass_CombinesCorrectly()
        {
            // Arrange
            IBankCard card = new StandardCard();
            card = new CashbackDecorator(card);
            card = new PayPassDecorator(card);

            // Act
            string description = card.GetDescription();

            // Assert
            Assert.Contains("Стандартна банківська картка", description);
            Assert.Contains("Кешбек", description);
            Assert.Contains("PayPass", description);
        }

        /// <summary>
        /// Перевіряє, що порядок декораторів впливає на результат.
        /// PayPass спочатку, потім Cashback.
        /// </summary>
        [Fact]
        public void MultipleDecorators_PayPassThenCashback_CombinesCorrectly()
        {
            // Arrange
            IBankCard card = new StandardCard();
            card = new PayPassDecorator(card);
            card = new CashbackDecorator(card);

            // Act
            string description = card.GetDescription();

            // Assert
            Assert.Contains("Стандартна банківська картка", description);
            Assert.Contains("Кешбек", description);
            Assert.Contains("PayPass", description);
        }

        /// <summary>
        /// Перевіряє, що можна застосувати один декоратор кілька разів.
        /// (Хоча в реальності це не логічно, але демонструє гнучкість)
        /// </summary>
        [Fact]
        public void MultipleDecorators_SameDecoratorTwice_StacksCorrectly()
        {
            // Arrange
            IBankCard card = new StandardCard();
            card = new CashbackDecorator(card);
            card = new CashbackDecorator(card); // Двічі кешбек

            // Act
            string description = card.GetDescription();

            // Assert
            // Очікуємо, що опис буде містити два рази "Кешбек"
            Assert.Equal(description.Split("Кешбек").Length - 1, 2);
        }

        /// <summary>
        /// Перевіряє, що декоратори не змінюють базовий об'єкт.
        /// (Демонструє, що декоратори обгортають, а не модифікують)
        /// </summary>
        [Fact]
        public void Decorator_OriginalObjectUnchanged_AfterDecoration()
        {
            // Arrange
            IBankCard originalCard = new StandardCard();
            string originalDescription = originalCard.GetDescription();

            // Act
            IBankCard decoratedCard = new CashbackDecorator(originalCard);
            _ = decoratedCard.GetDescription(); // Використовуємо декоровану картку

            // Assert
            Assert.Equal(originalDescription, originalCard.GetDescription());
        }

        /// <summary>
        /// Інтеграційний тест: демонструє побудову премійної картки через декоратори.
        /// Це має виглядати як реальний бізнес-сценарій.
        /// </summary>
        [Fact]
        public void IntegrationTest_BuildPremiumCard_ContainsAllFeatures()
        {
            // Arrange - побудова премійної картки
            IBankCard premiumCard = new StandardCard();
            premiumCard = new CashbackDecorator(premiumCard);
            premiumCard = new PayPassDecorator(premiumCard);

            // Act
            string description = premiumCard.GetDescription();

            // Assert
            Assert.Contains("Стандартна", description);
            Assert.Contains("Кешбек 5%", description);
            Assert.Contains("PayPass", description);
            Assert.Contains("NFC", description);
        }
    }
}
