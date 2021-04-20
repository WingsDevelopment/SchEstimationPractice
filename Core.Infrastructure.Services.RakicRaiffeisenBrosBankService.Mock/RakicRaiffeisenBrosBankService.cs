using Common.Utils.Exceptions;
using Core.Domain.Services.External;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Services.RakicRaiffeisenBrosBankService.Mock
{
    public class RakicRaiffeisenBrosBankService : IRakicRaiffeisenBrosBankService
    {
        public async Task<bool> CheckStatus(string JMBG, int PIN)
        {
            if (PIN == 0)
            {
                throw new EstimationPracticeException("PIN nije validan", "CheckStatus: Error!");
            }

            return true;
        }

        public async Task<decimal> Deposit(string JMBG, int PIN, decimal Amount)
        {
            if (PIN == 0)
            {
                throw new EstimationPracticeException("PIN nije validan", "CheckStatus: Error!");
            }

            return Amount;
        }

        public async Task<decimal> Withdraw(string JMBG, int PIN, decimal Amount)
        {
            if (PIN == 0)
            {
                throw new EstimationPracticeException("PIN nije validan", "CheckStatus: Error!");
            }

            return Amount;
        }
    }
}
