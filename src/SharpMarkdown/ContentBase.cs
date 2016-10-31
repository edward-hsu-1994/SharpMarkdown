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
    public class ContentBase {
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
        /// 隱含轉換<see cref="string"/>為<see cref="ContentBase"/>內容
        /// </summary>
        /// <param name="markdown">Markdown內容</param>
        public static implicit operator ContentBase(string markdown) {
            if (markdown == null) return null;
            return new ContentBase() { OuterMarkdown = markdown };
        }

        /// <summary>
        /// 明確轉換<see cref="ContentBase"/>為<see cref="string"/>
        /// </summary>
        /// <param name="content">string本文</param>
        public static explicit operator string(ContentBase content) {
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
                //typeof(BlockquotesItem),
                typeof(Divider),typeof(SetextHeader),
                typeof(Header),
                typeof(Tag),typeof(Content)                
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
        public static Content Parse(string text, bool inline = false) {
            List<ContentBase> result = new List<ContentBase>();
            text = text.Trim().Replace("\r","");

            var types = inline ? InlineTypes : AreaTypes.Concat(LineTypes);

            while (text.Length>0) {
                int skip = 0;
                //剖析引用參數
                object[] args = new object[] { text, 0 };

                //範圍樣式與行樣式檢查
                foreach (var type in types) {
                    if (!MatchAttribute.IsMatch(type, text)) continue;

                    var parseMethod = type.GetTypeInfo().GetMethod("Parse");
                    result.Add((ContentBase)parseMethod.Invoke(null, args));

                    
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
                    if (lastChild?.GetType() == typeof(ContentBase)) {
                        lastChild.OuterMarkdown += addChar;
                    }else {
                        result.Add(new ContentBase() { OuterMarkdown = addChar });
                    }
                }
                text = new string(text.Skip(skip).ToArray());
            }
            return new Content() { Children = result };//.Where(x=>x.OuterMarkdown.Length != 0).ToList();
        }
        
        internal static string ToMarkdown(List<ContentBase> contents, bool inline=false) {
            string result = "";
            foreach (var content in contents) {
                result += content.OuterMarkdown;
                if (!inline) {
                    result += "\n";
                }
            }
            return result;
        }
        internal static string ToText(List<ContentBase> contents, bool inline = false) {
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
