using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMarkdown {
    /// <summary>
    /// Markdown內容
    /// </summary>
    public class Content {
        /// <summary>
        /// 完整Markdown片段
        /// </summary>
        public virtual string OuterMarkdown { get; set; }
        
        /// <summary>
        /// 隱含轉換<see cref="string"/>為<see cref="Content"/>內容
        /// </summary>
        /// <param name="markdown">Markdown內容</param>
        public static implicit operator Content(string markdown) {
            if (markdown == null) return null;
            return new Content() { OuterMarkdown = markdown };
        }

        /// <summary>
        /// 明確轉換<see cref="Content"/>為<see cref="string"/>
        /// </summary>
        /// <param name="content">string本文</param>
        public static explicit operator string(Content content) {
            return content?.OuterMarkdown;
        }
    }
}
