using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WalletService.Entities
{
    public class Statement
    {
        [Key]
        public int StatementId { get; set; }

        [Required]
        public string TransactionType { get; set; } = string.Empty; // "CREDIT" or "DEBIT"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public int? OrderId { get; set; }

        [Required]
        public string TransactionRemarks { get; set; } = string.Empty;

        [Required]
        public int WalletId { get; set; }

        [JsonIgnore] // Prevent circular reference in JSON responses
        public EWallet? Wallet { get; set; }
    }
}
