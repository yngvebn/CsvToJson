using System;

namespace CsvToJson
{
    public class Feedback
    {
        public int Number { get; set; }
        public string User { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string Country { get; set; }
        public string SourceUrl { get; set; }
        public string Device { get; set; }
        public string Browser { get; set; }
        public string OS { get; set; }
        public string ScreenshotUrl { get; set; }
        public int Emotion { get; set; }
        public string Message { get; set; }
    }
}