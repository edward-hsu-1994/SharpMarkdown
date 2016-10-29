using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Inline {
    [Match(Regex = @"^`.+`")]
    public class Code : Content {
        public string Text { get; set; }
        public override string OuterMarkdown {
            get {
                return "`" + string.Join("", Children.Select(x => x.OuterMarkdown))
                    +"`";
            }
            set {
                int temp = 0;
                Code code = Parse(value, out temp);

                this.Text = code.Text;
            }
        }

        public static Code Parse(string text, out int length) {
            var attrs = MatchAttribute
                .GetMatchAttributes<Code>()
                .Select(x => new {
                    match = x.GetRegex().IsMatch(text),
                    attr = x
                });
            if (!attrs.Any(x=>x.match)) {
                throw new FormatException();
            }

            Match match = attrs.Where(x => x.match).FirstOrDefault()
                .attr.GetRegex().Match(text);
            string temp = match.Value;
            var result = new Code() {
                Text = temp.Substring(1, temp.Length - 2)
            };
            length = match.Index + match.Length;
            return result;
        }
    }
}
