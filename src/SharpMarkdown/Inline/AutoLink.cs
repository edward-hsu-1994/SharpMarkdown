using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static AutoLink Parse(string text,out int length) {
            var check = MatchAttribute.GetMatchAttributes<AutoLink>()
                .Select(x => x.GetRegex().IsMatch(text))
                .Any();
            if (!check) throw new FormatException();

            var value = text.Substring(1, text.Length - 2);
            length = text.Length;
            return new AutoLink() {
                Text = value,
                URL = value
            };
        }
    }
}
