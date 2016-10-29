using SharpMarkdown;
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
            var p = Blockquotes.Parse("> 哈哈哈哈\\***![XPY](https://ggg)**<http://autolink.org>");
        }
    }
}
