using System;
using System.Collections.Generic;
using System.Linq;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Domain.Extensions
{
    /// <summary>
    /// Методи розширення для колекцій Account. Виносять часто вживану LINQ логіку.
    /// (Самостійна 7: Складні LINQ запити)
    /// </summary>
    public static class AccountExtensions
    {
        /// <summary>
        /// Фільтрує рахунки за мінімальним балансом (VIP клієнти).
        /// </summary>
        /// <param name="threshold">Мінімальний баланс для VIP статусу (за умовч. 5000 UAH)</param>
        public static IEnumerable<Account> GetVipAccounts(this IEnumerable<Account> accounts, decimal threshold = 5000m)
        {
            return accounts.Where(a => a.Balance >= threshold);
        }

        /// <summary>
        /// Розраховує загальний баланс всіх рахунків у колекції (Aggregate).
        /// </summary>
        public static decimal CalculateTotalBalance(this IEnumerable<Account> accounts)
        {
            // Aggregate послідовно акумулює значення (аналог Reduce в функціональному програмуванні)
            return accounts.Aggregate(0m, (sum, acc) => sum + acc.Balance);
        }

        /// <summary>
        /// Групує рахунки за власником (GroupBy).
        /// </summary>
        /// <returns>Словник, де ключ — ім'я власника, значення — список його рахунків</returns>
        public static Dictionary<string, List<Account>> GroupByOwner(this IEnumerable<Account> accounts)
        {
            return accounts
                .GroupBy(a => a.OwnerName)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>
        /// Групує рахунки за типом (Checking vs Deposit) та розраховує суми.
        /// </summary>
        /// <returns>Словник типу рахунку та його загального балансу</returns>
        public static Dictionary<string, decimal> GroupByTypeWithSum(this IEnumerable<Account> accounts)
        {
            return accounts
                .GroupBy(a => a.GetType().Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Aggregate(0m, (sum, acc) => sum + acc.Balance)
                );
        }

        /// <summary>
        /// Розраховує статистику по всім рахункам (Aggregate з кортежем).
        /// </summary>
        /// <returns>Кортеж (загальний баланс, кількість рахунків, середній баланс)</returns>
        public static (decimal TotalBalance, int Count, decimal AverageBalance) GetAccountStats(
            this IEnumerable<Account> accounts)
        {
            var stats = accounts.Aggregate(
                (TotalBalance: 0m, Count: 0),
                (acc, account) => (
                    acc.TotalBalance + account.Balance,
                    acc.Count + 1
                )
            );

            decimal avg = stats.Count > 0 ? stats.TotalBalance / stats.Count : 0m;
            return (stats.TotalBalance, stats.Count, avg);
        }

        /// <summary>
        /// Знаходить рахунки з балансом в певному діапазоні.
        /// </summary>
        public static IEnumerable<Account> GetAccountsInRange(
            this IEnumerable<Account> accounts,
            decimal minBalance,
            decimal maxBalance)
        {
            return accounts.Where(a => a.Balance >= minBalance && a.Balance <= maxBalance);
        }

        /// <summary>
        /// Видає звіт про топ-N рахунків за балансом.
        /// </summary>
        public static List<(string Owner, decimal Balance, string Details)> GetTopAccountsByBalance(
            this IEnumerable<Account> accounts,
            int topCount = 5)
        {
            return accounts
                .OrderByDescending(a => a.Balance)
                .Take(topCount)
                .Select(a => (a.OwnerName, a.Balance, a.GetAccountDetails()))
                .ToList();
        }

        /// <summary>
        /// З'єднує рахунки з можливими угодами/пропозиціями (Join - демонстрація).
        /// </summary>
        public static List<(Account Account, string Offer)> JoinAccountsWithOffers(
            this IEnumerable<Account> accounts,
            IEnumerable<(decimal ThresholdBalance, string Offer)> offers)
        {
            return accounts
                .Join(
                    offers,
                    a => a.Balance >= offers.FirstOrDefault().ThresholdBalance,
                    o => true,
                    (a, o) => (a, o.Offer)
                )
                .ToList();
        }
    }
}