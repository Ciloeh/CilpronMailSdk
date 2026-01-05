using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{

    public partial class AiSpamResponse
    {
        [Key]
        public Guid Id { get; set; }

        public double SpamProbability { get; set; }

        [StringLength(1000)]
        public string Explanation { get; set; } = null!;

        [StringLength(100)]
        public string? Evaluator { get; set; }

        public DateTime EvaluatedAt { get; set; }

        [StringLength(100)]
        public string? EvaluatedBy { get; set; }

        public string? Metadata { get; set; }

        public double Threshold { get; set; }

        public bool IsSpam { get; set; }

        public Guid? MailRequestId { get; set; }

        [StringLength(50)]
        public string? RetentionPolicy { get; set; }

        public bool IsArchived { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [ForeignKey("MailRequestId")]
        [InverseProperty("AiSpamResponses")]
        public virtual MailRequest? MailRequest { get; set; }
    }
}
