using Dapper.Contrib.Extensions;
using DbUp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using static System.Console;

namespace Depo
{
    class Program
    {
        private const string CONNECTION_STRING = "Server=A-305-05;Database=DepoDb;Trusted_Connection=true;";
        static void Main(string[] args)
        {
            EnsureDatabase.For.SqlDatabase(CONNECTION_STRING);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(CONNECTION_STRING)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();
            var result = upgrader.PerformUpgrade();

            try
            {
                //InsertInitialData();
                ShowMenu();
            }
            catch(Exception exception)
            {
                WriteLine(exception.Message);
            }
            ReadKey();
        }

        private static void InsertInitialData()
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                var statuses = new List<Status>() {
                    new Status
                    {
                        Text = "Сломан"
                    },
                    new Status
                    {
                        Text = "В ремонте"
                    }
                };
                var engineers = new List<Engineer>()
                {
                    new Engineer
                    {
                        FullName = "Иванов Сергей"
                    },
                    new Engineer
                    {
                        FullName = "Уханов Алексей"
                    },
                    new Engineer
                    {
                        FullName = "Шнуров Евгений"
                    },
                    new Engineer
                    {
                        FullName = "Серый Николай"
                    }
                };
                var buses = new List<Bus>()
                {
                    new Bus
                    {
                        VinNumber = "Z135MIN"
                    },
                    new Bus
                    {
                        VinNumber = "X777AAA"
                    },
                    new Bus
                    {
                        VinNumber = "L789AGA"
                    }
                };

                var statusAffectedRows = connection.Insert(statuses);
                var engineersAffectedRows = connection.Insert(engineers);
                var busesAffectedRows = connection.Insert(buses);

                if (statusAffectedRows != statuses.Count)
                {
                    throw new Exception("Ошибка вставки в таблицу Status!");
                }
                else if (engineersAffectedRows != engineers.Count)
                {
                    throw new Exception("Ошибка вставки в таблицу Engineer!");
                }
                else if (busesAffectedRows != buses.Count)
                {
                    throw new Exception("Ошибка вставки в таблицу Bus!");
                }
            }
        }

        private static void ShowMenu()
        {
            WriteLine("0 - Добавить автобус для ремонта");
            WriteLine("1 - Показать все автобусы");
            WriteLine("2 - Выбрать автобус для ремонта");
            WriteLine("3 - Завершить ремонт автобуса");
            WriteLine("Выбрать >>");
            var userSelect = 0;
            int.TryParse(ReadLine(), out userSelect);
            switch (userSelect)
            {
                case 0:
                    {
                        AddBrokenBus();
                        break;
                    }
                case 1:
                    {
                        Clear();
                        ShowAllBus();
                        break;
                    }
                case 2:
                    {
                        Clear();
                        ChooseBusForRepair();
                        break;
                    }
                case 3:
                    {
                        Clear();
                        EndRepair();
                        break;
                    }
                default:
                    {
                        WriteLine("Такой команды не существует.");
                        break;
                    }
            }
        }

        private static void AddBrokenBus()
        {
            using(var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                WriteLine("Введите номер автобуса");
                var inputBusNumber = ReadLine();
                var bus = new Bus
                {
                    VinNumber = inputBusNumber,
                    StatusId = Convert.ToInt32(StatusesEnum.Broken)
                };
                var busAffectedRows = connection.Insert(bus);
                if (busAffectedRows != 1)
                {
                    throw new Exception("Ошибка вставки в таблицу Bus!");
                }
            }
        }

        private static void EndRepair()
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                var buses = connection.GetAll<Bus>();

                WriteLine("Введите номер автобуса:");
                var inputBusNumber = ReadLine();

                foreach (var bus in buses)
                {
                    if (bus.VinNumber.Equals(inputBusNumber))
                    {
                        var isDeleted = connection.Delete(bus);
                        if(!isDeleted)
                        {
                            throw new Exception("Запись не удалена в таблице Bus");
                        }
                        else
                        {
                            WriteLine("Запись успешно удалена!");
                        }
                    }
                }
            }
        }

        private static void ChooseBusForRepair()
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                var buses = connection.GetAll<Bus>();
                var engineers = connection.GetAll<Engineer>();

                WriteLine("Введите номер автобуса:");
                var inputBusNumber = ReadLine();
                WriteLine("Введите имя механика:");
                var inputEngineerName = ReadLine();

                foreach(var engineer in engineers)
                {
                    foreach (var bus in buses)
                    {
                        if (bus.StatusId == Convert.ToInt32(StatusesEnum.Broken))
                        {
                            if (bus.VinNumber.Equals(inputBusNumber) && engineer.Equals(inputEngineerName))
                            {
                                bus.StatusId = Convert.ToInt32(StatusesEnum.Repairing);
                                var busRowsAffected = connection.Update(bus);
                                if (!busRowsAffected)
                                {
                                    throw new Exception("Запись не обновлена в таблице Bus");
                                }
                            }
                            else
                            {
                                throw new Exception("Такого номера автобуса или механика нет в базе данных!");
                            }
                        }
                        else
                        {
                            throw new Exception("Этот автобус в ремонте или отремонтирован");
                        }

                    }
                }
                
            }
        }

        private static void ShowAllBus()
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                var buses = connection.GetAll<Bus>().ToList();

                foreach (var bus in buses)
                {
                    WriteLine($"{bus.Id}) {bus.VinNumber}");
                }
            }
        }
    }
}
