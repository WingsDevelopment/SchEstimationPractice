using Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Core.Domain.Entities
{
    public class Wallet
    {
        public string Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public decimal Balance { get; private set; }
        public string JMBG { get; private set; }
        public string Bank { get; private set; }
        public string PIN { get; private set; }
        public string BankAccount { get; private set; }
        public string PASS { get; private set; }
        public bool IsBlocked { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public Wallet()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
        }

        public Wallet(string firstName,
            string lastName,
            string jmbg,
            string bank,
            string pin,
            string bankAccount,
            string pass,
            decimal balance = 0) : this()
        {
            Balance = balance;
            SetFirstName(firstName);
            SetLastName(lastName);
            SetJMBG(jmbg);
            SetBank(bank);
            SetPin(pin);
            SetBankAccount(bankAccount);
            SetPASS(pass);
        }

        public void Withdraw(decimal amount)
        {
            if (amount < 0)
            {
                throw new WalletEntityException("Amount mora biti pozitivan!", "Withdraw: Amount must be positive number.");
            }

            if (Balance - amount < 0) 
            {
                throw new WalletEntityException("Nemate dovoljno kredita", "Withdraw: Not enough credit."); 
            }


            Balance -= amount;
        }
        public void Deposit(decimal amount)
        {
            if (amount < 0)
            {
                throw new WalletEntityException("Amount mora biti pozitivan!", "Withdraw: Amount must be positive number.");
            }

            Balance += amount;
        }

        public void SetFirstName(string firstName)
        {
            if (String.IsNullOrWhiteSpace(firstName))
            {
                throw new WalletEntityException("Ime je obavezno!", "SetFirstName: First name can't be null.");
            }

            FirstName = firstName;
        }

        public void SetLastName(string lastName)
        {
            if (String.IsNullOrWhiteSpace(lastName))
            {
                throw new WalletEntityException("Prezime je obavezno!", "SetLastName: Last name can't be null.");
            }

            LastName = lastName;
        }
        public void SetJMBG(string jmbg)
        {
            if (String.IsNullOrWhiteSpace(jmbg))
            {
                throw new WalletEntityException("JMBG je obavezan!", "SetJMBG: JMBG can't be null.");
            }

            if (jmbg.Length != 13)
            {
                throw new WalletEntityException("JMBG nije validan!", "SetJMBG: JMBG is invalid.");
            }
            string dateStr = jmbg.Substring(0, 7);
            string prefix = "1";
            if (dateStr[4] != '9') prefix = "2";
            dateStr = dateStr.Insert(4, prefix);

            DateTime birthDate;
            if (DateTime.TryParseExact(dateStr
                                        , "ddMMyyyy"
                                        , CultureInfo.InvariantCulture
                                        , DateTimeStyles.None
                                        , out birthDate))

            if (birthDate >= DateTime.Now.AddYears(-18))
            {
                throw new WalletEntityException("Morate biti punoletni da biste se prijavili!", "SetJMBG: User is not adult.");
            }

            int regionOfBithNumber = 0;
            if (!int.TryParse(jmbg.Substring(7, 2), out regionOfBithNumber)) 
                throw new WalletEntityException("JMBG nije validan!", "SetJMBG: JMBG is invalid");
            if (regionOfBithNumber < 70 || regionOfBithNumber > 99)
                throw new WalletEntityException("Morate biti Srbin!", "SetJMBG: User is not Serbian");

            JMBG = jmbg;
        }
        public void SetBank(string bank)
        {
            if (String.IsNullOrWhiteSpace(bank))
            {
                throw new WalletEntityException("Ime banke je obavezno!", "SetBank: Bank can't be null.");
            }

            Bank = bank;
        }
        public void SetPin(string pin)
        {
            if (pin.Length != 4)
            {
                throw new WalletEntityException("Pin je neispravnog formata!", "SetPin: Invalid pin format.");
            }

            PIN = pin;
        }
        public void SetBankAccount(string bankAccount)
        {
            if (String.IsNullOrWhiteSpace(bankAccount))
            {
                throw new WalletEntityException("Racun je obavezan!", "SetBankAccount: BankAccount can't be null.");
            }

            BankAccount = bankAccount;
        }
        public void SetPASS(string pass)
        {
            if (pass.Length != 6)
            {
                throw new WalletEntityException("PASS je neispravnog formata!", "SetPASS: Invalid PASS format.");
            }

            PASS = pass;
        }

        public void Block()
        {
            if (IsBlocked) throw new WalletEntityException("Wallet je vec blokiran!", "Block: Already blocked.");

            IsBlocked = true;
        }
        public void UnBlock()
        {
            if (!IsBlocked) throw new WalletEntityException("Wallet je vec unblokiran!", "UnBlock: Already unblocked.");

            IsBlocked = false;
        }
    }
}
