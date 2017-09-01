using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace CsvToJson
{
    public class BrowserSetup: IDisposable
    {
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

            //options.AddArguments("-incognito");
            options.Proxy = null;

            DesiredCapabilities capabilities = DesiredCapabilities.Chrome();
            capabilities.SetCapability(ChromeOptions.Capability, options);

            var chromedriverChromedriverExe = Path.GetFullPath(@"..\..\ChromeDriver\");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(chromedriverChromedriverExe);
            //service.Port = 60600;
            return new ChromeDriver(service, options);
        }
        
        public void Dispose()
        {
            KillAnyUsedBrowsers();
            WebDriver?.Dispose();
        }
    }
}