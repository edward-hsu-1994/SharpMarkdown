using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Inline {
    [Match(Regex = @"^\*\*.+\*\*")]
    public class Bold : Content{
        public static Bold Parse(string text, out int length) {
            text = text.Trim();
            var attr = MatchAttribute.GetMatchAttribute<Bold>();
            if (attr == null) throw new FormatException();
            Match match = attr.GetRegex().Match(text);
            if (match == null) throw new FormatException();
            text = match.Value.Substring(2, match.Value.Length - 4);

            length = match.Index + match.Length;
            return new Bold() { OuterMarkdown = text };
        }
    }
}