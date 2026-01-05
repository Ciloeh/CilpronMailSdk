using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{

    public partial class TrackingSetting
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MailRequestId { get; set; }

        public bool ClickTrackingEnabled { get; set; }

        public bool ClickTrackingHtml { get; set; }

        public bool ClickTrackingText { get; set; }

        public bool OpenTrackingEnabled { get; set; }

        [StringLength(100)]
        public string? OpenTrackingSubstitutionTag { get; set; }

        public bool SubscriptionTrackingEnabled { get; set; }

        [StringLength(500)]
        public string? SubscriptionText { get; set; }

        public string? SubscriptionHtml { get; set; }

        [StringLength(100)]
        public string? SubscriptionReplacementTag { get; set; }

        public bool GanalyticsEnabled { get; set; }

        [StringLength(100)]
        public string? UtmSource { get; set; }

        [StringLength(100)]
        public string? UtmMedium { get; set; }

        [StringLength(100)]
        public string? UtmCampaign { get; set; }

        [StringLength(100)]
        public string? UtmTerm { get; set; }

        [StringLength(100)]
        public string? UtmContent { get; set; }

        public DateTime CreatedAt { get; set; }


        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [InverseProperty("TrackingSettings")]
        public virtual ClickTracking? ClickTracking { get; set; }

        [InverseProperty("TrackingSettings")]
        public virtual Ganalytic? Ganalytic { get; set; }

        [ForeignKey("MailRequestId")]
        [InverseProperty("TrackingSetting")]
        public virtual MailRequest MailRequest { get; set; } = null!;

        [InverseProperty("TrackingSettings")]
        public virtual ICollection<MailRequest> MailRequests { get; set; } = new List<MailRequest>();

        [InverseProperty("TrackingSettings")]
        public virtual OpenTracking? OpenTracking { get; set; }

        [InverseProperty("TrackingSettings")]
        public virtual SubscriptionTracking? SubscriptionTracking { get; set; }
    }

}
