using Core.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ApplicationServices
{
    public class TransactionService
    {
        private readonly ICoreUnitOfWork UnitOfWork;

        public TransactionService(
                   ICoreUnitOfWork unitOfWork
               )
        {
            UnitOfWork = unitOfWork;
        }


    }
}
