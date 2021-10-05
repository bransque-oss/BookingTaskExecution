using System;
using BitrixWebScrapper;
using TasksExecutor.Tasks;
using System.Collections.Generic;

namespace TasksExecutor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Start();
            }
            catch(Exception ex)
            {
                LoggingExceptions.WriteLogToFile(ex);
            }
        }
        static void Start()
        {
            using (Browser driver = new Browser())
            {
                driver.Authorize();

                driver.ShowComissionTasks();
                IEnumerable<string> linksCommissionTask = driver.GetTasksLinks();

                driver.ShowDropComissionTasks();
                IEnumerable<string> linksDropComissionTask = driver.GetTasksLinks();

                driver.ShowStatusTasks();
                IEnumerable<string> linksStatusTask = driver.GetTasksLinks();

                TasksFactory factory = new TasksFactory();

                factory.OperateComissionTask(linksCommissionTask, driver);

                factory.OperateDropComissionTask(linksDropComissionTask, driver);

                factory.OperateStatusTask(linksStatusTask, driver);
            }
        }
    }
}
