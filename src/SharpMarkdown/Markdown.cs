using SharpMarkdown.Area;
using SharpMarkdown.Attributes;
using SharpMarkdown.Inline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown {
    [Match(Regex = ".+")]
    public class Markdown : MarkdownRaw{
        public List<MarkdownRaw> Children { get; set; } = new List<MarkdownRaw>();

        internal bool IsSingleLine {
            get {
                return Children.All(x => {
                    var NS = x.GetType().Namespace.Split('.').Last();
                    return NS != nameof(Area) && NS != nameof(Line);
                });
            }
        }

        public override string OuterMarkdown {
            get {
                return ToMarkdown(Children, IsSingleLine);
            }
            set {
                Children = MarkdownRaw.Parse(value.Trim()).Children;
            }
        }
        public override string OuterText {
            get {
                return ToText(Children, IsSingleLine);
            }
        }
        public override string InnerMarkdown {
            get {
                return OuterMarkdown;
            }
        }
        public override string InnerText {
            get {
                return OuterText;
            }
        }

        /// <summary>
        /// 找尋指定的子節點
        /// </summary>
        /// <typeparam name="T">節點類型</typeparam>
        /// <param name="func">條件方法</param>
        /// <returns>找尋結果</returns>
        public T Find<T>(Func<T, bool> func) where T : MarkdownRaw {
            T result = default(T);
            foreach (var child in Children) {
                if (child is T && func((T)child)) {
                    result = (T)child;
                } else if (child is Section) {
                    result = result ?? ((Section)child).Find(func);
                }
                if (result != null) break;
            }
            return result;
        }

        /// <summary>
        /// 轉換為Section
        /// </summary>
        /// <returns>Section</returns>
        public Section ToSection() {
            return Section.FromContent(this);
        }
        public static Markdown Parse(string text, out int length) {
            var lines = text.Split('\n');
            var temp = lines.FirstOrDefault() ?? "";
            length = temp.Length + 1;
            Markdown result = new Markdown();
            result.Children = MarkdownRaw.Parse(temp,true).Children;
            return result;
        }
    }
}
