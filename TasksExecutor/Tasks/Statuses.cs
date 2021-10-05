using System;

namespace TasksExecutor.Tasks
{
    public enum Statuses : byte
    {
        New,                                     //  Новая
        InProgress,                              //  В работе
        Processed,                               //  Обработана
        TreatySigning,                           //  Запись на договор
        TreatySigned,                            //  Договор подписан
        Purchased,                               //  Объект выкуплен
        CanceledDueToRepeat,                     //  Отменена - Дубль
        CanceledUnableToBook,                    //  Отменена - Невозможно забронировать
        CanceledOutOfStockExchangeMarket,        //  Отменена - Нет в наличии (Биржа переуступок)
        CanceledOutOfStockOurFault,              //  Отменена - Нет в наличии у застройщика 1 (Наша вина)
        CanceledOutOfStockDeveloperFault,        //  Отменена - Нет в наличии у застройщика 2 (Вина застройщика)
        CanceledCollisionWithDeveloperSalesDep,  //  Отменена - Пересечение с отделом продаж застройщика
        CanceledSubagentRequest,                 //  Отменена - По просьбе субагента
        CanceledFinancialCauses,                 //  Отменена - По финансовым причинам
        CanceledAlternativesOffered,             //  Отменена - Предложены альтернативы
        CanceledViewing,                         //  Отменена - Просмотр
        CanceledTest,                            //  Отменена - Тест
        CanceledWaitingInformation,              //  Отменена - Уточнение информации
        CanceledFixation                         //  Отменена - Фиксация
    }

    public static class StatusesExtension
    {
        public static Statuses StringToStatuses(this String status) =>
            status.ToLower() switch
            {
                "новая" => Statuses.New,
                "в работе" => Statuses.InProgress,
                "обработана" => Statuses.Processed,
                "запись на договор" => Statuses.TreatySigning,
                "договор подписан" => Statuses.TreatySigned,
                "объект выкуплен" => Statuses.Purchased,
                "отменена - дубль" => Statuses.CanceledDueToRepeat,
                "отменена - невозможно забронировать" => Statuses.CanceledUnableToBook,
                "отменена - нет в наличии (биржа переуступок)" => Statuses.CanceledOutOfStockExchangeMarket,
                "отменена - нет в наличии у застройщика 1 (наша вина)" => Statuses.CanceledOutOfStockOurFault,
                "отменена - нет в наличии у застройщика 2 (вина застройщика)" => Statuses.CanceledOutOfStockDeveloperFault,
                "отменена - пересечение с отделом продаж застройщика" => Statuses.CanceledCollisionWithDeveloperSalesDep,
                "отменена - по просьбе субагента" => Statuses.CanceledSubagentRequest,
                "отменена - по финансовым причинам" => Statuses.CanceledFinancialCauses,
                "отменена - предложены альтернативы" => Statuses.CanceledAlternativesOffered,
                "отменена - просмотр" => Statuses.CanceledViewing,
                "отменена - тест" => Statuses.CanceledTest,
                "отменена - уточнение информации" => Statuses.CanceledWaitingInformation,
                "отменена - фиксация" => Statuses.CanceledFixation,
                _ => throw new Exception("Статус брони, указанный в задаче, не существует")
            };
    }
}
