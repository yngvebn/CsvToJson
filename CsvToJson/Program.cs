using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CsvToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"c:\dev\poll.csv";
            if (args.Length > 0)
            {
                fileName = args[0];
            }

            var lines = new Queue<string>(FixLinesAndSplit(File.ReadAllLines(fileName)));

            string[] headers = GetFields(lines.Dequeue()).Select(ToValidIdentifier).ToArray();
            List<Dictionary<string, string>> allItems = new List<Dictionary<string, string>>();
            while (lines.Count > 0)
            {
                allItems.Add(CreateDictionaryItem(headers, GetFields(lines.Dequeue()).ToList()));
            }
            var json = JsonConvert.SerializeObject(allItems);
            var jsonFileName = Path.GetFileNameWithoutExtension(fileName) + ".json";
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(fileName), jsonFileName), json);
        }

        private static IEnumerable<string> FixLinesAndSplit(string[] readAllText)
        {
            string lineMatch = "^\".*\"$";
            List<string> lines = new List<string>();
            string currentLine = "";
            foreach (var line in readAllText)
            {
                if (line.StartsWith("\"") && line.ToCharArray().Count(c => c == '\"') % 2 == 0)
                {
                    currentLine = line;
                }
                else
                {
                    currentLine += line;
                }
                if (currentLine.EndsWith("\""))
                {
                    lines.Add(currentLine);
                    currentLine = line;
                }
            }
            return lines;
        }

        private static Dictionary<string, string> CreateDictionaryItem(string[] headers, List<string> toList)
        {
            Dictionary<string, string> item = new Dictionary<string, string>();
            for (int i = 0; i < headers.Length; i++)
            {
                item.Add(headers[i], toList[i]);
            }
            return item;
        }

        private static IEnumerable<string> GetFields(string str)
        {
            string fieldMatchPattern = "\"(.*?)\",?";
            foreach (Match match in Regex.Matches(str, fieldMatchPattern))
            {
                yield return match.Value;
            }
        }

        private static string ToValidIdentifier(string str)
        {
            string pattern = "[^a-zA-Z]";
            str = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
            str = Regex.Replace(str, pattern, "");
            str = str[0].ToString().ToLower() + str.Substring(1);
            return str;
        }
    }
}
