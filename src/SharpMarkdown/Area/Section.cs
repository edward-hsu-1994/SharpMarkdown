using SharpMarkdown.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown.Area {
    /// <summary>
    /// 章節
    /// </summary>
    public class Section : Markdown {
        /// <summary>
        /// 標題
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// 標題文字
        /// </summary>
        public string HeaderText {
            get {
                return Header?.OuterText;
            }
        }

        public override string OuterMarkdown {
            get {
                List<MarkdownRaw> temp = new List<MarkdownRaw>(Children);
                if (Header != null) temp.Insert(0, Header);
                return Markdown.ToMarkdown(temp);
            }
        }
        public override string OuterText {
            get {
                List<MarkdownRaw> temp = new List<MarkdownRaw>(Children);
                if (Header != null) temp.Insert(0, Header);
                return Markdown.ToText(temp);
            }
        }

        /// <summary>
        /// 取得所有子章節列表
        /// </summary>
        /// <returns>子章節列表</returns>
        public Section[] GetAllSubsections() {
            List<Section> result = new List<Section>();
            foreach (var section in Children) {
                if (!(section is Section)) continue;
                Section s = (Section)section;
                result.Add(s);
                result.AddRange(s.GetAllSubsections());
            }
            return result.ToArray();
        }

        internal static Section Convert(List<MarkdownRaw> contents, int level = 1) {
            var result = new Section();
            if (contents.Count == 0) return result;

            var indexList = contents.Select((x, i) => new {
                    level = (x is Header) ? ((Header)x).Level : 0,
                    content = x,
                    index = i
                })
                .Where(x => x.level == level)
                .ToList();

            if (indexList.Count == 0) {
                if (level < 6) {
                    return Convert(contents, level + 1);
                }
                result.Children = contents;
                return result;
            }

            if (indexList.First().index != 0) {
                indexList.Insert(0, new {
                    level = level,
                    content = default(MarkdownRaw),
                    index = 0
                });
            }

            for (int i = 0; i < indexList.Count; i++) {
                List<MarkdownRaw> sectionContents = null;

                if (i < indexList.Count - 1) {
                    sectionContents = contents
                        .Skip(indexList[i].index)
                        .Take(indexList[i + 1].index - indexList[i].index)
                        .ToList();
                } else {
                    sectionContents = contents
                        .Skip(indexList[i].index)
                        .ToList();
                }

                var childSection = new Section() {
                    Header = sectionContents.First() as Header
                };
                if (childSection.Header == null) {
                    childSection = Convert(sectionContents.Skip(1).ToList(), level + 1);
                } else {
                    childSection.Children.Add(Convert(sectionContents.Skip(1).ToList(), level + 1));
                }
                result.Children.Add(childSection);
            }
            return result;
        }

        internal static Section ClearSection(Section section) {
            List<MarkdownRaw> newChildren = new List<MarkdownRaw>();
            foreach(var child in section.Children) {
                var sec = child as Section;
                if(sec == null) {
                    newChildren.Add(child);
                }else if(sec.Header == null) {
                    newChildren.AddRange(ClearSection(sec).Children);
                }else {
                    newChildren.Add(ClearSection(sec));
                }
            }
            section.Children = newChildren;
            if (section.Header == null &&
               section.Children.Count == 1 &&
               section.Children.First() is Section) {
                return (Section)section.Children.First();
            }
            return section;
        }
        public static Section FromContent(Markdown content) {
            return ClearSection(Convert(content.Children));
        }

        public static explicit operator Section(List<MarkdownRaw> contents) {
            return ClearSection(Convert(contents));
        }
    }
}
