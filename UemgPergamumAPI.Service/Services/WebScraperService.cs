using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UemgPergamumAPI.Domain.Entities;
using UemgPergamumAPI.Domain.Enum;
using UemgPergamumAPI.Domain.Services;

namespace UemgPergamumAPI.Service.Services
{
    public class WebScraperService : IWebScraperService
    {
        public IEnumerable<LibraryItem> WebScrapingUemgPergamum(string searchTerm, SearchType searchType, string unitySearch = "")
        {
            var url = "http://200.198.18.141/pergamum/biblioteca/index.php";

            string[] units = {
            "Reitoria",
            "Musica",
            "Guignard",
            "Tancredo Neves",
            "Educação",
            "Design",
            "João Molevade",
            "Frutal",
            "Barbacena",
            "Ubá",
            "Leopoldina",
            "Campanha",
            "Ibirité",
            "Divinópolis",
            "Diamantina",
            "Ituiutaba",
            "Passos - CIRE",
            "Poços de Caldas",
            "Cláudio",
            "Abaeté",
            "Carangola",
            "Passos - Bloco 2",
            };

            string driverPath = "/opt/selenium/";
            String driverExecutableFileName = "chromedriver";
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--no-sandbox");
            options.AddArguments("--window-size=1920,1080");
            options.AddArguments("--disable-extensions");
            options.AddArguments("--proxy-server='direct://'");
            options.AddArguments("--proxy-bypass-list=*");
            options.AddArguments("start-maximized");
            options.AddArguments("--headless");
            options.AddArguments("--disable-gpu");
            options.AddArguments("--disable-dev-shm-usage");
            options.AddArguments("--ignore-certificate-errors");

            options.BinaryLocation = "/opt/google/chrome/chrome";
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverPath, driverExecutableFileName);
            var driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(180));
            //var driver = new ChromeDriver(service, options);
            //var driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            driver.Navigate().GoToUrl(url);
            var changeLanguage = driver.FindElementByXPath("/html/body/form/table/tbody/tr[2]/td/div[3]/table/tbody/tr[1]/td/div/a[5]");
            changeLanguage.Click();
            Thread.Sleep(180);


            var scriptType = "";
            switch (searchType)
            {
                case SearchType.books:
                    scriptType = "document.getElementById('tipo_obra_selecionados').value='1,'";
                    driver.ExecuteScript(scriptType);
                    break;
                case SearchType.article:
                    scriptType = "document.getElementById('tipo_obra_selecionados').value='4,'";
                    driver.ExecuteScript(scriptType);
                    break;
                case SearchType.monographs:
                    scriptType = "document.getElementById('tipo_obra_selecionados').value='5,'";
                    driver.ExecuteScript(scriptType);
                    break;
                default:
                    break;
            }

            if (unitySearch != null)
            {
                var unityListSelected = units.ToList().Where(t => t.ToLower().Contains(unitySearch.ToLower())).ToList();
                var unitySelected = "";

                foreach (var item in unityListSelected)
                    unitySelected = String.Concat(unitySelected, $"{units.ToList().IndexOf(item) + 1},");

                var scriptUnity = $"document.getElementById('listabib').value='{unitySelected}'";
                driver.ExecuteScript(scriptUnity);
            }

            var searhBox = driver.FindElement(By.Id("termo_para_pesquisa"));

            searhBox.Click();

            searhBox.SendKeys(searchTerm);

            var buttonOk = driver.FindElement(By.Id("pesq"));

            buttonOk.Click();

            while (driver.FindElements(By.Id("id_resultados_temp")).Count == 0)
            {
                Thread.Sleep(180);
            }

            var itemNumber = 1;
            var resultList = new List<LibraryItem>();
            while (driver.FindElements(By.XPath($"//*[@id=\"id_resultados_temp\"]/div[{itemNumber}]")).Count > 0)
            {
                var libraryItem = new LibraryItem();

                var maxResultNumber = driver.FindElements(By.XPath($"//*[@id=\"id_resultados_temp\"]/div/b/center"));
                if (maxResultNumber.Count > 0) return resultList;

                var tituloLink = driver.FindElement(By.XPath($"//*[@id=\"id_resultados_temp\"]/div[{itemNumber}]/table/tbody/tr/td[3]/table/tbody/tr[1]/td/a"));
                tituloLink.Click(); 

                while (driver.FindElements(By.XPath("//*[@id=\"div_detalhes_acervo\"]/div")).Count == 0)
                {
                    Thread.Sleep(180);
                }

                var divDetails = driver.FindElement(By.XPath("//*[@id=\"div_detalhes_acervo\"]/div/div[2]"));
                var table = divDetails.Text;

                var split = table.Replace("\r", "").Split('\n').ToList();

                var TituloIndex = split.IndexOf(split.Where(t => t.Contains("Título")).FirstOrDefault());
                libraryItem.Title = TituloIndex == -1 ? "" : split[TituloIndex + 1];

                var autorIndex = split.IndexOf(split.Where(t => t.Contains("Autor")).FirstOrDefault());
                libraryItem.Author = autorIndex == -1 ? "" : split[autorIndex + 1];

                var publicacaoIndex = split.IndexOf("Publicação");
                libraryItem.Publisher = publicacaoIndex == -1 ? "" : split[publicacaoIndex + 1];

                var closeDetailsFullXpath = driver.FindElement(By.Id("fechar_2"));
                Actions actionsClose = new Actions(driver);
                actionsClose.MoveToElement(closeDetailsFullXpath).Click().Perform();

                var copesLink = driver.FindElement(By.XPath($"//*[@id=\"id_resultados_temp\"]/div[{itemNumber}]/table/tbody/tr/td[3]/table/tbody/tr[6]/td/div[6]/a[1]"));
                copesLink.Click();
                ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile($"ImageCopes.png", ScreenshotImageFormat.Png);
                while (driver.FindElements(By.XPath("//*[@id=\"div_detalhes_exemplar\"]/div")).Count == 0)
                {
                    Thread.Sleep(180);
                }


                var spanList = driver.FindElements(By.XPath($"//*[@id=\"div_detalhes_exemplar\"]/div/span"));
                foreach (var span in spanList)
                {
                    var unity = new LibraryUnity();
                    var unityList = new List<LibraryUnity>();
                    var spanId = span.GetAttribute("id");

                    IWebElement localTable = driver.FindElement(By.XPath($"//*[@id=\"{spanId}\"]/table[1]"));
                    var local = localTable.FindElements(By.ClassName("txt_destaque"));
                    if (local.Count > 0) unity.Unity = local.FirstOrDefault().Text;

                    IWebElement tableCopies = driver.FindElement(By.XPath($"//*[@id=\"{spanId}\"]/table[2]"));
                    unity.Collection = tableCopies.Text;

                    if (unity != null) unityList.Add(unity);

                    libraryItem.Unities = unityList;
                }

                Actions actionsCloseSecond = new Actions(driver);
                actionsCloseSecond.MoveToElement(closeDetailsFullXpath).Click().Perform();

                itemNumber++;
                resultList.Add(libraryItem);
            }

            driver.Quit();
            return resultList;
        }
    }
}
