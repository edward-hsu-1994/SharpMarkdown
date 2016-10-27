using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Inline {
    public class Link : Content {
        /// <summary>
        /// 文字
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 連結
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 參考標籤
        /// </summary>
        public Tag ReferenceTag { get; set; }

        public override string OuterMarkdown {
            get {
                string result = $"[{Text}]";
                if (string.IsNullOrWhiteSpace(URL)) {
                    result += $"[{ReferenceTag.Id}]";
                } else {
                    result += $"[{URL}]";
                }
                return result;
            }
            set {
                base.OuterMarkdown = value;
            }
        }

        public static Link Parse(string text) {
            try {
                Regex tagLink = new Regex(@"\[.+\]\s*\[.+\]");
                Regex urlLink = new Regex(@"\[.+\]\s*\(.+\)");

                string linkText =
                    tagLink.Match(text)?.Value ??
                    urlLink.Match(text)?.Value;

                Regex t1Regex = new Regex(@"\[.+\]");
                Regex t2Regex = new Regex(@"\(.+\)");
                Link result = new Link();
                result.Text = t1Regex.Match(linkText).Value;
                result.Text = result.Text.Substring(1, result.Text.Length - 2);

                if (tagLink.IsMatch(text)) {
                    string tagId = t1Regex.Match(linkText).NextMatch().Value.Trim();
                    tagId = tagId.Substring(1, tagId.Length - 2);

                    result.ReferenceTag = new Tag() { Id = tagId.Trim(), IsRef = true };
                } else if (urlLink.IsMatch(text)) {
                    string url = t2Regex.Match(linkText).Value;
                    url = url.Substring(1, url.Length - 2);

                    result.URL = url.Trim();
                }
                return result;
            }catch {
                throw new FormatException();
            }
        }
    }
}
