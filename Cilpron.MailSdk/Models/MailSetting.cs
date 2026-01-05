using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{

    public partial class MailSetting
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MailRequestId { get; set; }

        public bool BypassListManagementEnabled { get; set; }

        public bool BypassSpamManagementEnabled { get; set; }

        public bool BypassBounceManagementEnabled { get; set; }

        public bool BypassUnsubscribeManagementEnabled { get; set; }

        public bool FooterEnabled { get; set; }

        [StringLength(500)]
        public string? FooterText { get; set; }

        public string? FooterHtml { get; set; }

        public bool SandboxModeEnabled { get; set; }

        public bool SpamCheckEnabled { get; set; }

        public int? SpamCheckThreshold { get; set; }

        [StringLength(500)]
        public string? SpamCheckPostToUrl { get; set; }

        [StringLength(255)]
        public string? BounceCnameTarget { get; set; }

        [StringLength(255)]
        public string? SuggestedSpfInclude { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [ForeignKey("MailRequestId")]
        [InverseProperty("MailSetting")]
        public virtual MailRequest MailRequest { get; set; } = null!;

        [InverseProperty("MailSettings")]
        public virtual ICollection<MailRequest> MailRequests { get; set; } = new List<MailRequest>();
    }

}
