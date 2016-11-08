using SharpMarkdown;
using SharpMarkdown.Area;
using SharpMarkdown.Inline;
using SharpMarkdown.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestConsole {
    public class Program {
        public static void Main(string[] args) {
            var text = System.IO.File.ReadAllText("test.md");
            var standardText = System.IO.File.ReadAllText("standard.md");

            var mdDocument = Markdown.Parse(text);
            var mdSections = mdDocument.ToSection();

            //讀取作為標準的章節結構
            var stDoc = Markdown.Parse(standardText).ToSection();
            var temp2 = stDoc.HeaderText;
            //基礎結構檢驗(標準有的都必須有)
            bool isMatchBase = mdSections.IsMatch(stDoc);

            //基礎結構檢驗加上順序檢驗(章節順序需要跟標準一樣)
            bool isMatchOrder = mdSections.IsMatch(stDoc, Section.MatchModes.Order);

            //章節的順序、結構都要完全一樣(標準有的都要有，沒有的不能有)
            bool isMatchFull = mdSections.IsMatch(stDoc, Section.MatchModes.Full);
            
            var tag1 = mdSections.FindTag("1");

            //顯示章節結構
            var temp = SegmentsList(mdSections);
            foreach(var line in temp) {
                Console.WriteLine(line);
            }
            Console.ReadKey();
        }

        public static string[] SegmentsList(Section section, int level = 0) {
            List<string> result = new List<string>();

            foreach (var children in section.Children) {
                var child = children as Section;
                if (child == null || child.HeaderText?.Length == 0) continue;
                string[] nextLevel = SegmentsList(child, level + 1);
                result.AddRange(nextLevel
                    .Where(x => x != null)
                    .Select(x => ' ' + x));
            }
            if (result.Count != 0 || section.HeaderText?.Length != 0) {
                result.Insert(0, section.HeaderText);
            }
            return result.ToArray();
        }
    }
}
