using Common.Utils.Exceptions;
using Core.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Applications.WebbClient.Controllers
{
    public class TransactionController : Controller
    {
        private readonly TransactionService TransactionService;
        private readonly ILogger<TransactionController> Logger;
        private readonly string BasicErrorMessage = "Something went wrong :/";

        public TransactionController(TransactionService transactionService,
            ILogger<TransactionController> logger)
        {
            TransactionService = transactionService;
            Logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Transactions(string walletId)
        {
            try
            {
                var transactions = await TransactionService.GetTransactionsByWalletId(walletId);

                return View(transactions);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                if (ex is EstimationPracticeException e) TempData["Error"] = e.EstimationPracticeExceptionMessage;
                else TempData["Error"] = BasicErrorMessage;

                return View();
            }
        }
    }
}
