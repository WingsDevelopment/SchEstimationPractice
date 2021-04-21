using Common.Utils.Exceptions;
using Core.Domain.Services.External;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Services.RakicRaiffeisenBrosBankService.Mock
{
    public class RakicRaiffeisenBrosBankService : IBankService
    {
        public async Task<bool> CheckStatus(string JMBG, string PIN)
        {
            if (PIN == "0000")
            {
                throw new EstimationPracticeException("PIN nije validan", "CheckStatus: Error!");
            }

            return true;
        }

        public async Task<decimal> Deposit(string JMBG, string PIN, decimal Amount)
        {
            if (PIN == "0000")
            {
                throw new EstimationPracticeException("PIN nije validan", "CheckStatus: Error!");
            }

            return Amount;
        }

        public async Task<decimal> Withdraw(string JMBG, string PIN, decimal Amount)
        {
            if (PIN == "0000")
            {
                throw new EstimationPracticeException("PIN nije validan", "CheckStatus: Error!");
            }

            return Amount;
        }
    }
}
