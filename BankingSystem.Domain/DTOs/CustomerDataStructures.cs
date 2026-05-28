using System;
using System.Collections.Generic;
using System.Linq;

namespace BankingSystem.Domain.DTOs
{
    // ==========================================
    // ДУЖЕ ЖОРСТКІ ДОМЕННІ МОДЕЛІ (Із зацикленістю)
    // ==========================================
    public class DomainCustomer
    {
        public string FullName { get; set; } = string.Empty;
        // Клієнт має список рахунків
        public List<DomainAccount> Accounts { get; set; } = new();
    }

    public class DomainAccount
    {
        public string AccountId { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        // Циклічне посилання: Рахунок посилається назад на Клієнта!
        public DomainCustomer Owner { get; set; } = null!;
    }

    // ==========================================
    // ЧИСТІ DTO ДЛЯ СЕРІАЛІЗАЦІЇ В JSON
    // ==========================================
    public class CustomerDto
    {
        public string FullName { get; set; } = string.Empty;
        // Зберігаємо вкладені DTO рахунків
        public List<AccountDto> Accounts { get; set; } = new();
    }

    public class AccountDto
    {
        public string AccountId { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        // Цикл РОЗІРВАНО. Тут немає посилання на Owner, JSON буде чистим!
    }

    // ==========================================
    // МАППЕР (Конвертація Домен <-> DTO)
    // ==========================================
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(DomainCustomer customer)
        {
            return new CustomerDto
            {
                FullName = customer.FullName,
                Accounts = customer.Accounts.Select(a => new AccountDto
                {
                    AccountId = a.AccountId,
                    Balance = a.Balance
                }).ToList()
            };
        }

        public static DomainCustomer ToDomain(CustomerDto dto)
        {
            var customer = new DomainCustomer { FullName = dto.FullName };
            
            // Відновлюємо циклічні посилання в пам'яті вручну
            customer.Accounts = dto.Accounts.Select(a => new DomainAccount
            {
                AccountId = a.AccountId,
                Balance = a.Balance,
                Owner = customer // Прив'язуємо назад на створеного клієнта!
            }).ToList();

            return customer;
        }
    }
}