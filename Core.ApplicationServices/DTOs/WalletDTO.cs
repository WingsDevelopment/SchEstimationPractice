using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ApplicationServices.DTOs
{
    public class WalletDTO
    {
        public string Id{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public string JMBG { get; set; }
        public string Bank { get; set; }
        public string PIN { get; set; }
        public string BankAccount { get; set; }
        public string PASS { get; set; }
        public bool IsBlocked { get; set; }

        public WalletDTO()
        {

        }

        public WalletDTO(Wallet wallet)
        {
            Id = wallet.Id;
            FirstName = wallet.FirstName;
            LastName = wallet.LastName;
            Balance = wallet.Balance;
            JMBG = wallet.JMBG;
            Bank = wallet.Bank;
            BankAccount = wallet.BankAccount;
            PIN = wallet.PIN;
            PASS = wallet.PASS;
            IsBlocked = wallet.IsBlocked;
        }
    }
}
