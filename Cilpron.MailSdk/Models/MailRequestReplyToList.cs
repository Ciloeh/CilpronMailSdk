using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{
    public partial class MailRequestReplyToList
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MailRequestId { get; set; }

        [StringLength(255)]
        public string Email { get; set; } = null!;

        [StringLength(255)]
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("MailRequestId")]
        [InverseProperty("MailRequestReplyToLists")]
        public virtual MailRequest MailRequest { get; set; } = null!;
    }

}
