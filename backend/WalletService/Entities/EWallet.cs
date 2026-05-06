using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletService.Entities
{
    public class EWallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Matches User/Customer ID
        public int WalletId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; }

        public IList<Statement> Statements { get; set; } = new List<Statement>();
    }
}
