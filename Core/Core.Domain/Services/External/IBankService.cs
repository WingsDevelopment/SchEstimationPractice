using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services.External
{
    public interface IBankService
    {
        Task<bool> CheckStatus(string JMBG, int PIN);
        Task<decimal> Deposit(string JMBG, int PIN, decimal Amount);
        Task<decimal> Withdraw(string JMBG, int PIN, decimal Amount);
    }
}
