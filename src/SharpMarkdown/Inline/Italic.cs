using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown.Inline {
    [Match(Regex = @"^\*.+\*")]
    [Match(Regex = @"^__.+__")]
    public class Italic : Content {
        public override string OuterMarkdown {
            get {
                return "__" + string.Join("", Children.Select(x => x.OuterMarkdown))
                    + "__";
            }
            set {
                Children = ContentBase.AreaParse(value.Trim());
            }
        }


        public static Italic Parse(string text, out int length) {
            var attrs = MatchAttribute.GetMatchAttributes<Italic>()
                .Select(x => new {
                    match = x.GetRegex().IsMatch(text),
                    attr = x
                });

            if (!attrs.Any(x => x.match)) throw new FormatException();
            var temp = attrs.Where(x => x.match).FirstOrDefault();
            var match = temp.attr.GetRegex()
                .Match(text);
            if(temp.attr.Regex == @"^\*.+\*"){
                text = match.Value.Substring(1, match.Value.Length - 2);
            }else{
                text = match.Value.Substring(2, match.Value.Length - 4);
            }
            length = match.Index + match.Length;
            
            return new Italic() { Children = ContentBase.AreaParse(text) };
        }
    }
}
