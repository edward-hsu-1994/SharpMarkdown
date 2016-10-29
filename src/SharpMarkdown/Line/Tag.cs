using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Line {
    public class Tag : ContentBase{
        public string Id { get; set; }
        public string URL { get; set; }
        public string Optional { get; set; }
        internal bool IsRef { get; set; }
        public override string OuterMarkdown {
            get {
                return $"[{Id}]: {URL} \"{Optional}\"";
            }
            set {
                Tag tag = Parse(value);
                this.Id = tag.Id;
                this.URL = tag.URL;
                this.Optional = tag.Optional;
            }
        }

        public static Tag Parse(string text) {
            try {
                Regex idRegex = new Regex(@"\[.+\]");
                Regex urlRegex = new Regex(":[^\"\'\\(]+[\\n\\s]");
                Regex optionalRegex = new Regex("[\"\'\\(].+[\"\'\\)]");

                string idText = idRegex.Match(text).Value.Trim();
                idText = idText.Substring(1, idText.Length - 2);

                string urlText = urlRegex.Match(text).Value.Trim();
                urlText = urlText.Substring(1)
                    .Replace("<","").Replace(">","")
                    .Trim();

                var optionalMatch = optionalRegex.Match(text);
                string optionalText = null;
                if(optionalMatch != null) {
                    optionalText = optionalMatch.Value.Trim();
                    optionalText = optionalText.Substring(1, optionalText.Length - 2);
                }
                return new Tag() {
                    Id = idText,
                    URL = urlText,
                    Optional = optionalText
                };
            }catch {
                throw new FormatException();
            }
        }
    }
}