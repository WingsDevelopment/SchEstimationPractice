using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services.External
{
    public interface IBankService
    {
        Task<bool> CheckStatus(string JMBG, string PIN);
        Task<decimal> Deposit(string JMBG, string PIN, decimal Amount);
        Task<decimal> Withdraw(string JMBG, string PIN, decimal Amount);
    }
}
