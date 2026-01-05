using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{
    public partial class Attachment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MailRequestId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; } = null!;

        [StringLength(100)]
        public string ContentType { get; set; } = null!;

        public byte[] Data { get; set; } = null!;

        public long FileSize { get; set; }

        public bool IsInline { get; set; }

        [StringLength(255)]
        public string? ContentId { get; set; }

        [StringLength(64)]
        public string? Checksum { get; set; }

        public int DownloadCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [StringLength(50)]
        public string Disposition { get; set; } = null!;

        [ForeignKey("MailRequestId")]
        [InverseProperty("Attachments")]
        public virtual MailRequest MailRequest { get; set; } = null!;
    }

}
