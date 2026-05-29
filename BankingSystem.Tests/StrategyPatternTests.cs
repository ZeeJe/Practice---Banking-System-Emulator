using System;
using System.Collections.Generic;
using Xunit;
using BankingSystem.Domain.Strategies;

namespace BankingSystem.Tests
{
    /// <summary>
    /// Тести для Strategy паттерну (Практична 11 / Самостійна 11).
    /// Перевіряє коректність обчислення комісій за різними алгоритмами.
    /// </summary>
    public class StrategyPatternTests
    {
        private const decimal TestAmount = 10000m;

        /// <summary>
        /// Перевіряє, що LocalTransferStrategy не нараховує комісію.
        /// </summary>
        [Fact]
        public void LocalTransferStrategy_CalculateFee_ReturnsZero()
        {
            // Arrange
            ITransferStrategy strategy = new LocalTransferStrategy();

            // Act
            decimal fee = strategy.CalculateFee(TestAmount);

            // Assert
            Assert.Equal(0m, fee);
        }

        /// <summary>
        /// Перевіряє опис LocalTransferStrategy.
        /// </summary>
        [Fact]
        public void LocalTransferStrategy_GetTransferType_ReturnsCorrectDescription()
        {
            // Arrange
            ITransferStrategy strategy = new LocalTransferStrategy();

            // Act
            string transferType = strategy.GetTransferType();

            // Assert
            Assert.Equal("Внутрішній переказ", transferType);
        }

        /// <summary>
        /// Перевіряє, що SwiftTransferStrategy нараховує комісію: 5% + 50 UAH.
        /// Формула: 10000 * 0.05 + 50 = 500 + 50 = 550 UAH
        /// </summary>
        [Fact]
        public void SwiftTransferStrategy_CalculateFee_Returns5PercentPlus50()
        {
            // Arrange
            ITransferStrategy strategy = new SwiftTransferStrategy();
            decimal expectedFee = (TestAmount * 0.05m) + 50m; // 550

            // Act
            decimal fee = strategy.CalculateFee(TestAmount);

            // Assert
            Assert.Equal(expectedFee, fee);
        }

        /// <summary>
        /// Перевіряє опис SwiftTransferStrategy.
        /// </summary>
        [Fact]
        public void SwiftTransferStrategy_GetTransferType_ReturnsCorrectDescription()
        {
            // Arrange
            ITransferStrategy strategy = new SwiftTransferStrategy();

            // Act
            string transferType = strategy.GetTransferType();

            // Assert
            Assert.Equal("Міжнародний SWIFT-переказ", transferType);
        }

        /// <summary>
        /// Перевіряє корректність обчислення комісій для різних сум у SwiftTransferStrategy.
        /// </summary>
        [Theory]
        [InlineData(100, 55)]       // 100 * 0.05 + 50 = 55
        [InlineData(1000, 100)]     // 1000 * 0.05 + 50 = 100
        [InlineData(100000, 5050)]  // 100000 * 0.05 + 50 = 5050
        public void SwiftTransferStrategy_CalculateFee_VariousAmounts(int amount, int expectedFee)
        {
            // Arrange
            ITransferStrategy strategy = new SwiftTransferStrategy();

            // Act
            decimal fee = strategy.CalculateFee((decimal)amount);

            // Assert
            Assert.Equal((decimal)expectedFee, fee);
        }

        /// <summary>
        /// Перевіряє, що комісія за мінімальну суму залишається позитивною.
        /// </summary>
        [Fact]
        public void SwiftTransferStrategy_MinimalAmount_StillChargesFee()
        {
            // Arrange
            ITransferStrategy strategy = new SwiftTransferStrategy();
            decimal minAmount = 0.01m;

            // Act
            decimal fee = strategy.CalculateFee(minAmount);

            // Assert
            Assert.True(fee > 0); // Навіть за мінімум буде комісія (50 UAH)
        }

        /// <summary>
        /// Перевіряє, що стратегія є замінна: обидві реалізують ITransferStrategy.
        /// Демонструє принцип замінності (LSP).
        /// </summary>
        [Fact]
        public void StrategySubstitutability_BothImplementITransferStrategy()
        {
            // Arrange & Act
            ITransferStrategy local = new LocalTransferStrategy();
            ITransferStrategy swift = new SwiftTransferStrategy();

            // Assert
            Assert.IsAssignableFrom<ITransferStrategy>(local);
            Assert.IsAssignableFrom<ITransferStrategy>(swift);
        }

        /// <summary>
        /// Інтеграційний тест: демонструє використання Strategy у циклі вибору.
        /// Це класичне використання паттерну замість switch-case.
        /// </summary>
        [Fact]
        public void IntegrationTest_ChooseStrategyDynamically()
        {
            // Arrange
            var strategies = new List<ITransferStrategy>
            {
                new LocalTransferStrategy(),
                new SwiftTransferStrategy()
            };

            // Act & Assert
            foreach (var strategy in strategies)
            {
                decimal fee = strategy.CalculateFee(TestAmount);
                string type = strategy.GetTransferType();

                Assert.False(string.IsNullOrEmpty(type));
                Assert.True(fee >= 0);
            }
        }

        /// <summary>
        /// Перевіряє, що комісія Локального переказу завжди нижча за SWIFT.
        /// </summary>
        [Fact]
        public void CompareStrategies_LocalCheaperThanSwift()
        {
            // Arrange
            ITransferStrategy local = new LocalTransferStrategy();
            ITransferStrategy swift = new SwiftTransferStrategy();

            // Act
            decimal localFee = local.CalculateFee(TestAmount);
            decimal swiftFee = swift.CalculateFee(TestAmount);

            // Assert
            Assert.True(localFee < swiftFee);
        }
    }
}
