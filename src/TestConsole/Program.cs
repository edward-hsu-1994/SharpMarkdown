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
            var mdContent = Markdown.Parse(text);
            var mdSection = mdContent.ToSection();

            var temp = SegmentsList(mdSection);
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
