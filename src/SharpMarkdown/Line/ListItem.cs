using SharpMarkdown.Attributes;
using SharpMarkdown.Inline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown.Line {
    [Match(Regex = @"^\d\.\s+.+$")]
    [Match(Regex = @"^\+\.\s+.+$")]
    [Match(Regex = @"^\+\.\s+.+$")]
    [Match(Regex = @"^\*\.\s+.+$")]
    public class ListItem : Paragraph{
        public string Symbol { get; set; }

        public override string OuterMarkdown {
            get {
                return $"{Symbol} {string.Join("",Children.Select(x=>x.OuterMarkdown))}";
            }
            set {
                ListItem listItem = Parse(value);
                this.Symbol = listItem.Symbol;
                this.Children = listItem.Children;
            }
        }

        public static ListItem Parse(string text) {
            text = text.Trim();
            var check = MatchAttribute.GetMatchAttributes<ListItem>()
                .Select(x => x.GetRegex().IsMatch(text))
                .Any();
            if (!check) throw new FormatException();
            var splited = text.Split(' ').Select(x => x.Trim()).ToArray();
            try {
                return new ListItem() {
                    Symbol = splited[0],
                    Children = Content.InlineParse(splited[1])
                };
            }catch {
                throw new FormatException();
            }
        }
    }
}