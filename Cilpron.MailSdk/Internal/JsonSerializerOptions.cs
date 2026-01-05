using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Internal
{
    /// <summary>
    /// Custom JSON serializer options.
    /// </summary>
    public static class JsonSerializerOptions
    {
        public static readonly System.Text.Json.JsonSerializerOptions Default = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };
    }
}
