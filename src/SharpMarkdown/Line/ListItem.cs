using SharpMarkdown.Attributes;
using SharpMarkdown.Inline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Line {
    [Match(Regex = @"^\d+\.\s+[^\r\n]+$")]
    [Match(Regex = @"^\d+\.\s+[^\r\n]+\r?\n")]
    [Match(Regex = @"^\*\s+[^\*\r\n]+$")]
    [Match(Regex = @"^\*\s+[^\*\r\n]+\r?\n")]
    [Match(Regex = @"^\+\s+[^\+\r\n]+$")]
    [Match(Regex = @"^\+\s+[^\+\r\n]+\r?\n")]
    [Match(Regex = @"^\-\s+[^\-\r\n]+$")]
    [Match(Regex = @"^\-\s+[^\-\r\n]+\r\n")]
    public class ListItem : Content{
        public string Symbol { get; set; }

        public override string OuterMarkdown {
            get {
                return $"{Symbol} {string.Join("",Children.Select(x=>x.OuterMarkdown))}\n";
            }
            set {
                int temp = 0;
                ListItem listItem = Parse(value,out temp);
                this.Symbol = listItem.Symbol;
                this.Children = listItem.Children;
            }
        }

        public static ListItem Parse(string text,out int length) {
            var attrs = MatchAttribute.GetMatchAttributes<ListItem>()
                    .Select(x => new {
                        match = x.GetRegex().IsMatch(text),
                        attr = x
                    });
            if (!attrs.Any(x=>x.match)) throw new FormatException();

            Match match = attrs.Where(x => x.match)
                .FirstOrDefault().attr
                .GetRegex().Match(text);
            
            var splited = match.Value.Split(new char[] { ' ' },2).Select(x => x.Trim()).ToArray();
            try {
                length = match.Index + match.Length;
                return new ListItem() {
                    Symbol = splited[0],
                    Children = ContentBase.Parse(splited[1])
                };
            }catch {
                throw new FormatException();
            }
        }
    }
}