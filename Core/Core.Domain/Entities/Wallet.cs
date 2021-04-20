using Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Core.Domain.Entities
{
    public class Wallet
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string JMBG { get; private set; }
        public string Bank { get; private set; }
        public int PIN { get; private set; }
        public string BankAccount { get; private set; }
        public int PASS { get; private set; }

        //q: zbog ef-a ?
        public Wallet()
        {

        }

        public Wallet(string firstName, string lastName, string jmbg, string bank, int pin, string bankAccount, int pass)
        {
            SetFirstName(firstName);
            SetLastName(lastName);
            SetJMBG(jmbg);
            SetBank(bank);
            SetPin(pin);
            SetBankAccount(bankAccount);
            SetPASS(pass);
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

            if (jmbg.Length < 7)
            {
                throw new WalletEntityException("JMBG nije validan!", "SetJMBG: JMBG is invalid.");
            }
            //todo: proveriti el ovako radi

            string dateStr = jmbg.Substring(0, 7);
            DateTime birthDate;
            if (DateTime.TryParseExact(dateStr
                                        , "ddMMyyy"
                                        , CultureInfo.InvariantCulture
                                        , DateTimeStyles.None
                                        , out birthDate))

            if (birthDate >= DateTime.Now.AddYears(-18))
            {
                throw new WalletEntityException("Morate biti punoletni da biste se prijavili!", "SetJMBG: User is not adult.");
            }

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
        public void SetPin(int pin)
        {
            if (pin < 1000 || pin > 9999)
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
        public void SetPASS(int pass)
        {
            if (pass < 100000 || pass > 999999)
            {
                throw new WalletEntityException("PASS je neispravnog formata!", "SetPASS: Invalid PASS format.");
            }

            PASS = pass;
        }
    }
}
