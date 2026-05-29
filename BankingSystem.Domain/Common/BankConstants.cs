namespace BankingSystem.Domain.Common
{
    /// <summary>
    /// Константи для конфігурації банківської системи. Центральне місце для всіх магічних чисел.
    /// </summary>
    public static class BankConstants
    {
        // ==========================================
        // КОМІСІЇ ТА СТАВКИ
        // ==========================================
        public const decimal StandardFeeRate = 0.01m;          // 1%
        public const decimal VipFeeRate = 0.00m;               // 0%
        public const decimal InternationalFeeRate = 0.05m;     // 5%
        public const decimal InternationalFixedFee = 50.00m;   // 50 UAH фіксовано
        public const decimal CashbackPercentage = 0.05m;       // 5% кешбек за покупки
        
        public const decimal DefaultNationalBankRate = 12.5m;  // 12.5% базова ставка НБУ

        // ==========================================
        // ЛІМІТИ ТА УМОВИ
        // ==========================================
        public const decimal MinWithdrawalAmount = 0.01m;
        public const decimal DailyTransferLimit = 50000m;
        public const int DefaultDepositTermYears = 1;          // Термін депозиту за замовчуванням

        // ==========================================
        // RETRY POLICY (Самостійна 8)
        // ==========================================
        public const int RetryMaxAttempts = 3;                 // Максимум 3 спроби
        public const int RetryBaseDelayMs = 200;               // Базова затримка 200 мс
        public const int RetryExponentialBase = 2;             // Експоненціальна база: 2^(attempt-1)

        // ==========================================
        // ФОРМАТУВАННЯ ТА РЯДКИ
        // ==========================================
        public const string CurrencySymbol = "UAH";
        public const string DefaultDateTimeFormat = "dd.MM.yyyy HH:mm:ss";
        
        // Декоратори (Практична 12)
        public const string CashbackSuffix = " + Кешбек 5%";
        public const string PayPassSuffix = " + PayPass (NFC)";

        // ==========================================
        // ПОВІДОМЛЕННЯ ТА ЛОГУВАННЯ
        // ==========================================
        public const string SmsNotificationPrefix = "[SMS Сповіщення]";
        public const string AuditLogPrefix = "[Audit Log]";
    }
}