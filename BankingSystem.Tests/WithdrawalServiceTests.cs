using System;
using Xunit;
using Moq;
using BankingSystem.Domain.Services;

namespace BankingSystem.Tests
{
    public class WithdrawalServiceTests
    {
        [Fact]
        public void Withdraw_ValidAmount_DecreasesBalanceAndSendsNotification()
        {
            // Arrange
            decimal initialBalance = 1000m;
            // Мокаємо саме наш новий відокремлений інтерфейс
            var mockNotifier = new Mock<IWithdrawalNotificationService>(); 
            mockNotifier.Setup(n => n.SendSms(It.IsAny<string>())).Returns(true);
            
            var service = new WithdrawalService(mockNotifier.Object, initialBalance);

            // Act
            service.Withdraw(200m);

            // Assert
            Assert.Equal(800m, service.Balance);
            mockNotifier.Verify(n => n.SendSms(It.Is<string>(msg => msg.Contains("знято 200"))), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public void Withdraw_InvalidAmount_ThrowsArgumentException(decimal invalidAmount)
        {
            // Arrange
            var mockNotifier = new Mock<IWithdrawalNotificationService>();
            var service = new WithdrawalService(mockNotifier.Object, 1000m);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => service.Withdraw(invalidAmount));
            mockNotifier.Verify(n => n.SendSms(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Withdraw_AmountGreaterThanBalance_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockNotifier = new Mock<IWithdrawalNotificationService>();
            var service = new WithdrawalService(mockNotifier.Object, 1000m);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => service.Withdraw(1500m));
            Assert.Equal(1000m, service.Balance);
        }
    }
}