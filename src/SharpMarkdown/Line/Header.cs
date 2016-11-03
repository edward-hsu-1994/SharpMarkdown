using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SharpMarkdown.Inline;
namespace SharpMarkdown.Line {
    /// <summary>
    /// 標題
    /// </summary>
    [Match(Regex = @"^([\t\f]*#)+\s+.+(\r?\n|$)")]
    public class Header : Markdown{
        /// <summary>
        /// 階層
        /// </summary>
        public int Level { get; set; }
        

        public override string OuterMarkdown {
            get {
                return new string('#', Level) + " " + 
                    InnerMarkdown;
            }
            set {
                int temp = 0;
                Header header = Parse(value,out temp);
                this.Level = header.Level;
                this.Children = header.Children;
            }
        }

        public override string InnerMarkdown {
            get {
                return string.Join("", Children.Select(x => x.OuterMarkdown));
            }
        }

        public override string InnerText {
            get {
                return base.InnerText;
            }
        }

        public static Header Parse(string text,out int length) {
            try {
                var attrs = MatchAttribute.GetMatchAttributes<Header>()
                    .Select(x =>new {
                        match = x.GetRegex().IsMatch(text),
                        attr = x
                    });

                Match match = attrs.Where(x => x.match)
                    .FirstOrDefault().attr
                    .GetRegex().Match(text);

                Regex regex = new Regex(@"#+");
                var headerText = regex.Match(match.Value).Value;

                length = match.Index + match.Length;
                return new Header() {
                    Level = headerText.Length,
                    Children = MarkdownRaw.Parse(match.Value.Replace(headerText, "")
                    .Trim()).Children
                };
            }catch(Exception e) {
                throw new FormatException();
            }
        }
    }
}
