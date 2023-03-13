using FlaUI.Core;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using static CCLeanerInstallerAppTests.Actions;

namespace CCLeanerInstallerAppTests
{ 
    /// <summary>
    /// Set of 4 test scenarios to check guideline (ACR-002):
    /// App's name is consistent across all points of user interaction: code signing, ads, offers,
    /// landing pages, install locations, and system/browser uninstall names.
    /// </summary>
    [TestClass]
    public class TestsForInstaller
    {
        private static AutomationBase _automation;
        private static string _screenshotDirectory;
        private static TestContext _testContext;


        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _testContext = testContext;
            _automation = new UIA3Automation();

            // Create a directory for storing screenshots
            _screenshotDirectory = Path.Combine(testContext.TestRunDirectory, "screenshots");
            Directory.CreateDirectory(_screenshotDirectory);
        }


        /// <summary>
        /// 1 - Current test checks that downloaded installer name is corresponding to application name that will be installed
        /// </summary>
        [TestMethod]
        public void When_InstallationFileIsDownloaded_FileNameShouldShouldMatchApplicationName()
        {
            Assert.IsTrue(DownloadCCleanerUsingChromeBrowser());
        }

        /// <summary>
        /// 2 - Current test checks that installer app title is corresponding to application name that will be installed
        /// </summary>
        [TestMethod]
        public void When_InstallerIsOpened_WindowTitleShouldMatchApplicationName()
        {
            string expectedTitle = "Cleaner Installer"; 
            string actualTitle = GetInstallerWindowTitle();
            Assert.AreEqual(expectedTitle, actualTitle,
            $"Expected window title is '{expectedTitle}', but actual window title is '{actualTitle}'");
        }

        /// <summary>
        /// THIS TEST EXPECT TO FAIL
        /// 3 - Current test checks that installer app propose correct installation location
        /// </summary>
        [TestMethod]
        public void When_InstallerIsOpened_ProposedLocationShouldBeClear()
        {
         string expectedLocation = "C:\\Program Files\\Cleaner";

         Assert.IsTrue(TryToFindProposedInstallationLocationOnMainPage(), "There is no installation location textbox on the main page.");
        }

        /// <summary>
        /// 4 - This test checks that installer allows to customize installation options
        /// </summary>
        [TestMethod]
        public void When_ApplicationIsIntalled_FolderLocationShouldMatchApplicationName()
        {
            string expectedLocation = "C:\\Program Files\\Cleaner";
            string proposedInstallationLocation = GetProposedInstallationLocationFromMoreOptionsMenu();
            Assert.AreEqual(expectedLocation, proposedInstallationLocation,
                $"Expected installation location is '{expectedLocation}', but actual installation location is '{proposedInstallationLocation}'");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _automation.Dispose();
        }

        [TestCleanup]
        public static void TestCleanup()
        {
            if (_testContext.CurrentTestOutcome == UnitTestOutcome.Failed)
            {
                var screenshotPath = Path.Combine(_screenshotDirectory, $"{DateTime.Now:yyyyMMdd_HHmmss}_screenshot.png");
                TakeScreenshot(screenshotPath);
                throw new AssertFailedException($"Test failed. See screenshot at {screenshotPath}");
            }
        }

        private static void TakeScreenshot(string filePath)
        {
            try
            {
                // Use FlaUI to take a screenshot of the entire desktop
                var image = _automation.GetDesktop().Capture();

                // Save the screenshot to the specified file path
                image.Save(filePath);
            }
            catch (Exception ex)
            {
                // If there is an error taking the screenshot, log the error and continue
                Console.WriteLine($"Error taking screenshot: {ex.Message}");
            }
        }

    }
}