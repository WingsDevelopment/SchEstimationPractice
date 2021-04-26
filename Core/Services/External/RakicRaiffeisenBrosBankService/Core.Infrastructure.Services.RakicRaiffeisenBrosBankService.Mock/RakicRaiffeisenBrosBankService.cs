using Core.Domain.Services.External;
using Core.Infrastructure.Services.RakicRaiffeisenBrosBankService.Mock.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Services.RakicRaiffeisenBrosBankService.Mock
{
    public class RakicRaiffeisenBrosBankService : IBankService
    {
        public Task<bool> CheckStatus(string JMBG, string PIN)
        {
            if (PIN == "0000")
            {
                throw new RakicRaiffeisenBrosException("CheckStatus: Error!");
            }

            return Task.FromResult(true);
        }

        public Task<decimal> Deposit(string JMBG, string PIN, decimal Amount)
        {
            if (PIN == "0001")
            {
                throw new RakicRaiffeisenBrosException("CheckStatus: Error!");
            }

            return Task.FromResult(Amount);
        }

        public Task<decimal> Withdraw(string JMBG, string PIN, decimal Amount)
        {
            if (PIN == "0002")
            {
                throw new RakicRaiffeisenBrosException("CheckStatus: Error!");
            }

            return Task.FromResult(Amount);
        }
    }
}
