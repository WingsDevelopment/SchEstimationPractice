using Common.Utils.Exceptions;
using Core.ApplicationServices;
using Core.ApplicationServices.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Applications.WebbClient.Controllers
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

                return RedirectToAction("WalletInfo", new { jmbg = wallet.JMBG, pass = wallet.PASS });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                if (ex is EstimationPracticeException e) TempData["Error"] = e.EstimationPracticeExceptionMessage;
                else TempData["Error"] = BasicErrorMessage;

                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> WalletInfo(string jmbg, string pass)
        {

            try
            {
                var wallet = await WalletService.GetWallet(jmbg, pass);

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

        [HttpGet]
        public IActionResult Deposit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(string jmbg, string pass, decimal amount)
        {

            try
            {
                var wallet = await WalletService.Deposit(jmbg, pass, amount);

                TempData["Success"] = "Money is successfully added!";
                return RedirectToAction("WalletInfo", new { jmbg = wallet.JMBG, pass = wallet.PASS });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                if (ex is EstimationPracticeException e) TempData["Error"] = e.EstimationPracticeExceptionMessage;
                else TempData["Error"] = BasicErrorMessage;

                return View();
            }
        }

        [HttpGet]
        public IActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(string jmbg, string pass, decimal amount)
        {

            try
            {
                var wallet = await WalletService.Withdraw(jmbg, pass, amount);

                TempData["Success"] = "Money is successfully withdrawed!";
                return RedirectToAction("WalletInfo", new { jmbg = wallet.JMBG, pass = wallet.PASS });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                if (ex is EstimationPracticeException e) TempData["Error"] = e.EstimationPracticeExceptionMessage;
                else TempData["Error"] = BasicErrorMessage;

                return View();
            }
        }

        [HttpGet]
        public IActionResult Transfer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(string jmbg, string relatedJmbg, string pass, decimal amount)
        {

            try
            {
                var wallet = await WalletService.Transfer(jmbg, relatedJmbg, pass, amount);

                TempData["Success"] = "Money is successfully transfered!: " + amount;
                return RedirectToAction("WalletInfo", new { jmbg = wallet.JMBG, pass = wallet.PASS });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                if (ex is EstimationPracticeException e) TempData["Error"] = e.EstimationPracticeExceptionMessage;
                else TempData["Error"] = BasicErrorMessage;

                return View();
            }
        }

        [HttpGet]
        public IActionResult AdminPanel(string pass) 
        {
            if (!AdminRoleHelper.IsAdmin(pass))
            {
                TempData["Error"] = "U are not admin!";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BlockWallet(string pass, string jmbg)
        {
            if (!AdminRoleHelper.IsAdmin(pass))
            {
                TempData["Error"] = "U are not admin!";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var wallet = await WalletService.BlockWallet(jmbg);

                TempData["Success"] = "Wallet is successfully blocked!";
                return RedirectToAction("AdminPanel", new { pass = pass });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                if (ex is EstimationPracticeException e) TempData["Error"] = e.EstimationPracticeExceptionMessage;
                else TempData["Error"] = BasicErrorMessage;

                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UnBlockWallet(string pass, string jmbg)
        {
            if (!AdminRoleHelper.IsAdmin(pass))
            {
                TempData["Error"] = "U are not admin!";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var wallet = await WalletService.UnBlockWallet(jmbg);

                TempData["Success"] = "Wallet is successfully unblocked!";
                return RedirectToAction("AdminPanel", new { pass = pass });
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
