using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;

namespace CsvToJson
{
    public class BrowserSetup: IDisposable
    {
        public const string DownloadDirectory = @"C:\PhantomJS";
        public IWebDriver WebDriver { get; }

        public BrowserSetup()
        {
            WebDriver = GetWebDriver();
        }
        
        public void KillAnyUsedBrowsers()
        {
            var driver = WebDriver;

            if (driver != null)
            {
                try
                {
                    driver.Manage().Cookies.DeleteAllCookies();
                }
                finally
                {
                    driver.Quit();
                }
            }
        }

        

        private IWebDriver GetWebDriver()
        {
            ChromeOptions options = new ChromeOptions();

            options.AddArguments("--disable-extensions");
            options.AddArguments("--disable-extensions-file-access-check");
            options.AddArguments("--disable-extensions-http-throttling");
            options.AddArguments("--disable-gpu");
            options.AddArguments("--disable-infobars");
            options.AddArguments("--disable-notifications");
            options.AddArguments("--enable-automation");
            options.AddArguments("--start-maximized");
            options.AddUserProfilePreference("download.default_directory", DownloadDirectory);
            options.AddUserProfilePreference("download.default_directory", DownloadDirectory);
            //options.AddArguments("-incognito");
            options.Proxy = null;

            var chromedriverChromedriverExe = Path.GetFullPath(@"..\..\Driver\");
            
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(chromedriverChromedriverExe);
            //service.Port = 60600;
            return new ChromeDriver(service, options);
        }

        public static IWebDriver PhantomJsWebDriver()
        {
            var chromedriverChromedriverExe = Path.GetFullPath(@"..\..\Driver\");

            PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService(chromedriverChromedriverExe);
            service.Port = 60601;
            service.LocalStoragePath = DownloadDirectory;
            service.AddArgument("test-type");
            PhantomJSOptions options = new PhantomJSOptions();

            options.AddAdditionalCapability("phantomjs.page.settings.userAgent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");


            return new PhantomJSDriver(service, options);
        }

        public void Dispose()
        {
            KillAnyUsedBrowsers();
            WebDriver?.Dispose();
        }
    }
}