using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SharpMarkdown.Inline;
namespace SharpMarkdown.Line {
    /// <summary>
    /// Setext標題
    /// </summary>
    [Match(Regex = @"^(\s*.+)\r?\n[=-]+$?")]
    public class SetextHeader : Header{
        public override string OuterMarkdown {
            get {
                return string.Join("",Children.Select(x=>x.OuterMarkdown)) +
                    "\n" + new string(Level == 1 ? '=': '-',10);
            }
            set {
                int temp = 0;
                SetextHeader header = Parse(value,out temp);
                this.Level = header.Level;
                this.Children = header.Children;
            }
        }

        public static SetextHeader Parse(string text,out int length) {
            try {
                var attrs = MatchAttribute.GetMatchAttributes<SetextHeader>()
                    .Select(x =>new {
                        match = x.GetRegex().IsMatch(text),
                        attr = x
                    });

                Match match = attrs.Where(x => x.match)
                    .FirstOrDefault().attr
                    .GetRegex().Match(text);

                Regex regex = new Regex(@"(\s*.+)\r?\n");
                Match title = regex.Match(match.Value);
                var headerText = title.Value.Trim();
                var sy = match.Value.Substring(title.Length)[0];
                length = match.Index + match.Length;
                return new SetextHeader() {
                    Level = sy == '=' ? 1 : 0,
                    Children = MarkdownRaw.Parse(headerText).Children
                };
            }catch(Exception e) {
                throw new FormatException();
            }
        }
    }
}
