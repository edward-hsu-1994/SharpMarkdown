using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown.Inline {
    [Match(Regex = @"^`.+`")]
    public class Code : Paragraph {
        public string Text { get; set; }
        public override string OuterMarkdown {
            get {
                return string.Join("", Children.Select(x => x.OuterMarkdown));
            }
            set {
                
            }
        }

        public static Code Parse(string text, out int length) {
            if (!MatchAttribute
                .GetMatchAttributes<Code>()
                .Select(x => x.GetRegex().IsMatch(text)).Any()) {
                throw new FormatException();
            }
            var result = new Code() {
                Text = text.Substring(1, text.Length - 2)
            };
            length = text.Length;
            return result;
        }
    }
}
