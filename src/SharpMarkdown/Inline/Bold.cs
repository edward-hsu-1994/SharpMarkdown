using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Inline {
    [Match(Regex = @"^\*\*[^\*\r\n]+\*\*")]
    [Match(Regex = @"^__[^_\r\n]+__")]
    public class Bold : Content{
        public override string OuterMarkdown {
            get {
                return "**" + string.Join("",Children.Select(x=>x.OuterMarkdown))
                    + "**";
            }
            set {
                Children = ContentBase.Parse(value.Trim()).Children;
            }
        }
        
        public static Bold Parse(string text, out int length) {
            var attrs = MatchAttribute.GetMatchAttributes<Bold>()
                .Select(x => new {
                    match = x.GetRegex().IsMatch(text),
                    attr = x
                });

            if(!attrs.Any(x => x.match)) throw new FormatException();
            var match = attrs.Where(x => x.match).FirstOrDefault().attr.GetRegex()
                .Match(text);

            length = match.Index + match.Length;
            text = match.Value.Substring(2, match.Value.Length - 4);
            return new Bold() { Children = ContentBase.Parse(text).Children };
        }
    }
}