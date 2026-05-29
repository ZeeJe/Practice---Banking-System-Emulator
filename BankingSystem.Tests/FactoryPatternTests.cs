using System;
using Xunit;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Factories;

namespace BankingSystem.Tests
{
    /// <summary>
    /// Тести для Factory Method паттерну (Практична 10 / Самостійна 10).
    /// Перевіряє коректність динамічного створення об'єктів різних типів.
    /// </summary>
    public class FactoryPatternTests
    {
        private const string TestOwner = "Іван Петренко";
        private const decimal TestBalance = 5000m;

        /// <summary>
        /// Перевіряє, що фабрика створює CheckingAccount при типі "checking".
        /// </summary>
        [Fact]
        public void CreateAccount_CheckingType_ReturnsCheckingAccount()
        {
            // Act
            var account = AccountFactory.CreateAccount("checking", TestOwner, TestBalance);

            // Assert
            Assert.NotNull(account);
            Assert.IsType<CheckingAccount>(account);
            Assert.Equal(TestOwner, account.OwnerName);
            Assert.Equal(TestBalance, account.Balance);
        }

        /// <summary>
        /// Перевіряє, що фабрика створює Deposit при типі "deposit".
        /// </summary>
        [Fact]
        public void CreateAccount_DepositType_ReturnsDeposit()
        {
            // Act
            var account = AccountFactory.CreateAccount("deposit", TestOwner, TestBalance);

            // Assert
            Assert.NotNull(account);
            Assert.IsType<Deposit>(account);
            Assert.Equal(TestOwner, account.OwnerName);
            Assert.Equal(TestBalance, account.Balance);
        }

        /// <summary>
        /// Перевіряє, що фабрика розрізняє тип незалежно від регістру.
        /// </summary>
        [Theory]
        [InlineData("CHECKING")]
        [InlineData("Checking")]
        [InlineData("ChEcKiNg")]
        public void CreateAccount_CaseInsensitive_ReturnsCorrectType(string type)
        {
            // Act
            var account = AccountFactory.CreateAccount(type, TestOwner, TestBalance);

            // Assert
            Assert.IsType<CheckingAccount>(account);
        }

        /// <summary>
        /// Перевіряє, що фабрика викидає ArgumentException при невідомому типі.
        /// </summary>
        [Theory]
        [InlineData("savings")]
        [InlineData("credit")]
        [InlineData("unknown")]
        [InlineData("")]
        public void CreateAccount_InvalidType_ThrowsArgumentException(string invalidType)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                AccountFactory.CreateAccount(invalidType, TestOwner, TestBalance)
            );

            Assert.Contains("Невідомий тип рахунку", exception.Message);
        }

        /// <summary>
        /// Перевіряє, що всі створені рахунки мають унікальні номери.
        /// </summary>
        [Fact]
        public void CreateAccount_MultipleAccounts_HasUniqueAccountNumbers()
        {
            // Act
            var account1 = AccountFactory.CreateAccount("checking", "Іван", 1000m);
            var account2 = AccountFactory.CreateAccount("checking", "Марія", 2000m);
            var account3 = AccountFactory.CreateAccount("deposit", "Петро", 3000m);

            // Assert
            Assert.NotEqual(account1.AccountNumber, account2.AccountNumber);
            Assert.NotEqual(account2.AccountNumber, account3.AccountNumber);
            Assert.NotEqual(account1.AccountNumber, account3.AccountNumber);
        }

        /// <summary>
        /// Перевіряє, що Deposit рахунок має установлений термін та ставку.
        /// </summary>
        [Fact]
        public void CreateAccount_DepositType_HasEndDateAndRate()
        {
            // Act
            var account = AccountFactory.CreateAccount("deposit", TestOwner, TestBalance);
            var deposit = account as Deposit;

            // Assert
            Assert.NotNull(deposit);
            Assert.True(deposit!.EndDate > DateTime.Now);
            Assert.True(deposit.InterestRate > 0);
        }

        /// <summary>
        /// Перевіряє, що фабрика дотримується принципу Single Responsibility:
        /// вона тільки вибирає тип, логіка ініціалізації у конструкторах.
        /// </summary>
        [Fact]
        public void CreateAccount_CheckingType_HasInitialBalance()
        {
            // Act
            decimal initialBalance = 7500m;
            var account = AccountFactory.CreateAccount("checking", TestOwner, initialBalance);

            // Assert
            Assert.Equal(initialBalance, account.Balance);
        }
    }
}
