using SharpMarkdown.Attributes;
using SharpMarkdown.Inline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown.Line {
    public class Paragraph : Content{
        

        public List<Content> Children { get; set; } = new List<Content>();

        public override string OuterMarkdown {
            get {
                return string.Join("", Children.Select(x => x.OuterMarkdown));
            }
            set {
                
            }
        }    
        
        public static Paragraph Parse(string text) {
            var temp = text;
            Paragraph result = new Paragraph();
            result.Children.AddRange(Content.InlineParse(text));            
            return result;
        }
    }
}
