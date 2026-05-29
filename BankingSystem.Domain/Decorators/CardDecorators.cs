using System;
using BankingSystem.Domain.Common;

namespace BankingSystem.Domain.Decorators
{
    // Базовий інтерфейс компонента
    public interface IBankCard
    {
        string GetDescription();
    }

    // Конкретний базовий компонент
    public class StandardCard : IBankCard
    {
        public string GetDescription() => "Стандартна банківська картка";
    }

    // Базовий декоратор (містить посилання на обгорнутий об'єкт)
    public abstract class CardDecorator : IBankCard
    {
        protected IBankCard _card;
        
        public CardDecorator(IBankCard card)
        {
            _card = card;
        }
        
        public virtual string GetDescription() => _card.GetDescription();
    }

    // Конкретний декоратор 1: Додає кешбек
    public class CashbackDecorator : CardDecorator
    {
        public CashbackDecorator(IBankCard card) : base(card) { }
        
        // Повністю позбулися хардкоду тексту
        public override string GetDescription() => base.GetDescription() + BankConstants.CashbackSuffix;
    }

    // Конкретний декоратор 2: Додає безконтактну оплату
    public class PayPassDecorator : CardDecorator
    {
        public PayPassDecorator(IBankCard card) : base(card) { }
        
        public override string GetDescription() => base.GetDescription() + " + PayPass (NFC)";
    }
}