using System.Collections.Generic;

namespace AtlasReaper.Utils
{
    class Regex
    {
        Dictionary<string, string> regexMap = new Dictionary<string, string>
        {
            // These are taken from Nosey Parker
            // https://github.com/praetorian-inc/noseyparker

            { "AWS API Key", @"/((?:A3T[A-Z0-9]|AKIA|AGPA|AIDA|AROA|AIPA|ANPA|ANVA|ASIA)[A-Z0-9]{16})/" },
        };
    }
}

