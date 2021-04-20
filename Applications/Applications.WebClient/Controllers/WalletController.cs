using Common.Utils.Exceptions;
using Core.ApplicationServices;
using Core.ApplicationServices.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Applications.WebClient.Controllers
{
    public class WalletController : Controller
    {
        private readonly WalletService WalletService;
        private readonly ILogger<WalletController> Logger;
        private readonly string BasicErrorMessage = "Something went wrong :/";

        public WalletController(WalletService walletService,
            ILogger<WalletController> logger)
        {
            WalletService = walletService;
            Logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(WalletDTO walletDTO)
        {
            try
            {
                var wallet = await WalletService.CreateWallet(walletDTO);

                return View(wallet);
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
