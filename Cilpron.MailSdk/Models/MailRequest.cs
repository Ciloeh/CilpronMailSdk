using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Xml.Linq;


namespace Cilpron.MailSdk.Models
{
    public partial class MailRequest
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(255)]
        public string FromEmail { get; set; } = null!;

        [StringLength(255)]
        public string? FromName { get; set; }

        [StringLength(255)]
        public string? ReplyToEmail { get; set; }

        [StringLength(255)]
        public string? ReplyToName { get; set; }

        [StringLength(500)]
        public string? Subject { get; set; }

        public string? HtmlContent { get; set; }

        public string? PlainTextContent { get; set; }

        [StringLength(100)]
        public string? TemplateId { get; set; }

        [StringLength(50)]
        public string? Language { get; set; }

        public int Priority { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = null!;

        public DateTime? ScheduledAt { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public DateTime? FailedAt { get; set; }

        [StringLength(500)]
        public string? ErrorMessage { get; set; }

        public int DeliveryAttempts { get; set; }

        public bool SuppressionChecked { get; set; }

        public bool DkimSigned { get; set; }

        public bool DmarcPassed { get; set; }

        public bool Encrypted { get; set; }

        [StringLength(50)]
        public string? RetentionPolicy { get; set; }

        public int OpenCount { get; set; }

        public int ClickCount { get; set; }

        public DateTime? LastOpenedAt { get; set; }

        public DateTime? LastClickedAt { get; set; }

        public bool Complaint { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(255)]
        public string? UserAgent { get; set; }

        [StringLength(255)]
        public string ApiKey { get; set; } = null!;

        public string? Categories { get; set; }

        public string? ReplyToList { get; set; }

        public string? Content { get; set; }

        public Guid? TrackingSettingsId { get; set; }

        public Guid? MailSettingsId { get; set; }

        public string? CustomArgs { get; set; }

        public long? SendAt { get; set; }

        public string? Recipients { get; set; }

        [InverseProperty("MailRequest")]
        public virtual ICollection<AiSpamResponse> AiSpamResponses { get; set; } = new List<AiSpamResponse>();

        [InverseProperty("IdNavigation")]
        public virtual Asm? Asm { get; set; }

        [InverseProperty("MailRequest")]
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        [InverseProperty("MailRequest")]
        public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

        [InverseProperty("MailRequest")]
        public virtual ICollection<MailRequestReplyToList> MailRequestReplyToLists { get; set; } = new List<MailRequestReplyToList>();

        [InverseProperty("MailRequest")]
        public virtual MailSetting? MailSetting { get; set; }

        [ForeignKey("MailSettingsId")]
        [InverseProperty("MailRequests")]
        public virtual MailSetting? MailSettings { get; set; }

        [InverseProperty("MailRequest")]
        public virtual ICollection<Personalization> Personalizations { get; set; } = new List<Personalization>();

        [InverseProperty("MailRequest")]
        public virtual TrackingSetting? TrackingSetting { get; set; }

        [ForeignKey("TrackingSettingsId")]
        [InverseProperty("MailRequests")]
        public virtual TrackingSetting? TrackingSettings { get; set; }
    }

}
