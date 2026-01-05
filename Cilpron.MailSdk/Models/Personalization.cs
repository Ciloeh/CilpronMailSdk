using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{
    public partial class Personalization
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MailRequestId { get; set; }

        [StringLength(255)]
        public string ToEmail { get; set; } = null!;

        [StringLength(255)]
        public string? ToName { get; set; }

        [StringLength(255)]
        public string? CcEmail { get; set; }

        [StringLength(255)]
        public string? BccEmail { get; set; }

        public string? SubstitutionData { get; set; }

        public string? CustomArgs { get; set; }

        [StringLength(255)]
        public string? Tags { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = null!;

        [StringLength(500)]
        public string? ErrorMessage { get; set; }

        public int DeliveryAttempts { get; set; }

        public DateTime? ScheduledAt { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public bool Suppressed { get; set; }

        public bool Unsubscribed { get; set; }

        public bool Complaint { get; set; }

        public int OpenCount { get; set; }

        public int ClickCount { get; set; }

        public DateTime? LastOpenedAt { get; set; }

        public DateTime? LastClickedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? Headers { get; set; }

        public string? Substitutions { get; set; }

        [ForeignKey("MailRequestId")]
        [InverseProperty("Personalizations")]
        public virtual MailRequest MailRequest { get; set; } = null!;
    }
}
