using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using SharpMarkdown.Inline;
using ExpressionReflection;

namespace SharpMarkdown {
    /// <summary>
    /// Markdown內容
    /// </summary>
    public class Content {
        /// <summary>
        /// 完整Markdown片段
        /// </summary>
        public virtual string OuterMarkdown { get; set; }
        
        /// <summary>
        /// 隱含轉換<see cref="string"/>為<see cref="Content"/>內容
        /// </summary>
        /// <param name="markdown">Markdown內容</param>
        public static implicit operator Content(string markdown) {
            if (markdown == null) return null;
            return new Content() { OuterMarkdown = markdown };
        }

        /// <summary>
        /// 明確轉換<see cref="Content"/>為<see cref="string"/>
        /// </summary>
        /// <param name="content">string本文</param>
        public static explicit operator string(Content content) {
            return content?.OuterMarkdown;
        }


        public static List<Type> InlineTypes = new List<Type>(
            new Type[] {
                typeof(Bold),typeof(Link)
            });
        public static List<Content> InlineParse(string text) {
            List<Content> result = new List<Content>();
            string temp = text;
            while(temp.Length > 0) {
                bool check = false;
                int skip = 1;
                foreach (var type in InlineTypes) {
                    if (!MatchAttribute.IsMatch(type, temp)) continue;

                    var parseMethod = type.GetTypeInfo().GetMethod("Parse");

                    var args = new object[] { temp, 0 };
                    result.Add((Content)parseMethod.Invoke(null, args));
                    skip = (int)args[1];

                    check = true;
                    break;
                }
                if (!check) { 
                    var last = result.LastOrDefault();
                    if (last?.GetType() == typeof(Content)) {
                        last.OuterMarkdown += temp.First();
                    } else {
                        last = new Content() { OuterMarkdown = temp.First().ToString() };
                        result.Add(last);
                    }
                }
                temp = temp.Substring(skip);
            }
            return result;
        }
    }
}
