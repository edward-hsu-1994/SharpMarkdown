using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using SharpMarkdown.Inline;
using SharpMarkdown.Line;

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


        public static List<Type> InlineTypes = new List<Type>(
            new Type[] {
                typeof(AutoLink), typeof(Bold),
                typeof(Code),typeof(Delete),
                typeof(Italic),typeof(Link),
                typeof(Image)
            });

        public static List<Type> LineTypes = new List<Type>(
            new Type[] {
                typeof(Blockquotes),typeof(Divider),typeof(Header),
                typeof(ListItem),typeof(Tag)
            });
        public static List<ContentBase> InlineParse(string text) {
            List<ContentBase> result = new List<ContentBase>();
            string temp = text;
            while(temp.Length > 0) {
                bool check = false;
                int skip = 1;
                if (temp.FirstOrDefault() == '\\') {//溢出字元
                    skip = 2;
                } else {
                    foreach (var type in InlineTypes) {
                        if (!MatchAttribute.IsMatch(type, temp)) continue;

                        var parseMethod = type.GetTypeInfo().GetMethod("Parse");

                        var args = new object[] { temp, 0 };
                        result.Add((ContentBase)parseMethod.Invoke(null, args));
                        skip = (int)args[1];

                        check = true;
                        break;
                    }
                }
                if (!check) { 
                    var last = result.LastOrDefault();
                    if (last?.GetType() == typeof(ContentBase)) {
                        last.OuterMarkdown += temp.Substring(0,skip);
                    } else {
                        last = new ContentBase() { OuterMarkdown = temp.Substring(0,skip) };
                        result.Add(last);
                    }
                }
                temp = temp.Substring(skip);
            }
            return result;
        }
    }
}
