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

        protected Match GetMatch(string text) {
            return MatchAttribute.GetMatchAttributes<ListItem>()
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


        public static List<Type> InlineTypes = new List<Type>(
            new Type[] {
                typeof(AutoLink), typeof(Bold),
                typeof(Code),typeof(Delete),
                typeof(Italic),typeof(Link),
                typeof(Image)
            });

        public static List<Type> LineTypes = new List<Type>(
            new Type[] {
                //typeof(BlockquotesItem),
                typeof(Divider),typeof(Header),
                typeof(ListItem),typeof(Tag),
                typeof(Content)
            });

        public static List<Type> AreaTypes = new List<Type>(
            new Type[] {
                typeof(List),
                typeof(Blockquotes)
            });
        public static List<ContentBase> InlineP0arse(string text) {
            List<ContentBase> result = new List<ContentBase>();
            string temp = text;
            while (temp.Length > 0) {
                bool check = false;
                int skip = 1;
                if (temp.FirstOrDefault() == '\\') {//溢出字元
                    skip = 2;
                } else if (temp.FirstOrDefault() == '\n') {
                    skip = 1;
                    check = true;
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
                        last.OuterMarkdown += temp.Substring(0, skip);
                    } else {
                        last = new ContentBase() { OuterMarkdown = temp.Substring(0, skip) };
                        result.Add(last);
                    }
                }
                temp = temp.Substring(skip);
            }
            return result;
        }

        private static List<ContentBase> LineParse(string text) {
            List<ContentBase> result = new List<ContentBase>();
            string temp = text;
            while (temp.Length > 0) {
                bool check = false;
                int skip = 1;
                if (temp.FirstOrDefault() == '\\') {//溢出字元
                    skip = 2;
                } else if (temp.FirstOrDefault() == '\n') {
                    skip = 1;
                    check = true;
                } else {
                    foreach (var type in LineTypes) {
                        if (!MatchAttribute.IsMatch(type, temp)) continue;

                        var parseMethod = type.GetTypeInfo().GetMethod("Parse");

                        var args = new object[] { temp };
                        result.Add((ContentBase)parseMethod.Invoke(null, args));
                        return result;
                    }

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
                        last.OuterMarkdown += temp.Substring(0, skip);
                    } else {
                        last = new ContentBase() { OuterMarkdown = temp.Substring(0, skip) };
                        result.Add(last);
                    }
                }
                temp = temp.Substring(skip);
            }
            return result;
        }

        public static List<ContentBase> AreaParse(string text) {
            List<ContentBase> result = new List<ContentBase>();
            string temp = text;
            while (temp.Length > 0) {
                bool check = false;
                int skip = 1;
                if (temp.FirstOrDefault() == '\\') {//溢出字元
                    skip = 2;
                /*}else if (temp.FirstOrDefault() == '\n') {
                    skip = 1;
                    check = true;*/
                } else {
                    foreach (var type in AreaTypes) {
                        if (!MatchAttribute.IsMatch(type, temp)) continue;

                        var parseMethod = type.GetTypeInfo().GetMethod("Parse");

                        var args = new object[] { temp ,0};
                        result.Add((ContentBase)parseMethod.Invoke(null, args));

                        skip = (int)args[1];
                        check = true;
                        break;
                    }
                    if (!check) {
                        foreach (var type in LineTypes) {
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
                }
                if (!check) {
                    var last = result.LastOrDefault();
                    if (last?.GetType() == typeof(ContentBase)) {
                        last.OuterMarkdown += temp.Substring(0, skip);
                    } else {
                        last = new ContentBase() { OuterMarkdown = temp.Substring(0, skip) };
                        result.Add(last);
                    }
                }
                temp = new string(temp.Skip(skip).ToArray());//.Substring(skip);
            }
            
            return result.Where(x=>x.OuterMarkdown.Trim().Length != 0).ToList();
        }



        public static List<ContentBase> Parse(string text, bool inline = false) {
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
            return result.Where(x=>x.OuterMarkdown.Length != 0).ToList();
        }
    }
}
