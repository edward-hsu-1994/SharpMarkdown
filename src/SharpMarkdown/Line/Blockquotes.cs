using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Line {
    [Match(Regex = @"^(>\s)+.*")]
    public class Blockquotes : Content {
        public int Level { get; set;}
        public override string OuterMarkdown {
            get {
                string result = string.Empty;
                for (int i = 0; i < Level; i++) result += "> ";
                return result + Children.Select(x => x.OuterMarkdown);
            }
            set {
                base.OuterMarkdown = value;
            }
        }

        public static Blockquotes Parse(string text) {
            text = text.Trim();
            var check = MatchAttribute.GetMatchAttributes<Blockquotes>()
                .Select(x => x.GetRegex().IsMatch(text))
                .Any();
            if (!check) throw new FormatException();

            Blockquotes result = new Blockquotes();
            Regex regex = new Regex(@"(>\s)+");
            text = text.Replace(regex.Match(text).Value,"");
            result.Children = Content.InlineParse(text);

            return result;
        }
    }
}
