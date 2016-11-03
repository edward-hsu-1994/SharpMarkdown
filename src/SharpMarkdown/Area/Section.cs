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

        /// <summary>
        /// 找尋指定ID的參考標籤
        /// </summary>
        /// <param name="id">參考標籤ID</param>
        /// <returns>Tag</returns>
        public Tag FindTag(string id) {
            return Find<Tag>(x => x.Id.ToLower() == id.ToLower());
        }

        /// <summary>
        /// 找尋指定名稱的章節
        /// </summary>
        /// <param name="header">章節名稱</param>
        /// <returns>章節</returns>
        public Section FindSection(string header) {
            return Find<Section>(x => x.HeaderText == header);
        }

        /// <summary>
        /// 找尋指定的子節點
        /// </summary>
        /// <typeparam name="T">節點類型</typeparam>
        /// <param name="func">條件方法</param>
        /// <returns>找尋結果</returns>
        public T Find<T>(Func<T,bool> func) where T : MarkdownRaw {
            T result = default(T);
            foreach(var child in Children) {
                if(child is T && func((T)child)) {
                    result = (T)child;
                }else if(child is Section){
                    result = result ?? ((Section)child).Find(func);
                }
                if (result != null) break;
            }
            return result;
        }

        /// <summary>
        /// 結構檢驗方式
        /// </summary>
        public enum MatchModes {
            /// <summary>
            /// 標準結構為基本需求(不檢查順序與多餘的項目)
            /// </summary>
            Basic,
            /// <summary>
            /// 基於Base檢查後加上章節順序檢查(可多餘項目)
            /// </summary>
            Order,
            /// <summary>
            /// 結構與順序必須完全符合
            /// </summary>
            Full
        }

        /// <summary>
        /// 檢查是否符合指定標準章節結構
        /// </summary>
        /// <param name="standard">標準結構</param>
        /// <param name="mode">檢查模式</param>
        /// <returns>是否符合結構</returns>
        public bool IsMatch(Section standard,MatchModes mode = MatchModes.Basic) {
            var subsections = this.Children.Where(x => x is Section).Select(x=>(Section)x).ToList<Section>();
            var standardSubsections = standard.Children.Where(x => x is Section).Select(x => (Section)x).ToList<Section>();

            //嚴格模式必須數量也一樣
            if (mode == MatchModes.Full && subsections.Count != standardSubsections.Count) return false;
            
            //去除不再標準內的章節
            subsections = subsections.Where(x => standardSubsections.Any(y => y.HeaderText == x.HeaderText)).ToList();

            //標題不符合或章節數量不符合
            if (standard.HeaderText != this.HeaderText ||
                subsections.Count != standardSubsections.Count) return false;

            //無子章節
            if (standardSubsections.Count() == 0) return true;

            //順序檢驗
            if(mode != MatchModes.Basic) {
                for(int i = 0;
                    i < standardSubsections.Count;
                    i++) {
                    if (standardSubsections[i].HeaderText ==
                        subsections[i].HeaderText &&
                        subsections[i].IsMatch(standardSubsections[i])) {
                        continue;
                    }
                    return false;
                }
                return true;
            }

            //基礎檢驗
            return standardSubsections
                .All(x => subsections.Any(
                    y => y.HeaderText == x.HeaderText &&
                    y.IsMatch(x)
                ));
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
