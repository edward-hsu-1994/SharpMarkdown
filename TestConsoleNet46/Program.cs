using SharpMarkdown;
using SharpMarkdown.Area;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleNet46 {
    class Program {
        public static void Main(string[] args) {
            var text = System.IO.File.ReadAllText("test.md");
            var mdContent = Markdown.Parse(text);
            var mdSection = mdContent.ToSection();

            var tag1 = mdSection.FindTag("1");

            //顯示章節結構
            var temp = SegmentsList(mdSection);
            foreach (var line in temp) {
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
