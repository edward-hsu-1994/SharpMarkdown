using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SharpMarkdown.Inline;
namespace SharpMarkdown.Line {
    /// <summary>
    /// Markdown標題
    /// </summary>
    [Match(Regex = @"#+\s+.+")]
    public class Header : Content{
        /// <summary>
        /// 階層
        /// </summary>
        public int Level { get; set; }
        
        public override string OuterMarkdown {
            get {
                return new string('#', Level) + " " + 
                    string.Join("",Children.Select(x=>x.OuterMarkdown));
            }
            set {
                Header header = Parse(value);
                this.Level = header.Level;
                this.Children = header.Children;
            }
        }

        public static Header Parse(string text) {
            try {
                text = text.Trim();
                Regex regex = new Regex(@"#+");
                var headerText = regex.Match(text).Value;

                return new Header() {
                    Level = headerText.Length,
                    Children = ContentBase.InlineParse(text.Replace(headerText, "").Trim())
                };
            }catch(Exception e) {
                throw new FormatException();
            }
        }

        public static bool TryParse(string text,out Header result) {
            try {
                result = Parse(text);
                return true;
            } catch {
                result = null;
                return false;
            }
        }
    }
}
