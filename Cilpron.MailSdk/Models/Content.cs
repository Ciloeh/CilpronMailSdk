using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{

    public partial class Content
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MailRequestId { get; set; }

        [StringLength(50)]
        public string Type { get; set; } = null!;

        public string Value { get; set; } = null!;

        [StringLength(50)]
        public string? Charset { get; set; }

        [StringLength(50)]
        public string? Language { get; set; }

        [StringLength(100)]
        public string? TemplateId { get; set; }

        public string? SubstitutionData { get; set; }

        public bool IsFallback { get; set; }

        public bool Sanitized { get; set; }

        public bool Encrypted { get; set; }

        [StringLength(50)]
        public string? RetentionPolicy { get; set; }

        public int RenderCount { get; set; }

        public DateTime? LastRenderedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("MailRequestId")]
        [InverseProperty("Contents")]
        public virtual MailRequest MailRequest { get; set; } = null!;
    }

}
