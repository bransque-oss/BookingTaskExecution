using System;
using System.Data;
using System.Data.SqlClient;

namespace Db
{
    public class Booking
    {
        public static void ChangeComission(int bookingId, decimal comissionValue)
        {
            //  ComissionType 1 - в руб., 2 - в процентах, 3 - в руб/метр
            string query = @"
                EXEC dbo.SetPlayerContext @PlayerId = 86532

                UPDATE dbo.Bookings
                SET ComissionType = 2,
                    BaseCommissionValue = @comissionValue
                WHERE BookingId = @bookingId

                WAITFOR DELAY '00:00:10'  /*ждем пока все триггеры выполнятся, меньше лучше не ставить, больше - можно*/

                UPDATE brokerage.BrokerComission
                SET PlanCommissionValue = (SELECT ComissionValue FROM dbo.Bookings WHERE BookingId = @bookingId),
                    PlanCommissionType = 2
                WHERE ComissionId = @bookingId

                DECLARE @bronBigId BigIntId 
                INSERT @bronBigId (Id) VALUES (@bookingId)
                EXEC fin.RefreshViewBookingAccountingDenorm @BookingIds = @bronBigId
            ";

            using (SqlConnection connection = GetConnection())
            {
                connection.Open();

                using (SqlCommand command = new(query, connection))
                {
                    command.CommandType = CommandType.Text;

                    SqlParameter id = new()
                    {
                        ParameterName = "@bookingId",
                        SqlDbType = SqlDbType.Int,
                        SqlValue = bookingId
                    };
                    SqlParameter comission = new()
                    {
                        ParameterName = "@comissionValue",
                        SqlDbType = SqlDbType.Decimal,
                        SqlValue = comissionValue
                    };
                    command.Parameters.Add(id);
                    command.Parameters.Add(comission);

                    int result = command.ExecuteNonQuery();
                    if (result == 0)
                    {
                        throw new RowNotInTableException($"В базе нет брони {bookingId}");
                    }
                }
            }
        }
        public static void ChangeStatus(int bookingId, byte statusesId)
        {
            string queryChangeStatus = @"
                EXEC dbo.SetPlayerContext @PlayerId = 86532
                UPDATE dbo.Bookings 
                SET StatusId = @statusId, 
                    ResolutionId = @resolutionId
                WHERE BookingId = @bookingId
            ";

            //  Удалять значения в "Работе с комиссионными нужно, потому что при возврате на статус "Запись на договор"
            //  и более ранний, без удаления лагают значения в НГ
            string queryDeleteComission = @"
                EXEC dbo.SetPlayerContext @PlayerId = 86532 
                DELETE brokerage.BrokerComission  
                WHERE ComissionId = @bookingId
            ";

            short statusId = 0;
            short resolutionId = 0;

            //  Статусы смотреть в TasksExecutor.Tasks.Statuses
            switch (statusesId)
            {
                case 0:     //  Новая
                    {
                        statusId = 1;
                        resolutionId = 1;
                        break;
                    }
                case 1:     //  В работе
                    {
                        statusId = 3;
                        resolutionId = 18;
                        break;
                    }
                case 2:     //  Обработана
                    {
                        statusId = 4;
                        resolutionId = 3;
                        break;
                    }
                case 3:     //  Запись на договор
                    {
                        statusId = 6;
                        resolutionId = 19;
                        break;
                    }
                case 4:     //  Договор подписан
                    {
                        statusId = 7;
                        resolutionId = 5;
                        break;
                    }
                case 5:     //  Объект выкуплен
                    {
                        statusId = 5;
                        resolutionId = 6;
                        break;
                    }
                case 6:     //  Отменена - Дубль
                    {
                        statusId = 2;
                        resolutionId = 11;
                        break;
                    }
                case 7:     //  Отменена - Невозможно забронировать
                    {
                        statusId = 2;
                        resolutionId = 20;
                        break;
                    }
                case 8:     //  Отменена - Нет в наличии(Биржа переуступок)
                    {
                        statusId = 2;
                        resolutionId = 16;
                        break;
                    }
                case 9:     //  Отменена - Нет в наличии у застройщика 1(Наша вина)
                    {
                        statusId = 2;
                        resolutionId = 8;
                        break;
                    }
                case 10:     //  Отменена - Нет в наличии у застройщика 2(Вина застройщика)
                    {
                        statusId = 2;
                        resolutionId = 9;
                        break;
                    }
                case 11:    //  Отменена - Пересечение с отделом продаж застройщика
                    {
                        statusId = 2;
                        resolutionId = 10;
                        break;
                    }
                case 12:    //  Отменена - По просьбе субагента
                    {
                        statusId = 2;
                        resolutionId = 7;
                        break;
                    }
                case 13:    //  Отменена - По финансовым причинам
                    {
                        statusId = 2;
                        resolutionId = 22;
                        break;
                    }
                case 14:    //  Отменена - Предложены альтернативы
                    {
                        statusId = 2;
                        resolutionId = 21;
                        break;
                    }
                case 15:    //  Отменена - Просмотр
                    {
                        statusId = 2;
                        resolutionId = 13;
                        break;
                    }
                case 16:    //  Отменена - Тест
                    {
                        statusId = 2;
                        resolutionId = 12;
                        break;
                    }
                case 17:    //  Отменена - Уточнение информации 
                    {
                        statusId = 2;
                        resolutionId = 14;
                        break;
                    }
                case 18:    //  Отменена - Фиксация
                    {
                        statusId = 2;
                        resolutionId = 24;
                        break;
                    }

            }

            using (SqlConnection connection = GetConnection())
            {
                connection.Open();

                //  Изменение статуса брони
                using (SqlCommand commandChangeStatus = new(queryChangeStatus, connection))
                {
                    commandChangeStatus.CommandType = CommandType.Text;

                    SqlParameter id = new()
                    {
                        ParameterName = "@bookingId",
                        SqlDbType = SqlDbType.Int,
                        SqlValue = bookingId
                    };
                    SqlParameter status = new()
                    {
                        ParameterName = "@statusId",
                        SqlDbType = SqlDbType.SmallInt,
                        SqlValue = statusId
                    };
                    SqlParameter resolution = new()
                    {
                        ParameterName = "@resolutionId",
                        SqlDbType = SqlDbType.SmallInt,
                        SqlValue = resolutionId
                    };

                    commandChangeStatus.Parameters.Add(id);
                    commandChangeStatus.Parameters.Add(status);
                    commandChangeStatus.Parameters.Add(resolution);

                    int result = commandChangeStatus.ExecuteNonQuery();
                    if (result == 0)
                    {
                        throw new RowNotInTableException($"В базе нет брони {bookingId}");
                    }
                }   

                //  Удаление работы с комиссионными, если ставится статус "Запись на договор" или более ранний
                if (statusesId <= 3)
                {
                    using (SqlCommand commandDeleteComission = new(queryDeleteComission, connection))
                    {
                        commandDeleteComission.CommandType = CommandType.Text;

                        SqlParameter id = new()
                        {
                            ParameterName = "@bookingId",
                            SqlDbType = SqlDbType.Int,
                            SqlValue = bookingId
                        };

                        commandDeleteComission.Parameters.Add(id);

                        commandDeleteComission.ExecuteNonQuery();
                    }
                }
            }
        }
        public static void DropComission(int bookingId)
        {
            string queryDeleteComission = @"
                EXEC dbo.SetPlayerContext @PlayerId = 86532 
                DELETE brokerage.BrokerComission  
                WHERE ComissionId = @bookingId
            ";

            using (SqlConnection connection = GetConnection())
            {
                connection.Open();

                using (SqlCommand commandDeleteComission = new(queryDeleteComission, connection))
                {
                    commandDeleteComission.CommandType = CommandType.Text;

                    SqlParameter id = new()
                    {
                        ParameterName = "@bookingId",
                        SqlDbType = SqlDbType.Int,
                        SqlValue = bookingId
                    };

                    commandDeleteComission.Parameters.Add(id);

                    int result = commandDeleteComission.ExecuteNonQuery();
                }                
            }
        }
        static SqlConnection GetConnection()
            => new(@"data source=server.local;Initial Catalog=Database;Trusted_Connection=True;");
    }
}
