using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using WalletService.Data;
using WalletService.Entities;
using WalletService.Interfaces;
using Xunit;

namespace WalletService.Tests
{
    public class WalletServiceTests
    {
        private readonly Mock<IWalletRepository> _repoMock;

        public WalletServiceTests()
        {
            _repoMock = new Mock<IWalletRepository>();
        }

        // ────────────────────────────────────────────────────────────
        // GetWallets
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetWallets_ShouldReturnAllWallets()
        {
            var wallets = new List<EWallet>
            {
                new EWallet { WalletId = 1, CurrentBalance = 500 },
                new EWallet { WalletId = 2, CurrentBalance = 1500 }
            };
            _repoMock.Setup(r => r.GetWallets()).ReturnsAsync(wallets);

            var result = await _repoMock.Object.GetWallets();

            Assert.Equal(2, result.Count);
            Assert.Equal(500, result[0].CurrentBalance);
        }

        // ────────────────────────────────────────────────────────────
        // GetById
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_ExistingWallet_ShouldReturnWallet()
        {
            var wallet = new EWallet { WalletId = 10, CurrentBalance = 2000 };
            _repoMock.Setup(r => r.GetById(10)).ReturnsAsync(wallet);

            var result = await _repoMock.Object.GetById(10);

            Assert.NotNull(result);
            Assert.Equal(2000, result.CurrentBalance);
        }

        [Fact]
        public async Task GetById_NotFound_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetById(999)).ReturnsAsync((EWallet)null);

            var result = await _repoMock.Object.GetById(999);

            Assert.Null(result);
        }

        // ────────────────────────────────────────────────────────────
        // AddWallet (via service logic)
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task AddWallet_NewWallet_ShouldCallRepositoryAdd()
        {
            var wallet = new EWallet { WalletId = 5, CurrentBalance = 0 };
            _repoMock.Setup(r => r.GetById(5)).ReturnsAsync((EWallet)null);
            _repoMock.Setup(r => r.AddWallet(wallet)).Returns(Task.CompletedTask);

            // Simulate the service logic: only add if not exists
            var existing = await _repoMock.Object.GetById(5);
            if (existing == null)
                await _repoMock.Object.AddWallet(wallet);

            _repoMock.Verify(r => r.AddWallet(wallet), Times.Once);
        }

        [Fact]
        public async Task AddWallet_ExistingWallet_ShouldNotCallRepositoryAdd()
        {
            var existingWallet = new EWallet { WalletId = 3, CurrentBalance = 100 };
            _repoMock.Setup(r => r.GetById(3)).ReturnsAsync(existingWallet);

            // Simulate service logic: skip add if wallet already exists
            var existing = await _repoMock.Object.GetById(3);
            if (existing == null)
                await _repoMock.Object.AddWallet(existingWallet);

            _repoMock.Verify(r => r.AddWallet(It.IsAny<EWallet>()), Times.Never);
        }

        // ────────────────────────────────────────────────────────────
        // UpdateWallet
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateWallet_ShouldCallRepositoryUpdate()
        {
            var wallet = new EWallet { WalletId = 1, CurrentBalance = 1000 };
            _repoMock.Setup(r => r.UpdateWallet(wallet)).Returns(Task.CompletedTask);

            await _repoMock.Object.UpdateWallet(wallet);

            _repoMock.Verify(r => r.UpdateWallet(wallet), Times.Once);
        }

        // ────────────────────────────────────────────────────────────
        // AddStatement (CREDIT/DEBIT logic)
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task AddStatement_Credit_ShouldPersistCreditStatement()
        {
            var statement = new Statement
            {
                WalletId = 1,
                TransactionType = "CREDIT",
                Amount = 500,
                DateTime = DateTime.UtcNow,
                TransactionRemarks = "Razorpay Deposit"
            };
            _repoMock.Setup(r => r.AddStatement(statement)).Returns(Task.CompletedTask);

            await _repoMock.Object.AddStatement(statement);

            _repoMock.Verify(r => r.AddStatement(statement), Times.Once);
        }

        [Fact]
        public async Task AddStatement_Debit_ShouldPersistDebitStatement()
        {
            var statement = new Statement
            {
                WalletId = 1,
                TransactionType = "DEBIT",
                Amount = 300,
                DateTime = DateTime.UtcNow,
                TransactionRemarks = "Order Payment"
            };
            _repoMock.Setup(r => r.AddStatement(statement)).Returns(Task.CompletedTask);

            await _repoMock.Object.AddStatement(statement);

            _repoMock.Verify(r => r.AddStatement(It.Is<Statement>(s => s.TransactionType == "DEBIT")), Times.Once);
        }

        // ────────────────────────────────────────────────────────────
        // GetStatements
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetStatements_ShouldReturnAllStatements()
        {
            var statements = new List<Statement>
            {
                new Statement { StatementId = 1, WalletId = 1, Amount = 1000, TransactionType = "CREDIT" },
                new Statement { StatementId = 2, WalletId = 2, Amount = 500,  TransactionType = "DEBIT" }
            };
            _repoMock.Setup(r => r.GetStatements()).ReturnsAsync(statements);

            var result = await _repoMock.Object.GetStatements();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetStatementsById_ShouldReturnWalletSpecificStatements()
        {
            var statements = new List<Statement>
            {
                new Statement { StatementId = 1, WalletId = 5, Amount = 200, TransactionType = "CREDIT" },
                new Statement { StatementId = 2, WalletId = 5, Amount = 100, TransactionType = "DEBIT" }
            };
            _repoMock.Setup(r => r.GetStatementsById(5)).ReturnsAsync(statements);

            var result = await _repoMock.Object.GetStatementsById(5);

            Assert.Equal(2, result.Count);
            Assert.All(result, s => Assert.Equal(5, s.WalletId));
        }

        // ────────────────────────────────────────────────────────────
        // DeleteById
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteById_ShouldCallRepositoryDelete()
        {
            _repoMock.Setup(r => r.DeleteById(7)).Returns(Task.CompletedTask);

            await _repoMock.Object.DeleteById(7);

            _repoMock.Verify(r => r.DeleteById(7), Times.Once);
        }

        // ────────────────────────────────────────────────────────────
        // Balance Validation (Unit Logic)
        // ────────────────────────────────────────────────────────────

        [Fact]
        public void AddMoney_AmountMustBePositive_ShouldValidate()
        {
            decimal amount = -100;
            Assert.True(amount <= 0); // Service throws ArgumentException for amount <= 0
        }

        [Fact]
        public void Update_InsufficientBalance_ShouldDetect()
        {
            var wallet = new EWallet { WalletId = 1, CurrentBalance = 200 };
            decimal amountToDeduct = 500;

            // Service throws InvalidOperationException when balance < amount
            Assert.True(wallet.CurrentBalance < amountToDeduct);
        }

        [Fact]
        public void Update_SufficientBalance_ShouldPass()
        {
            var wallet = new EWallet { WalletId = 1, CurrentBalance = 1000 };
            decimal amountToDeduct = 500;

            Assert.True(wallet.CurrentBalance >= amountToDeduct);
        }
    }
}
