using System;
using BankingSystem.Domain.Events;
using BankingSystem.Domain.Strategies;

// Контекстний клас, який об'єднує Strategy та Observer
public class TransferService
{
    private ITransferStrategy _strategy;

    // Оголошення події на основі стандартного делегата EventHandler<T>
    public event EventHandler<TransactionEventArgs> OnTransferCompleted;

    public TransferService(ITransferStrategy initialStrategy)
    {
        _strategy = initialStrategy;
    }

    // Зміна алгоритму "на льоту" (Strategy)
    public void SetStrategy(ITransferStrategy newStrategy)
    {
        _strategy = newStrategy;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n[Система]: Алгоритм переказу змінено на '{_strategy.GetTransferType()}'");
        Console.ResetColor();
    }

    public void ExecuteTransfer(string from, string to, decimal amount)
    {
        decimal fee = _strategy.CalculateFee(amount);
        
        Console.WriteLine($"\n--- Виконується {_strategy.GetTransferType()} на {amount} UAH ---");
        
        // Тут могла б бути логіка зміни балансу...

        // Публікація події (Observer)
        // Використовуємо ?.Invoke для безпечного виклику (якщо підписників немає - помилки не буде)
        OnTransferCompleted?.Invoke(this, new TransactionEventArgs(from, to, amount, fee));
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== ДОСЛІДЖЕННЯ: ПОВЕДІНКОВІ ПАТЕРНИ ТА ПОДІЇ (ЛР 11 / СР 11) ===\n");

        // Ініціалізуємо сервіс із початковою стратегією (Внутрішній переказ)
        var transferService = new TransferService(new LocalTransferStrategy());

        // Створюємо наших підписників
        var auditLogger = new AuditLogger();
        var smsNotifier = new UserNotifier();

        // 1. ПІДПИСКА НА ПОДІЇ (Observer)
        transferService.OnTransferCompleted += auditLogger.LogTransaction;
        transferService.OnTransferCompleted += smsNotifier.SendSmsAlert;

        // Транзакція 1: Відпрацюють обидва підписники + локальна стратегія (0 комісії)
        transferService.ExecuteTransfer("UA111", "UA222", 1000m);

        // 2. ЗМІНА АЛГОРИТМУ НА ЛЬОТУ (Strategy)
        // Міняємо на SWIFT. Код TransferService при цьому не змінюється!
        transferService.SetStrategy(new SwiftTransferStrategy());
        transferService.ExecuteTransfer("UA111", "US999", 2000m);

        // 3. ВІДПИСКА ВІД ПОДІЙ (Запобігання Memory Leak)
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n[Система]: Клієнт відключив SMS-сповіщення (Відписка від події -=)");
        Console.ResetColor();
        
        // Відписуємось, щоб Garbage Collector міг безпечно видалити об'єкт, якщо він більше не потрібен
        transferService.OnTransferCompleted -= smsNotifier.SendSmsAlert;

        // Транзакція 3: Тепер SMS не прийде, спрацює тільки AuditLogger
        transferService.ExecuteTransfer("UA111", "US888", 5000m);

        Console.ReadLine();
    }
}