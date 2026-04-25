using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WalletService.Interfaces;
using WalletService.Entities;
using WalletService.DTOs;

namespace WalletService.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            throw new UnauthorizedAccessException("Invalid Token: Missing UserId.");
        }

        [HttpPost("addNew")]
        [Authorize(Roles = "CUSTOMER,MERCHANT")]
        public async Task<IActionResult> AddNewWallet()
        {
            var customerId = GetCurrentUserId();
            var existingWallet = await _walletService.GetById(customerId);
            if (existingWallet != null) return Ok(existingWallet); // Already exists

            var wallet = new EWallet { WalletId = customerId, CurrentBalance = 0 };
            await _walletService.AddWallet(wallet);
            return Ok(wallet);
        }

        [HttpPost("addMoney")]
        [Authorize(Roles = "CUSTOMER,MERCHANT")]
        public async Task<IActionResult> AddMoney([FromQuery] decimal amount, [FromQuery] string remarks = "Deposit")
        {
            var customerId = GetCurrentUserId();
            var wallet = await _walletService.GetById(customerId);
            
            if (wallet == null)
            {
                wallet = new EWallet { WalletId = customerId, CurrentBalance = 0 };
                await _walletService.AddWallet(wallet);
            }

            await _walletService.AddMoney(wallet, amount, remarks);
            return Ok(wallet);
        }

        [HttpPost("payMoney")]
        public async Task<IActionResult> PayMoney([FromQuery] int customerId, [FromQuery] decimal amount, [FromQuery] int orderId = 0)
        {
            var callerId = GetCurrentUserId();
            if (!User.IsInRole("ADMIN") && callerId != customerId)
            {
                return Forbid("You do not have permission to deduct money from this wallet.");
            }

            try
            {
                var wallet = await _walletService.GetById(customerId);
                if (wallet == null) return BadRequest(new { Message = "Wallet not found." });

                await _walletService.Update(wallet, amount, "Order Payment", orderId);
                return Ok(wallet);
            }
            catch (InvalidOperationException ex)
            {
                // Returns HTTP 400 for insufficient funds
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllWallets()
        {
            var wallets = await _walletService.GetWallets();
            return Ok(wallets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var callerId = GetCurrentUserId();
            if (!User.IsInRole("ADMIN") && callerId != id)
            {
                return Forbid("Access denied.");
            }

            var wallet = await _walletService.GetById(id);
            return Ok(wallet);
        }

        [HttpGet("statementsById")]
        public async Task<IActionResult> GetStatementsById([FromQuery] int id)
        {
            var callerId = GetCurrentUserId();
            if (!User.IsInRole("ADMIN") && callerId != id)
            {
                return Forbid("Access denied.");
            }

            var statements = await _walletService.GetStatementsById(id);
            return Ok(statements);
        }

        [HttpGet("statements")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllStatements()
        {
            var statements = await _walletService.GetStatements();
            return Ok(statements);
        }

        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteById([FromQuery] int id)
        {
            await _walletService.DeleteById(id);
            return Ok("Wallet deleted successfully.");
        }

        // Razorpay Endpoints

        [HttpPost("createRechargeOrder")]
        [Authorize(Roles = "CUSTOMER,MERCHANT")]
        public async Task<IActionResult> CreateRechargeOrder([FromBody] RazorpayOrderRequestDto request)
        {
            if (request.Amount <= 0) return BadRequest("Amount must be greater than zero.");
            
            try
            {
                var orderId = await _walletService.CreateRazorpayOrder(request.Amount);
                return Ok(new { RazorpayOrderId = orderId, Amount = request.Amount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to create Razorpay Order", Error = ex.Message });
            }
        }

        [HttpPost("verifyRecharge")]
        [Authorize(Roles = "CUSTOMER,MERCHANT")]
        public async Task<IActionResult> VerifyRecharge([FromQuery] decimal amount, [FromBody] RazorpayVerificationDto verification)
        {
            if (amount <= 0) return BadRequest("Amount must be greater than zero.");

            try
            {
                var isValid = await _walletService.VerifyRazorpayPayment(
                    verification.RazorpayOrderId, 
                    verification.RazorpayPaymentId, 
                    verification.RazorpaySignature);

                if (!isValid)
                {
                    return BadRequest(new { Message = "Invalid payment signature. Payment verification failed." });
                }

                // If valid, add money to the user's wallet
                var customerId = GetCurrentUserId();
                var wallet = await _walletService.GetById(customerId);
                
                if (wallet == null)
                {
                    wallet = new EWallet { WalletId = customerId, CurrentBalance = 0 };
                    await _walletService.AddWallet(wallet);
                }

                await _walletService.AddMoney(wallet, amount, $"Razorpay Deposit: {verification.RazorpayPaymentId}");
                return Ok(new { Message = "Payment successful. Wallet recharged.", Wallet = wallet });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to verify payment or recharge wallet.", Error = ex.Message });
            }
        }
    }
}
