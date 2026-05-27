namespace BankingSystem.Domain.Interfaces
{
    // Строгий контракт без прив'язки до жодного базового класу
    public interface INotificationService
    {
        void SendNotification(string message);
    }
}   