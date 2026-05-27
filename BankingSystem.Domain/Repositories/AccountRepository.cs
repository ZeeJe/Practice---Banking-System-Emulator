using System;
using System.Collections.Generic;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Domain.Repositories
{
    // Generics з обмеженням: працюємо тільки з класами, що наслідують Account
    public class AccountRepository<T> where T : Account
    {
        private readonly List<T> _items = new List<T>();

        // Додавання елементу в репозиторій
        public void Add(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _items.Add(item);
        }

        // =========================================================
        // САМОСТІЙНА 5: Узагальнені алгоритми та делегати (Func, Action)
        // =========================================================

        // 1. Метод ForEach (використовує Action для виконання операції над кожним елементом)
        public void ForEach(Action<T> action)
        {
            foreach (var item in _items)
            {
                action(item);
            }
        }

        // 2. Метод Map (використовує Func для трансформації списку T в список іншого типу TResult)
        public List<TResult> Map<TResult>(Func<T, TResult> selector)
        {
            var result = new List<TResult>();
            foreach (var item in _items)
            {
                result.Add(selector(item));
            }
            return result;
        }

        // 3. Метод Reduce (Агрегація даних: рахує загальну суму, середнє тощо за допомогою Func)
        public TAccumulate Reduce<TAccumulate>(TAccumulate initialValue, Func<TAccumulate, T, TAccumulate> aggregator)
        {
            TAccumulate accumulator = initialValue;
            foreach (var item in _items)
            {
                accumulator = aggregator(accumulator, item);
            }
            return accumulator;
        }
    }
}