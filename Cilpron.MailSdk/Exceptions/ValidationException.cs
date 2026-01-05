using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Exceptions
{
    /// <summary>
    /// Exception for input validation failures.
    /// </summary>
    public class ValidationException : CilpronMailException
    {
        public ValidationException(string message) : base(message) { }
    }
}
