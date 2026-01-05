using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Models
{
    public class SendMailResponse
    {
        public string MessageId { get; set; } = default!;
        public string Status { get; set; } = "queued"; // e.g. "queued", "sent", "failed"
        public string? Error { get; set; }
    }

}
