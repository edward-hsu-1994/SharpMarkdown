using SharpMarkdown.Attributes;
using SharpMarkdown.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Area {
    [Match(Regex = @"^(>\s?(>\s?)*.+((\r?\n)|$))+")]
    public class Blockquotes : Content {
        public override string OuterMarkdown {
            get {
                return "\n" + 
                    string.Join("\n\n", Children.Select(x => "> " + x.OuterMarkdown)) +
                    "\n\n";
            }
            set {
                int temp = 0;
                Blockquotes blockquotes = Parse(value, out temp);
                this.Children = blockquotes.Children;
            }
        }

        public static Blockquotes Parse(string text, out int length) {
            var attrs = MatchAttribute.GetMatchAttributes<Blockquotes>()
                .Select(x => new {
                    match = x.GetRegex().IsMatch(text),
                    attr = x
                });

            var result = new Blockquotes();
            Match match = attrs.Where(x => x.match)
                .FirstOrDefault().attr
                .GetRegex().Match(text);

            string text2 = string.Join("\n", match.Value.Replace("\r", "").Split('\n')                
                .Select(x => {
                    Regex regex = new Regex(@"\s*>\s?");
                    var temp = regex.Match(x);
                    return x.Substring(temp.Index + temp.Length);
                })).Trim();

            result.Children = Content.Parse(text2);
            
            length = match.Index + match.Length;
            return result;
        }
    }
}
