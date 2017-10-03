using System.Configuration;
using System.Security.Cryptography;

namespace CsvToJson
{
    public class Settings
    {
        public const string Password = "FpW-U2-N";

#if DEBUG
        public static string SaveLocation => ConfigurationManager.AppSettings["saveLocation"];
#else
           public static string SaveLocation => ConfigurationManager.AppSettings["saveLocationProd"];
#endif
    }
}