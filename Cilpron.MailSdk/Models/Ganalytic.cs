using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{
    public partial class Ganalytic
    {
        [Key]
        public Guid Id { get; set; }

        public Guid TrackingSettingsId { get; set; }

        public bool? Enable { get; set; }

        [StringLength(100)]
        public string? UtmSource { get; set; }

        [StringLength(100)]
        public string? UtmMedium { get; set; }

        [StringLength(100)]
        public string? UtmTerm { get; set; }

        [StringLength(100)]
        public string? UtmContent { get; set; }

        [StringLength(100)]
        public string? UtmCampaign { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [ForeignKey("TrackingSettingsId")]
        [InverseProperty("Ganalytic")]
        public virtual TrackingSetting TrackingSettings { get; set; } = null!;
    }
}
    