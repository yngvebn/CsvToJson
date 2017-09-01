using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CsvToJson
{
    public class CsvToJson
    {
        public static List<T> Parse<T>(string file)
        {
            var allLines = File.ReadAllLines(file);
            var headers = Regex.Matches(allLines[0], "\\\"(.*?)\\\",");
            List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();
            for (int i = 1; i < allLines.Length; i++)
            {
                Dictionary<string, string> thisDict = new Dictionary<string, string>();
                MatchCollection values = Regex.Matches(allLines[i], "\\\"(.*?)\\\",");
                for (int v = 0; v < values.Count; v++)
                {
                    thisDict.Add(Regex.Replace(headers[v].Groups[1].Value, @"[\W\d]", ""), values[v].Groups[1].Value);
                }
                returnList.Add(thisDict);
            }

            return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(returnList));
        }
    }
}