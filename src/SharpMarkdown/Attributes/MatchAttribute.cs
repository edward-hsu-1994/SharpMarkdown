using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMarkdown.Attributes {
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =true)]
    public class MatchAttribute : Attribute {
        public string Regex { get; set; }


        public Regex GetRegex() => new Regex(this.Regex);

        public static MatchAttribute GetMatchAttribute<T>() {
            return GetMatchAttribute(typeof(T));
        }
        public static MatchAttribute GetMatchAttribute(Type type) {
            return GetMatchAttributes(type).FirstOrDefault();
        }

        public static MatchAttribute[] GetMatchAttributes<T>() {
            return GetMatchAttributes(typeof(T));
        }
        public static MatchAttribute[] GetMatchAttributes(Type type) {
            return type.GetTypeInfo().GetCustomAttributes<MatchAttribute>().ToArray();
        }

        public static bool IsMatch<T>(string text) {
            return IsMatch(typeof(T), text);
        }

        public static bool IsMatch(Type type,string text) {
            return GetMatchAttributes(type).Select(x => x.GetRegex()).ToArray()
                .Any(x=>x.IsMatch(text));
        }
    }
}
