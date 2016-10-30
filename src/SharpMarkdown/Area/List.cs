using SharpMarkdown.Attributes;
using SharpMarkdown.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Area {
    [Match(Regex = @"^(\s*\d+\.\s+.+((\r?\n)|$))+")]
    [Match(Regex = @"^(\s*\*\s+.+((\r?\n)|$))+")]
    [Match(Regex = @"^(\s*\+\s+.+((\r?\n)|$))+")]
    [Match(Regex = @"^(\s*\-\s+.+((\r?\n)|$))+")]
    public class List : Content {
        public ListTypes Type { get; set; }

        public override string OuterMarkdown {
            get {
                return "\n\n" + string.Join("\n", Children.Select((x,i) => {
                    return (Type == ListTypes.Number ? ((i + 1) + ". ") : "* ") +
                        x.OuterMarkdown;
                })) + "\n\n";
            }
            set {
                int temp = 0;
                List blockquotes = Parse(value, out temp);
                this.Children = blockquotes.Children;
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

            var items = string.Join("\n", 
                    match.Value.Replace("\r", "")
                    .Split('\n')   
                    .Select(x => {
                        Regex regex = new Regex(@"\s*((\d+.)|\*|\+|\-)+\s*");
                        var temp = regex.Match(x);
                        return x.Substring(temp.Index + temp.Length);
                    })
                ).Trim().Split(new char[] { '\n' },StringSplitOptions.RemoveEmptyEntries)
                .Select(x=>new Content() { Children = Content.Parse(x) })
                .ToList<ContentBase>();

            result.Children = items;
            
            length = match.Index + match.Length;
            return result;
        }
    }
}
