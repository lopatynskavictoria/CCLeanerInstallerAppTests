using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCLeanerInstallerAppTests.PageObjects;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CCLeanerInstallerAppTests
{
    public class Actions
    {
        private static string _downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        public static bool DownloadCCleanerUsingChromeBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("start-maximized");
            IWebDriver driver = new ChromeDriver(options);

            try
            {
                // Navigate to the CCleaner download page
                driver.Navigate().GoToUrl("https://www.ccleaner.com/ccleaner/download");

                // Find the download button and click it
                IWebElement downloadButton = driver.FindElement(By.ClassName("download-btn"));
                downloadButton.Click();


                // Wait for the download to complete
                WaitUntilFileDownloaded(_downloadsPath);

                // Check that the downloaded file exists and its name correct
                string[] downloadedFiles = Directory.GetFiles(_downloadsPath, "ccsetup*.exe");
                if (downloadedFiles.Length == 0)
                {
                    throw new Exception("Downloaded file not found.");
                }

                // Output the path to the downloaded file
                Console.WriteLine($"Downloaded file path: {downloadedFiles[0]}");
                return true;
            }
            catch
            {
                return false;
            }
            finally {}
            {
                // Quit the Chrome driver
                driver.Quit();
            }
        }

        private static void WaitUntilFileDownloaded(string downloadDirectory)
        {
            // Wait for up to 120 seconds for a file to be downloaded
            for (int i = 0; i < 120; i++)
            {
                var downloadedFiles = Directory.GetFiles(downloadDirectory);
                if (downloadedFiles.Length > 0)
                {
                    return;
                }
                Thread.Sleep(1000);
            }

            throw new Exception($"Download did not complete within 120 seconds.");
        }

        public static string GetInstallerWindowTitle()
        {
                var app = RunInstaller();
                var automation = new UIA3Automation();
                var window = app.GetMainWindow(automation);
                return window.Title;
        }

        public static bool TryToFindProposedInstallationLocationOnMainPage()
        {
            var app = RunInstaller();
            var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);
            try
            {
                var locationElement = window.FindFirstDescendant(cf => cf.ByAutomationId("installLocationTextBox"));
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static string GetProposedInstallationLocationFromMoreOptionsMenu()
        {
            var app = RunInstaller();
            var automation = new UIA3Automation();
            var window = app.GetMainWindow(automation);
            var customizeButton = window.FindFirstDescendant(cf => cf.ByText("Customize"));
            customizeButton.Click();

            var moreButton = window.FindFirstDescendant(cf => cf.ByText("More"));
            moreButton.Click();

            //click on more link
            var locationElement = window.FindFirstDescendant(cf => cf.ByAutomationId("installLocationTextBox"));
            var locationValue = locationElement.AsTextBox().Text;
            return locationValue;
        }


        private static Application RunInstaller()
        {
            string InstallerPath = Directory.GetFiles(_downloadsPath, "ccsetup*.exe")[0];
            var app = Application.Launch(InstallerPath);
            return app;
        }
    }
}
