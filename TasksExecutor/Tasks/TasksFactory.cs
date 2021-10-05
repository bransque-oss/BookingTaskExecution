using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using BitrixWebScrapper;
using Db;

namespace TasksExecutor.Tasks
{
    class TasksFactory
    {
        private readonly TaskDescription description;

        public TasksFactory() 
        {
            description = new TaskDescription();
        }

        public void OperateComissionTask(IEnumerable<string> links, Browser driver)
        {
            Console.WriteLine("Обработка задач по смене комиссионных...");

            foreach (var link in links)
            {
                TaskComission task;

                try
                {
                    task = GetComissionTask(link, driver);

                    foreach (int bookingId in task.BookingIds)
                    {
                        Booking.ChangeComission(bookingId, task.ComissionValue);
                    }
                }
                catch (Exception ex)
                {
                    LoggingExceptions.WriteLogToFile(ex, link);
                    continue;
                }                          

                driver.SetTaskTags(task.TagRegion, task.TagDivision, task.TagAction);

                driver.SetTaskTime(task.TaskTime);

                driver.CloseTask();
            }
        }
        public void OperateDropComissionTask(IEnumerable<string> links, Browser driver)
        {
            Console.WriteLine("Обработка задач по смене процесса работы с комиссионными (удаление)...");

            foreach (var link in links)
            {
                TaskDropComission task;

                try
                {
                    task = GetDropComissionTask(link, driver);
                }
                catch (Exception ex)
                {
                    LoggingExceptions.WriteLogToFile(ex, link);
                    continue;
                }

                foreach (int bookingId in task.BookingIds)
                {
                    Booking.DropComission(bookingId);
                }

                driver.SetTaskTags(task.TagRegion, task.TagDivision, task.TagAction);

                driver.SetTaskTime(task.TaskTime);

                driver.CloseTask();
            }
        }
        public void OperateStatusTask(IEnumerable<string> links, Browser driver)
        {
            Console.WriteLine("Обработка задач по смене статуса...");

            foreach (var link in links)
            {
                TaskStatus task;

                try
                {
                    task = GetStatusTask(link, driver);

                    foreach (int bookingId in task.BookingIds)
                    {
                        Booking.ChangeStatus(bookingId, (byte)task.Status);
                    }
                }
                catch (Exception ex)
                {
                    LoggingExceptions.WriteLogToFile(ex, link);
                    continue;
                }

                driver.SetTaskTags(task.TagRegion, task.TagDivision, task.TagAction);

                driver.SetTaskTime(task.TaskTime);

                driver.CloseTask();
            }
        }
        TaskComission GetComissionTask(string link, Browser driver)
        {
            driver.OpenPage(link);

            string descriptionStr = driver.GetTaskDescriptionText();
            IEnumerable<int> bookingIds = description.GetBookingIds(descriptionStr);
            decimal comission = description.GetComission(descriptionStr);
            Regions region = description.GetRegion(descriptionStr);

            return new TaskComission(link, bookingIds, comission, region);
        }
        TaskDropComission GetDropComissionTask(string link, Browser driver)
        {
            driver.OpenPage(link);

            string descriptionStr = driver.GetTaskDescriptionText();
            IEnumerable<int> bookingIds = description.GetBookingIds(descriptionStr);
            Regions region = description.GetRegion(descriptionStr);

            return new TaskDropComission(link, bookingIds, region);
        }
        TaskStatus GetStatusTask(string link, Browser driver)
        {
            driver.OpenPage(link);

            string descriptionStr = driver.GetTaskDescriptionText();

            IEnumerable<int> bookingIds = description.GetBookingIds(descriptionStr);
            Statuses status = description.GetStatus(descriptionStr);
            Regions region = description.GetRegion(descriptionStr);

            return new TaskStatus(link, bookingIds, status, region);
        }
    }
}
