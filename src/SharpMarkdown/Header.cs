using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown {
    /// <summary>
    /// Markdown標題
    /// </summary>
    public class Header : Content{
        /// <summary>
        /// 階層
        /// </summary>
        public int Level { get; set; } 

        /// <summary>
        /// 內容
        /// </summary>
        public string Text { get; set; }
        
        public override string OuterMarkdown {
            get {
                return new string('#', Level) + " " + Text;
            }
            set {
                Header header = Parse(value);
                this.Level = header.Level;
                this.Text = header.Text;
            }
        }

        public static Header Parse(string text) {
            try {
                text = text.Trim();
                Regex regex = new Regex(@"#+");
                var headerText = regex.Match(text).Value;

                return new Header() {
                    Level = headerText.Length,
                    Text = text.Replace(headerText, "").Trim()
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
