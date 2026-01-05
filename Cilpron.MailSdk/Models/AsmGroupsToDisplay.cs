using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{
    public partial class AsmGroupsToDisplay
    {
        [Key]
        public Guid Id { get; set; }

        public Guid AsmId { get; set; }

        public int GroupValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("AsmId")]
        [InverseProperty("AsmGroupsToDisplays")]
        public virtual Asm Asm { get; set; } = null!;
    }

}
