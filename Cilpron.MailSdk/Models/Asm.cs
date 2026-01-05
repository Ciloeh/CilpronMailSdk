using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{
    public partial class Asm
    {
        [Key]
        public Guid Id { get; set; }

        public Guid SendGridMailRequestId { get; set; }

        public int GroupId { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [InverseProperty("Asm")]
        public virtual ICollection<AsmGroupsToDisplay> AsmGroupsToDisplays { get; set; } = new List<AsmGroupsToDisplay>();

        [ForeignKey("Id")]
        [InverseProperty("Asm")]
        public virtual MailRequest IdNavigation { get; set; } = null!;
    }
}
