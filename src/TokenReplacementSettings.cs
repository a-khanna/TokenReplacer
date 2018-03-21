using System.Collections.Generic;

namespace ConfigurationExtension
{
    internal class TokenReplacementSettings
    {
        public bool Replace { get; set; }

        public bool OnlySpecific { get; set; }

        public IEnumerable<string> Tokens { get; set; }
    }
}
