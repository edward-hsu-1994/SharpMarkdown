using SharpMarkdown.Attributes;
using SharpMarkdown.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Area {
    /// <summary>
    /// 清單
    /// </summary>
    [Match(Regex = @"^(\s*\d+\.\s*.+((\r?\n)|$))+")]
    [Match(Regex = @"^(\s*\*\s+.+((\r?\n)|$))+")]
    [Match(Regex = @"^(\s*\+\s+.+((\r?\n)|$))+")]
    [Match(Regex = @"^(\s*\-\s+.+((\r?\n)|$))+")]
    public class List : Markdown {
        /// <summary>
        /// 類型
        /// </summary>
        public ListTypes Type { get; set; }

        public override string OuterMarkdown {
            get {
                return string.Join("\n", Children.Select((x,i) => {
                    return (Type == ListTypes.Number ? ((i + 1) + ". ") : "* ") +
                        x.OuterMarkdown;
                }));
            }
            set {
                int temp = 0;
                List blockquotes = Parse(value, out temp);
                this.Children = blockquotes.Children;
            }
        }

        public override string OuterText {
            get {
                return string.Join("\n", Children.Select((x, i) => {
                    return (Type == ListTypes.Number ? ((i + 1) + ". ") : "* ") +
                        x.OuterMarkdown;
                }));
            }
        }

        public static List Parse(string text, out int length) {
            var attrs = MatchAttribute.GetMatchAttributes<List>()
                .Select(x => new {
                    match = x.GetRegex().IsMatch(text),
                    attr = x
                });

            var result = new List();
            Match match = attrs.Where(x => x.match)
                .FirstOrDefault().attr
                .GetRegex().Match(text);
            bool? isNumber = null;
            var items = string.Join("\n", 
                    match.Value.Replace("\r", "")
                    .Split(new char[] { '\n' },StringSplitOptions.RemoveEmptyEntries)   
                    .Select(x => {
                        Regex regex = new Regex(@"\s*((\d+\.)|\*+|\++|\-+)\s*");
                        var temp = regex.Match(x);
                        if (!isNumber.HasValue) {
                            isNumber = new Regex(@"\d+").IsMatch(temp.Value.Trim());
                        }
                        return x.Substring(temp.Index + temp.Length);
                    })
                ).Trim().Split(new char[] { '\n' },StringSplitOptions.RemoveEmptyEntries)
                .Select(x=>new Markdown() { Children = Markdown.Parse(x).Children })
                .ToList<MarkdownRaw>();

            result.Children = items;
            result.Type = isNumber.Value ? ListTypes.Number : ListTypes.Symbol;
            length = match.Index + match.Length;
            return result;
        }
    }
}
