using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BankingSystem.Domain.DTOs;

namespace BankingSystem.Domain.Services
{
    public class CustomerService
    {
        private readonly JsonStorageService _storageService;
        private readonly string _dataDirectory;
        private readonly string _customersFilePath;

        public CustomerService(JsonStorageService storageService, string? dataDirectory = null)
        {
            _storageService = storageService;
            
            // Якщо директорія не передана, використовуємо корінь Solution
            _dataDirectory = dataDirectory ?? GetRootDirectory();
            _customersFilePath = Path.Combine(_dataDirectory, "bank_customers.json");
        }

        /// <summary>
        /// Знаходить корінь Solution (папка з .slnx файлом)
        /// </summary>
        private string GetRootDirectory()
        {
            var currentDir = AppContext.BaseDirectory;
            
            while (!string.IsNullOrEmpty(currentDir))
            {
                var slnxFiles = Directory.GetFiles(currentDir, "*.slnx");
                if (slnxFiles.Length > 0)
                {
                    return currentDir;
                }

                var parentDir = Directory.GetParent(currentDir);
                currentDir = parentDir?.FullName;
            }

            // Якщо не знайдено - повертаємо поточну директорію
            return AppContext.BaseDirectory;
        }

        /// <summary>
        /// Завантажує всіх клієнтів з JSON
        /// </summary>
        public List<CustomerDto> GetAllCustomers()
        {
            try
            {
                if (!File.Exists(_customersFilePath))
                {
                    Console.WriteLine($"[CustomerService]: Файл не знайдено: {_customersFilePath}");
                    return GenerateSeedData();
                }

                var customers = _storageService.LoadFromFile<List<CustomerDto>>(_customersFilePath);
                return customers ?? GenerateSeedData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CustomerService]: Помилка при завантаженні клієнтів: {ex.Message}");
                return GenerateSeedData();
            }
        }

        /// <summary>
        /// Додає нового клієнта
        /// </summary>
        public void AddCustomer(CustomerDto customer)
        {
            var customers = GetAllCustomers();
            customers.Add(customer);
            SaveCustomers(customers);
        }

        /// <summary>
        /// Оновлює дані клієнта
        /// </summary>
        public void UpdateCustomer(string fullName, CustomerDto updatedCustomer)
        {
            var customers = GetAllCustomers();
            var existing = customers.FirstOrDefault(c => c.FullName == fullName);
            
            if (existing != null)
            {
                var index = customers.IndexOf(existing);
                customers[index] = updatedCustomer;
                SaveCustomers(customers);
            }
        }

        /// <summary>
        /// Видаляє клієнта
        /// </summary>
        public void DeleteCustomer(string fullName)
        {
            var customers = GetAllCustomers();
            customers.RemoveAll(c => c.FullName == fullName);
            SaveCustomers(customers);
        }

        /// <summary>
        /// Зберігає клієнтів у JSON
        /// </summary>
        private void SaveCustomers(List<CustomerDto> customers)
        {
            try
            {
                _storageService.SaveToFile(_customersFilePath, customers);
                Console.WriteLine($"[CustomerService]: Клієнти збережено у {_customersFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CustomerService]: Помилка при збереженні: {ex.Message}");
            }
        }

        /// <summary>
        /// Генерує тестові дані, якщо файл не існує
        /// </summary>
        private List<CustomerDto> GenerateSeedData()
        {
            var seedData = new List<CustomerDto>
            {
                new CustomerDto
                {
                    FullName = "Іван Петренко",
                    Accounts = new List<AccountDto>
                    {
                        new AccountDto { AccountId = "UA-FIAT-001", Balance = 45000m },
                        new AccountDto { AccountId = "UA-CRYPTO-001", Balance = 150000m }
                    }
                },
                new CustomerDto
                {
                    FullName = "Марія Коваленко",
                    Accounts = new List<AccountDto>
                    {
                        new AccountDto { AccountId = "UA-FIAT-002", Balance = 25500m }
                    }
                },
                new CustomerDto
                {
                    FullName = "Олег Шевченко",
                    Accounts = new List<AccountDto>
                    {
                        new AccountDto { AccountId = "UA-FIAT-003", Balance = 112000m },
                        new AccountDto { AccountId = "UA-CRYPTO-003", Balance = 75000m }
                    }
                }
            };

            SaveCustomers(seedData);
            return seedData;
        }
    }
}
