using SharpMarkdown.Attributes;
using SharpMarkdown.Inline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown {
    public class Content : ContentBase{
        public List<ContentBase> Children { get; set; } = new List<ContentBase>();

        public override string OuterMarkdown {
            get {
                return string.Join("", Children.Select(x => x.OuterMarkdown));
            }
            set {
                Children = ContentBase.InlineParse(value.Trim());
            }
        }    
        
        public static Content Parse(string text) {
            var temp = text;
            Content result = new Content();
            result.Children = ContentBase.InlineParse(text);
            return result;
        }
    }
}
