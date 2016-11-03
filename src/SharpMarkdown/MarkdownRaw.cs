using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using SharpMarkdown.Inline;
using SharpMarkdown.Line;
using SharpMarkdown.Area;
using System.Text.RegularExpressions;

namespace SharpMarkdown {
    /// <summary>
    /// Markdown內容
    /// </summary>
    public class MarkdownRaw {
        /// <summary>
        /// 完整Markdown片段
        /// </summary>
        public virtual string OuterMarkdown { get; set; }

        /// <summary>
        /// 外顯文字內容
        /// </summary>
        public virtual string OuterText {
            get {
                return OuterMarkdown;
            }
        }

        /// <summary>
        /// 內部Markdown片段
        /// </summary>
        public virtual string InnerMarkdown {
            get {
                return OuterMarkdown;
            }
        }

        /// <summary>
        /// 外顯內部文字內容
        /// </summary>
        public virtual string InnerText {
            get {
                return OuterMarkdown;
            }
        }

        protected Match GetMatch(string text) {
            return MatchAttribute.GetMatchAttributes(GetType())
                .Select(x => new {
                    match = x.GetRegex().IsMatch(text),
                    attr = x
                })
                .Where(x=>x.match)
                .FirstOrDefault()
                ?.attr.GetRegex().Match(text);
        }
        
        /// <summary>
        /// 隱含轉換<see cref="string"/>為<see cref="MarkdownRaw"/>內容
        /// </summary>
        /// <param name="markdown">Markdown內容</param>
        public static implicit operator MarkdownRaw(string markdown) {
            if (markdown == null) return null;
            return new MarkdownRaw() { OuterMarkdown = markdown };
        }

        /// <summary>
        /// 明確轉換<see cref="MarkdownRaw"/>為<see cref="string"/>
        /// </summary>
        /// <param name="content">string本文</param>
        public static explicit operator string(MarkdownRaw content) {
            return content?.OuterMarkdown;
        }

        /// <summary>
        /// 行內樣式
        /// </summary>
        public static List<Type> InlineTypes = new List<Type>(
            new Type[] {
                typeof(AutoLink), typeof(Bold),
                typeof(Code),typeof(Delete),
                typeof(Italic),typeof(Link),
                typeof(Image)
            });

        /// <summary>
        /// 行樣式
        /// </summary>
        public static List<Type> LineTypes = new List<Type>(
            new Type[] {
                typeof(Divider),typeof(SetextHeader),
                typeof(Header),
                typeof(Tag),typeof(Markdown)                
            });

        /// <summary>
        /// 多行樣式
        /// </summary>
        public static List<Type> AreaTypes = new List<Type>(
            new Type[] {
                typeof(List),
                typeof(Blockquotes),
                typeof(CodeArea)
            });

        /// <summary>
        /// 將Markdown字串轉換為Markdown Content
        /// </summary>
        /// <param name="text">Markdown本文</param>
        /// <param name="inline">是否為單一行</param>
        /// <returns>Markdown Content</returns>
        public static Markdown Parse(string text, bool inline = false) {
            List<MarkdownRaw> result = new List<MarkdownRaw>();
            text = text.Replace("\r","");

            var types = inline ? InlineTypes : AreaTypes.Concat(LineTypes);

            while (text.Length>0) {
                int skip = 0;
                //剖析引用參數
                object[] args = new object[] { text, 0 };

                //範圍樣式與行樣式檢查
                foreach (var type in types) {
                    if (!MatchAttribute.IsMatch(type, text)) continue;

                    var parseMethod = type.GetTypeInfo().GetMethod("Parse");
                    result.Add((MarkdownRaw)parseMethod.Invoke(null, args));

                    
                    skip = (int)args[1];
                    break;
                }
                if (inline && skip == 0) {//行內檢驗
                    skip = 1;
                    string addChar = text[0].ToString();
                    if(text[0] == '\\') {
                        skip = 2;
                        addChar = new string(text.Take(2).ToArray());
                    }
                    var lastChild = result.LastOrDefault();
                    if (lastChild?.GetType() == typeof(MarkdownRaw)) {
                        lastChild.OuterMarkdown += addChar;
                    }else {
                        result.Add(new MarkdownRaw() { OuterMarkdown = addChar });
                    }
                }
                text = new string(text.Skip(skip).ToArray());
            }
            var resultInstace = new Markdown() { Children = result };//.Where(x=>x.OuterMarkdown.Length != 0).ToList();
            if (!resultInstace.IsSingleLine) {//多行實體
                resultInstace.Children = FilterNewLine(resultInstace.Children);
            }
            return resultInstace;
        }

        private static List<MarkdownRaw> FilterNewLine(List<MarkdownRaw> children) {
            List<MarkdownRaw> result = new List<MarkdownRaw>();

            for (int i = 0; i < children.Count; i++) {
                if (children[i].GetType() != typeof(Markdown) ||
                    i == result.Count - 1) {
                    result.Add(children[i]);
                    continue;
                }
                MarkdownRaw last = children[i];
                var lines = children.Skip(i + 1).TakeWhile(
                    x => {
                        var check = last.GetType() == typeof(Markdown) &&
                            ((Markdown)last).Children.Count != 0;
                        last = x;
                        return check;
                    }).ToList() ;
                lines.Insert(0, children[i]);
                i += lines.Count() - 1;

                if (lines.Count > 1 && lines.Last().OuterMarkdown.Length == 0) {
                    lines.RemoveAt(lines.Count - 1);
                }                
                if (lines.Count > 1) {
                    result.Add(new Markdown() { Children = lines });
                }else {
                    result.Add(lines.First());
                }
            }

            return result;
        }


        internal static string ToMarkdown(List<MarkdownRaw> contents, bool inline=false) {
            string result = "";
            foreach (var content in contents) {
                result += content.OuterMarkdown;
                if (!inline) {
                    result += "\n";
                    if(content.GetType() == typeof(Markdown) &&
                       content.OuterMarkdown.Length > 0) {
                        result += "\n";
                    }
                }
            }
            return result;
        }
        internal static string ToText(List<MarkdownRaw> contents, bool inline = false) {
            string result = "";
            foreach (var content in contents) {
                result += content.OuterText;
                if (!inline) {
                    result += "\n";
                }
            }
            return result;
        }
    }
}
