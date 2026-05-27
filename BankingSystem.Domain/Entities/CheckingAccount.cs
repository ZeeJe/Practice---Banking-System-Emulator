using System;

namespace BankingSystem.Domain.Entities
{
    // Поточний рахунок успадковує спільну поведінку від абстрактного Account
    public class CheckingAccount : Account
    {
        public CheckingAccount(string ownerName, decimal initialBalance) 
            : base(ownerName, initialBalance) 
        {
        }

        public override string GetAccountDetails()
        {
            return $"[Поточний рахунок] № {AccountNumber} (Власник: {OwnerName})";
        }
    }
}