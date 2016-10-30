using SharpMarkdown.Attributes;
using SharpMarkdown.Inline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown {
    [Match(Regex = ".+")]
    public class Content : ContentBase{
        public List<ContentBase> Children { get; set; } = new List<ContentBase>();

        public bool IsSingleLine {
            get {
                return Children.All(x => {
                    return x.GetType().Namespace.Split('.').Last() == nameof(Inline);
                });
            }
        }

        public override string OuterMarkdown {
            get {
                return ToMarkdown(Children, IsSingleLine);
            }
            set {
                Children = ContentBase.Parse(value.Trim());
            }
        }    
        
        public static Content Parse(string text, out int length) {
            //var temp = new string(text.TakeWhile(x=> x!= '\n').ToArray());
            var lines = text.Split('\n');
            var temp = lines.FirstOrDefault() ?? "";
            length = temp.Length+1;
            Content result = new Content();
            result.Children = ContentBase.Parse(temp,true);
            return result;
        }
    }
}
