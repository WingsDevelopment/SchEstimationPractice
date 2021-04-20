﻿using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ApplicationServices.DTOs
{
    public class WalletDTO
    {
        public int Id{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JMBG { get; set; }
        public string Bank { get; set; }
        public int PIN { get; set; }
        public string BankAccount { get; set; }
        public int PASS { get; set; }

        public WalletDTO()
        {

        }

        public WalletDTO(Wallet wallet)
        {
            Id = wallet.Id;
            FirstName = wallet.FirstName;
            LastName = wallet.LastName;
            JMBG = wallet.JMBG;
            Bank = wallet.Bank;
            BankAccount = wallet.BankAccount;
        }
    }
}
