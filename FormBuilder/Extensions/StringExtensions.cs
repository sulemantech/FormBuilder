using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using FormBuilder.Helpers;
using System.Text;

namespace FormBuilder.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex EmailExpression = new Regex(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Substring but OK if shorter
        /// </summary>
        public static string Limit(this string str, int characterCount)
        {
            if (str.Length <= characterCount) return str;
            else return str.Substring(0, characterCount).TrimEnd(' ');
        }

        public static bool IsNumeric(this string value)
        {
            float num;
            return float.TryParse(value, out num);
        }

        public static string AppendIfDebugMode(this string val, string toAppend)
        {
            if (UtilityHelper.IsDebugMode())
            {
                return val.ConcatWith(toAppend);
            }

            return val;
        }

        public static int IsInt(this string val, int elseReturn = 0)
        {
            int toReturn = 0;
            if (int.TryParse(val, out toReturn))
            {
                return toReturn;
            }

            return elseReturn;
        }

        public static bool IsInteger(this string val)
        {
            int toReturn = 0;
            if (int.TryParse(val, out toReturn))
            {
                return true;
            }

            return false;
        }

        public static bool IsBool(this string val, bool elseReturn = false)
        {
            bool toReturn = false;
            if (bool.TryParse(val, out toReturn))
            {
                return toReturn;
            }

            return elseReturn;
        }


        /// <summary>
        /// Substring with elipses but OK if shorter, will take 3 characters off character count if necessary
        /// </summary>
        public static string LimitWithElipses(this string str, int characterCount)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (characterCount < 5) return str.Limit(characterCount);       // Can’t do much with such a short limit
                if (str.Length <= characterCount - 3) return str;
                else return str.Substring(0, characterCount - 3) + "...";

            }
            return "";
        }

        public static bool IsValidEmail(this string target)
        {
            return !string.IsNullOrEmpty(target) && EmailExpression.IsMatch(target);
        }

        public static string ToTitleCase(this string target)
        {
            var currCulture = Thread.CurrentThread.CurrentCulture;
            var tInfo = currCulture.TextInfo;
            return tInfo.ToTitleCase(target.ToLower());
        }
        
        public static string FormatWith(this string target, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, target, args);
        }

        public static IHtmlString ToHtmlString(this string target)
        {
            return new HtmlString(target);
        }

        public static string ToJson(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public static T FromJson<T>(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(obj as string);
        }

        public static string ConcatWith(this string value, params string[] values)
        {
            return value + string.Concat(values);
        }

        /// <summary>
        /// Transforms the string into a URL-friendly slug
        /// </summary>
        /// <param name="name">The original string</param>
        /// <returns>A string containing a url-friendly slug</returns>
        public static string ToSlug(this string name)
        {
            var sb = new StringBuilder();
            string lower = string.IsNullOrEmpty(name) ? "" : name.ToLower();
            foreach (char c in lower)
            {
                if (c == ' ' || c == '.' || c == '=' || c == '-')
                    sb.Append('-');
                else if ((c <= 'z' && c >= 'a') || (c <= '9' && c >= '0'))
                    sb.Append(c);
            }

            return sb.ToString().Trim('-');
        }

        public static string ToUnorderedList(this IList<string> target)
        {
            StringBuilder sb = new StringBuilder("<ul>");
            target.Each((str, index) =>
            {
                sb.AppendFormat("<li>{0}</li>", str);
            });
            sb.Append("</ul>");
            return sb.ToString();
        }

        public static string[] Split(this string value, string regexPattern, RegexOptions options)
        {
            return Regex.Split(value, regexPattern, options);
        }

        public static string[] Split(this string value, string regexPattern)
        {
            return value.Split(regexPattern, RegexOptions.None);
        }

        public static bool IsTheSameAs(this string target, string stringToCompare)
        {
            return IsTheSameAs(target, stringToCompare, true);
        }

        public static bool IsTheSameAs(this string target, string stringToCompare, bool ignoreCase)
        {
            return string.Compare(target, stringToCompare, ignoreCase) == 0;
        }

        public static string OutputIfIs(this string target, string matchValue, string valueToOutput, string elseOutput = "")
        {
            if (StringExtensions.IsTheSameAs(target, matchValue))
            {
                return valueToOutput;
            }

            return elseOutput;
        }

        public static string OutputIfNot(this string target, string matchValue, string valueToOutput, string elseOutput = "")
        {
            if (!StringExtensions.IsTheSameAs(target, matchValue))
            {
                return valueToOutput;
            }

            return elseOutput;
        }

        public static string Pluralize(this string singular, int count)
        {
            IList<string> Unpluralizables = new List<string> { "equipment", "information", "rice", "money", "species", "series", "fish", "sheep", "deer" };
            IDictionary<string, string> Pluralizations = new Dictionary<string, string>
            {
                // Start with the rarest cases, and move to the most common
                { "person", "people" },
                { "ox", "oxen" },
                { "child", "children" },
                { "foot", "feet" },
                { "tooth", "teeth" },
                { "goose", "geese" },
                // And now the more standard rules.
                { "(.*)fe?", "$1ves" },         // ie, wolf, wife
                { "(.*)man$", "$1men" },
                { "(.+[aeiou]y)$", "$1s" },
                { "(.+[^aeiou])y$", "$1ies" },
                { "(.+z)$", "$1zes" },
                { "([m|l])ouse$", "$1ice" },
                { "(.+)(e|i)x$", @"$1ices"},    // ie, Matrix, Index
                { "(octop|vir)us$", "$1i"},
                { "(.+(s|x|sh|ch))$", @"$1es"},
                { "(.+)", @"$1s" }
            };

            if (count == 1)
                return singular;

            if (Unpluralizables.Contains(singular))
                return singular;

            var plural = "";

            foreach (var pluralization in Pluralizations)
            {
                if (Regex.IsMatch(singular, pluralization.Key))
                {
                    plural = Regex.Replace(singular, pluralization.Key, pluralization.Value);
                    break;
                }
            }

            return plural;
        }
       


    }
}