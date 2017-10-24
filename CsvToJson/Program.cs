using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using Selenium.Webdriver.Domify;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium;
using Selenium.Webdriver.Domify.Elements;

namespace CsvToJson
{
    public class Program
    {

        static void Main(string[] args)
        {
            //using (var browserSetup = new BrowserSetup())
            //{
            //   // browserSetup.WebDriver.Manage().Window.Size = new Size(0, 0);
            //    var d = browserSetup.WebDriver.Document();
            //    Console.WriteLine("Going to Hotjar");
            //    d.Navigation.GoTo("https://insights.hotjar.com/sites/330961/feedback/responses/14493");
            //    Console.WriteLine("Logging in");
            //    d.TextField("email").Text = "yngve.bakken.nilsen@rikstoto.no";
            //    d.TextField("password").Text = Settings.Password;
            //    d.Button(Find.ByText("Sign in")).Click();
            //    Console.WriteLine("Selecting stuff");
            //    d.WaitUntilFound<Div>(By.Id("filter-date")).Div(By.ClassName("dropdown-toggle")).Click();
            //    d.WaitUntilFound<Div>(By.Id("filter-date")).Lists.First().OwnListItems.Last().Links.Single().Click();
            //    System.Threading.Thread.Sleep(1000);
            //    Console.WriteLine("Downloading report");
            //    d.Link(Find.ByText("Download as CSV")).Click();
            //    System.Threading.Thread.Sleep(1000);
            //}

            
            var newestFile = Directory.GetFiles(BrowserSetup.DownloadDirectory, "feedback-14493*.csv").Select(f => new FileInfo(f)).OrderByDescending(f => f.CreationTime).FirstOrDefault();
            if (newestFile != null)
            {
                Console.WriteLine($"Converting {newestFile} to json");
                var json = CsvToJson.Parse<Feedback>(newestFile.FullName).Where(c => c.Emotion > 0).ToList();
                var data = new FeedbackData(json);
                Console.WriteLine($"Got {data.all.Count()} feedback-items");
                string saveFileName = Path.Combine(Settings.SaveLocation, "feedback.json");

                Console.WriteLine($"Saving to {saveFileName}");

                File.WriteAllText(saveFileName, JsonConvert.SerializeObject(data));
#if DEBUG
                Console.WriteLine("Done! Exiting in 3 sec");
                System.Threading.Thread.Sleep(3000);
#endif

            }
            else
            {
                Console.WriteLine("No files downloaded!");
                System.Threading.Thread.Sleep(4000);
            }
        }

        public class FeedbackData
        {
            public int count => all.Count;
            public List<Feedback> all { get; set; }
            public double average => all.Average(f => f.Emotion);

            public object averageByDevice => all.GroupBy(f => f.Device)
                .Select(group => new
                {
                    device = group.Key,
                    average = group.Average(f => f.Emotion)
                });

            public object averageByOS => all.GroupBy(f => Regex.Replace(f.OS, @"[\d\.]", "").Trim())
                .Select(group => new
                {
                    device = group.Key,
                    average = group.Average(f => f.Emotion)
                });


            public object distribution => all.GroupBy(f => f.Emotion)
                .Select(group => new
                {
                    emotion = group.Key,
                    percent = group.Count() * 100F / (double)all.Count
                });

            public FeedbackData(List<Feedback> feedback)
            {
                all = feedback;
            }
        }

        public class FeedbackDistribution
        {
            public FeedbackDistribution(List<Feedback> all)
            {
                ;
            }
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
