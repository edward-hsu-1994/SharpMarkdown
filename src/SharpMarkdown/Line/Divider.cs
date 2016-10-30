using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Line {
    [Match(Regex = @"^-{3,}$")]
    [Match(Regex = @"^-{3,}(\r?\n)")]
    [Match(Regex = @"^\*{3,}$")]
    [Match(Regex = @"^\*{3,}(\r?\n)")]
    public class Divider : ContentBase {
        public override string OuterMarkdown {
            get {
                return "-----\n";
            }
            set {
                int temp = 0;
                Parse(value, out temp);
            }
        }
        public static Divider Parse(string text, out int length) {
            var attrs = MatchAttribute.GetMatchAttributes<Divider>()
                .Select(x => new {
                    match = x.GetRegex().IsMatch(text),
                    attr = x
                });

            if (!attrs.Any(x=>x.match)) throw new FormatException();

            
            Match match = attrs.Where(x => x.match)
                    .FirstOrDefault().attr
                    .GetRegex().Match(text);

            length = match.Index + match.Length;
            return new Divider();
        }

    }
}
