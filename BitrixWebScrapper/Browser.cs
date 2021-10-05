using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace BitrixWebScrapper
{
    public class Browser : IDisposable
    {
        public Browser()
        {
            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("-headless");
            Driver = new FirefoxDriver(options);
        }
        
        FirefoxDriver Driver { get; }

        public void Authorize()
        {
            Console.WriteLine("Авторизация в битриксе...");

            Driver.Navigate().GoToUrl(@"https://industry.bitrix24.ru");

            // Поле ввода логина
            GetVisibleElementById("login").SendKeys("bitrix-login");
            ClickSubmitButton();

            // Поле ввода пароля
            GetVisibleElementById("password").SendKeys("bitrix-password");
            ClickSubmitButton();
            
            void ClickSubmitButton()
            {
                // Кнопка "Далее"
                string submitButtonCss = ".ui-btn.ui-btn-md.ui-btn-success.ui-btn-round.b24-network-auth-form-btn";

                IWebElement submitButton = GetVisibleElementByCss(submitButtonCss);
                submitButton.Click();
            }
        }
        public void CloseTask()
        {
            GetClickableElementByCss(".task-view-button.complete.pause.ui-btn.ui-btn-success").Click();
        }
        public void Dispose()
        {
            Driver?.Quit();
        }
        public IEnumerable<string> GetTasksLinks()
        {
            Console.WriteLine("Получение ссылок на задачи...");

            List<string> taskLinks = new();

            // Бывает, что битрикс показывает список из закрытых задач.
            // Если задач нет, то после поиска в битриксе будет показан блок "Ничего не найдено".
            // Если задачи есть, то поиск такого блока выдает исключение. 
            // Поэтому пока так сделал.
            try
            {
                if (Driver.FindElementsByCssSelector(".task-title.task-status-text-color-in-progress").Count == 0 ||
                    Driver.FindElementByCssSelector(".main-grid-empty-block") is not null)
                {
                    return taskLinks;
                }
            }
            catch (NoSuchElementException) { }


            // Задачи, у которых заголовок выглядит, как у задач в статусе "Ждем выполнения"
            IEnumerable<IWebElement> linkElements = GetVisibleElementsByCss(".task-title.task-status-text-color-in-progress");

            foreach (var e in linkElements)
            {
                taskLinks.Add(e.GetAttribute("href"));
            }

            return taskLinks;
        }
        public string GetTaskDescriptionText()
        {
            string taskDescriptionId = "task-detail-description";

            WebDriverWait wait = Wait();
            IWebElement taskDescription = wait.Until(ExpectedConditions.ElementExists(By.Id(taskDescriptionId)));
            return taskDescription.Text;
        }
        public void OpenPage(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }
        public void SetTaskTags(string tagRegion, string tagDivision, string tagAction)
        {
            //  Кнопка в секции тегов "Добавить"
           GetClickableElementById("task-tags-link").Click();

            //  Клик по текстовому полю и добавление тегов
            IWebElement fieldOnPopup = Driver.FindElement(By.ClassName("popup-tags-textbox"));
            IWebElement fieldInsideFieldOnPopup = fieldOnPopup.FindElement(By.CssSelector("input[type='text']"));
            fieldInsideFieldOnPopup.Click();
            fieldInsideFieldOnPopup.SendKeys($"{tagRegion}, {tagDivision}, {tagAction}");

            //  Кнопка "Выбрать"
            GetClickableElementByCss(".popup-window-button.popup-window-button-create").Click();
        }
        public void SetTaskTime(string minute)
        {
            //  Переключение на вкладку "Время"
            //Driver.FindElement(By.Id("task-time-switcher")).Click();
            GetClickableElementById("task-time-switcher").Click();

            //  Кнопка "Добавить"
            IWebElement timeBlock = GetVisibleElementById("task-time-table");
            IWebElement timeAdd = timeBlock.FindElement(By.ClassName("task-dashed-link-inner"));
            timeAdd.Click();

            //  Убирает "1" в часах, вписываемую Битриксом по умолчанию
            IWebElement spentHours = timeBlock.FindElement(By.ClassName("task-time-spent-hours"));
            IWebElement hoursTextBlock = spentHours.FindElement(By.ClassName("task-time-field-textbox"));
            hoursTextBlock.Clear();

            //  Вписывает время в минуты
            IWebElement spentMinutes = timeBlock.FindElement(By.ClassName("task-time-spent-minutes"));
            IWebElement minutesTextBlock = spentMinutes.FindElement(By.ClassName("task-time-field-textbox"));
            minutesTextBlock.SendKeys(minute);

            //  Галочка "Подтвердить"
            GetClickableElementByCss(".task-table-edit-ok").Click();
        }
        public void ShowComissionTasks()
        {
            Console.WriteLine("Поиск задач по смене комиссионных...");

            ShowTasksByTitle("Сменить комиссионные в брони");
        }
        public void ShowStatusTasks()
        {
            Console.WriteLine("Поиск задач по cмене статуса...");

            ShowTasksByTitle("Сменить статус в брони");
        }
        public void ShowDropComissionTasks()
        {
            Console.WriteLine("Поиск задач по смене процесса работы с комиссионными...");

            ShowTasksByTitle("Сменить процесс работы с комиссионными");
        }

        private IWebElement GetClickableElementByCss(string css)
        {
            WebDriverWait wait = Wait();
            IWebElement element = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(css)));
            return element;
        }
        private IWebElement GetClickableElementById(string id)
        {
            WebDriverWait wait = Wait();
            IWebElement element = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(id)));
            return element;
        }
        private IWebElement GetClickableElementByXPath(string xPath)
        {
            WebDriverWait wait = Wait();
            IWebElement element = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xPath)));
            return element;
        }
        private void ShowTasksByTitle(string wordsToSearch)
        {
            // Страница с задачами
            Driver.Navigate().GoToUrl(@"https://industry.bitrix24.ru/company/personal/user/2083/tasks/");

            // Задачи в виде списка
            GetClickableElementById("tasks_view_mode_list").Click();

            // Строка поиска задач
            string searchField = "TASKS_GRID_ROLE_ID_4096_0_ADVANCED_N_search";
            //GetClickableElementById(searchField).Clear();
            GetClickableElementById(searchField).Click();

            // Строка "Название в поиске"
            string searchTitle = "//input[@name=\"TITLE\"]";
            GetClickableElementByXPath(searchTitle).Clear();
            GetClickableElementByXPath(searchTitle).Click();
            GetClickableElementByXPath(searchTitle).SendKeys(wordsToSearch);

            // Клик по кнопке "Найти"
            GetClickableElementByCss(".ui-btn.ui-btn-primary.ui-btn-icon-search.main-ui-filter-field-button.main-ui-filter-find").Click(); 

        }
        private IWebElement GetVisibleElementById(string id)
        {
            WebDriverWait wait = Wait();
            IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)));
            return element;
        }
        private IWebElement GetVisibleElementByCss(string css)
        {
            WebDriverWait wait = Wait();
            IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(css)));
            return element;
        }
        private IEnumerable<IWebElement> GetVisibleElementsByCss(string css)
        {
            WebDriverWait wait = Wait();
            ICollection<IWebElement> elements = wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(css)));
            return elements;
        }
        private WebDriverWait Wait()
            => new WebDriverWait(Driver, TimeSpan.FromSeconds(30));   
    }
}
