using SharpMarkdown.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Line {
    /// <summary>
    /// 參考標籤
    /// </summary>
    [Match(Regex = @"^(\s*\[[^\]]+\]):\s*.+([\n\s]|(" + "[\"\'\\(].+[\"\'\\)]" + "))?")]
    public class Tag : MarkdownRaw{
        /// <summary>
        /// 唯一識別號
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 超連結
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 選擇項目
        /// </summary>
        public string Optional { get; set; }

        /// <summary>
        /// 是否為參考項目
        /// </summary>
        public bool IsRef { get; set; }
        public override string OuterMarkdown {
            get {
                var result = $"[{Id}]: {URL}";
                if (!string.IsNullOrWhiteSpace(Optional)) {
                    result += $"\"{Optional}\"";
                }
                return result;
            }
            set {
                int temp = 0;
                Tag tag = Parse(value,out temp);
                this.Id = tag.Id;
                this.URL = tag.URL;
                this.Optional = tag.Optional;
            }
        }

        public override string OuterText {
            get {
                return "";
            }
        }

        public static Tag Parse(string text,out int length) {
            try {

                var match = MatchAttribute.GetMatchAttributes<Tag>()
                    .Select(x => new {
                        match = x.GetRegex().IsMatch(text),
                        attr = x
                    }).Where(x => x.match).FirstOrDefault().attr
                    .GetRegex().Match(text);


                Regex idRegex = new Regex(@"\[[^\]]+\]");
                Regex urlRegex = new Regex(":[^\"\'\\(]+[\\n\\s]?");
                Regex optionalRegex = new Regex("[\"\'\\(].+[\"\'\\)]");
                var temp = match.Value;

                string idText = idRegex.Match(temp).Value.Trim();
                idText = idText.Substring(1, idText.Length - 2);

                string urlText = urlRegex.Match(temp).Value.Trim();
                urlText = urlText.Substring(1)
                    .Replace("<","").Replace(">","")
                    .Trim();

                var optionalMatch = optionalRegex.Match(temp);
                string optionalText = null;
                if(optionalMatch.Value.Length > 2) {
                    optionalText = optionalMatch.Value.Trim();
                    optionalText = optionalText.Substring(1, optionalText.Length - 2);
                }
                length = match.Index + match.Length;

                return new Tag() {
                    Id = idText,
                    URL = urlText,
                    Optional = optionalText
                };
            }catch(Exception e) {
                throw new FormatException();
            }
        }
    }
}