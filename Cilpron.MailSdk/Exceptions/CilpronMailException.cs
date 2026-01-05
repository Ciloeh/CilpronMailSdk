using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Exceptions
{
    /// <summary>
    /// Base exception for Cilpron Mail SDK errors.
    /// </summary>
    public class CilpronMailException : Exception
    {
        public CilpronMailException(string message) : base(message) { }

        public CilpronMailException(string message, Exception innerException) : base(message, innerException) { }
    }
}
