using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown.Inline {
    /// <summary>
    /// 斜體
    /// </summary>
    [Match(Regex = @"^\*[^\*\r\n]+\*")]
    [Match(Regex = @"^__[^_\r\n]+__")]
    public class Italic : Markdown {
        public override string OuterMarkdown {
            get {
                return "*" + string.Join("", Children.Select(x => x.OuterMarkdown))
                    + "*";
            }
            set {
                Children = MarkdownRaw.Parse(value.Trim()).Children;
            }
        }


        public static Italic Parse(string text, out int length) {
            var attrs = MatchAttribute.GetMatchAttributes<Italic>()
                .Select(x => new {
                    match = x.GetRegex().IsMatch(text),
                    attr = x
                });

            if (!attrs.Any(x => x.match)) throw new FormatException();
            var temp = attrs.Where(x => x.match).FirstOrDefault();
            var match = temp.attr.GetRegex()
                .Match(text);
            if(temp.attr.Regex == @"^\*[^\*\r\n]+\*") {
                text = match.Value.Substring(1, match.Value.Length - 2);
            }else{
                text = match.Value.Substring(2, match.Value.Length - 4);
            }
            length = match.Index + match.Length;
            
            return new Italic() { Children = MarkdownRaw.Parse(text).Children };
        }
    }
}
