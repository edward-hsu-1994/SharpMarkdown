using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Inline {
    /// <summary>
    /// 自動連結
    /// </summary>
    [Match(Regex = @"^<https?://.+>")]
    [Match(Regex = @"^<.+@.+>")]
    public class AutoLink : Link {
        public override string OuterMarkdown {
            get {
                return $"<{this.URL}>";
            }
            set {
                int temp = 0;
                AutoLink autoLink = Parse(value, out temp);
                this.Text = autoLink.Text;
                this.URL = autoLink.URL;
            }
        }

        public override string OuterText {
            get {
                return URL;
            }
        }

        public static AutoLink Parse(string text,out int length) {
            var attrs = MatchAttribute.GetMatchAttributes<AutoLink>()
                .Select(x => new {
                    match = x.GetRegex().IsMatch(text),
                    attr = x
                });
            if (!attrs.Any(x=>x.match)) throw new FormatException();

            Match match = attrs.Where(x => x.match).FirstOrDefault().attr
                .GetRegex().Match(text);

            string temp = match.Value;
            var value = temp.Substring(1, text.Length - 2);
            length = match.Index + match.Length;
            return new AutoLink() {
                Text = value,
                URL = value
            };
        }
    }
}
