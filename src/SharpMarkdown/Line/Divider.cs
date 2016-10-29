using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown.Line {
    [Match(Regex = @"^([\*\-]\s?){3,}")]
    public class Divider : ContentBase {
        public override string OuterMarkdown {
            get {
                return "-----";
            }
            set {
                int temp = 0;
                Parse(value, out temp);
            }
        }
        public static Divider Parse(string text, out int length) {
            var check = MatchAttribute.GetMatchAttributes<Divider>()
                .Select(x => x.GetRegex().IsMatch(text))
                .Any();

            if (!check) throw new FormatException();
            length = text.Length;
            return new Divider();
        }

    }
}
