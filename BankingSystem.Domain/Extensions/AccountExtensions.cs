using System.Collections.Generic;
using System.Linq;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Domain.Extensions
{
    // Клас для методів розширення ОБОВ'ЯЗКОВО має бути статичним
    public static class AccountExtensions
    {
        // 1. Виносимо часто вживану логіку фільтрації VIP-клієнтів
        // Ключове слово 'this' перед першим параметром робить метод розширенням
        public static IEnumerable<Account> GetVipAccounts(this IEnumerable<Account> accounts, decimal threshold = 5000m)
        {
            return accounts.Where(a => a.Balance >= threshold);
        }

        // 2. Виносимо логіку агрегації (використання Aggregate з СР 7)
        public static decimal CalculateTotalBalance(this IEnumerable<Account> accounts)
        {
            // Aggregate послідовно акумулює значення (аналог Reduce)
            return accounts.Aggregate(0m, (sum, acc) => sum + acc.Balance);
        }
    }
}