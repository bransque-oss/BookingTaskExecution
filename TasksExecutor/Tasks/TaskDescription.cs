using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TasksExecutor.Tasks
{
    // Класс для получения текста из описания задачи в соответствии с шаблонами
    public class TaskDescription
    {
        readonly string _bookingIdPattern = @"((?<=Бронь № )[\d\s,]+(?=(\r)))";
        readonly string _comissionValuePattern = @"(?<=Поменять на: )\d{1,2}[.,\s]?\d*(?=[%\s\r а-я])";
        readonly string _regionPattern = @"(?<=Регион: )\D+(?=(\r))";
        readonly string _statusPattern = @"(?<=Поменять на: )[^\r\n]+(?=(\r))";

        public TaskDescription() { }

        //  Метод возвращает список номеров броней на случай, если в задаче указано несколько номеров броней
        public IEnumerable<int> GetBookingIds(string description)
        {        
            // Строка с номерами броней из описания задачи после применения шаблона
            string bookingIdsString = ApplyTextPattern(description, _bookingIdPattern);

            return GetBookingIdsFromString(bookingIdsString);

            // Получает список номеров броней из строки
            static IEnumerable<int> GetBookingIdsFromString(string bookingIdsString)
            {
                //  Переписать это когда-нибудь с помощью регулярных выражений и группировкой в них 

                List<int> bookingIds = new();

                // Эта часть берет из строки с номерами броней только цифры, на случай если туда вписали что-то еще кроме номера.
                // После того, как отобрано 6 цифр, string преобразуется в int.
                // Если успешно, то добавляется в список, если нет, то выдается исключение.
                string resultStr = "";
                int index = 0;
                foreach (char c in bookingIdsString)
                {
                    if (Char.IsDigit(c))
                    {
                        resultStr = String.Concat(resultStr, c);
                        index++;
                    }
                    if (index == 6)
                    {
                        int resultInt;
                        bool resultBool = Int32.TryParse(resultStr, out resultInt);
                        if (!resultBool)
                        {
                            throw new Exception($"Какой-то из номеров броней не удалось распознать.Номера броней в задаче написаны в непредусмотренном формате");
                        }
                        bookingIds.Add(resultInt);
                        index = 0;
                        resultStr = "";
                    }
                }

                // Если вообще не удалось никакой номер брони распознать
                if (bookingIds.Count == 0)
                {
                    throw new Exception("Номера броней в задаче написаны в непредусмотренном формате");
                }

                return bookingIds;
            }
        } 
        //  Комиссионные из описания задачи
        public decimal GetComission(string description)
        {
            string comissionStr = ApplyTextPattern(description, _comissionValuePattern);

            if (String.IsNullOrEmpty(comissionStr))
            {
                throw new Exception($"Комиссионные в задаче написаны в непредусмотренном формате");
            }

            // Если значение комиссионных написано с точкой, знаком процента или пробелом
            StringBuilder comissionSB = new(comissionStr);
            comissionSB = comissionSB.Replace(".", ",").Replace("%", "").Replace(" ", "");

            decimal comissionDec;
            bool resultToDec = Decimal.TryParse(comissionSB.ToString(), out comissionDec);           
            // На случай, если комиссионные указали не в процентах, а в рублях
            if (!resultToDec || comissionDec > 10.00m)
            {
                throw new Exception($"Не удалось преобразовать комиссионные в проценты или они больше 10%");
            }

            return comissionDec;
        }
        public Regions GetRegion(string description)
        {
            string regionStr = ApplyTextPattern(description, _regionPattern);
            return regionStr.StringToRegions();
        }
        public Statuses GetStatus(string description)
        {
            string statusStr = ApplyTextPattern(description, _statusPattern);
            return statusStr.StringToStatuses();
        }
        
        private string ApplyTextPattern(string text, string pattern)
            => Regex.Match(text, pattern, RegexOptions.IgnoreCase).Value;
    }
}
