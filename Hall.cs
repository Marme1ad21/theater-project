// TODO:
// 1. Реализован учет информации о театральном зале
// 2. Реализована система мест и их бронирования
// 3. Реализован контроль заполняемости зала

using System;
using System.Collections.Generic;
using System.Linq;

namespace Theater
{
    public class Hall
    {
        public int Number { get; set; }           // Номер зала
        public string Name { get; set; }          // Название (Большой, Малый, Экспериментальный)
        public int Capacity { get; set; }         // Вместимость

        // TODO 1: Добавлены свойства
        public string SectorLayout { get; set; }  // Расположение секторов: "партер:200,бельэтаж:150,балкон:100"
        public string Equipment { get; set; }     // Оборудование: звук, свет, спецэффекты

        // Словарь для хранения информации о секторах
        private Dictionary<string, (int rows, int seatsPerRow, int totalSeats)> sectors =
            new Dictionary<string, (int rows, int seatsPerRow, int totalSeats)>();

        // Карты занятости по датам
        private Dictionary<DateTime, Dictionary<string, bool[,]>> seatMaps =
            new Dictionary<DateTime, Dictionary<string, bool[,]>>();

        public Hall(int number, string name, int capacity, string sectorLayout, string equipment)
        {
            Number = number;
            Name = name;
            Capacity = capacity;

            // TODO 1: Сохранить расположение секторов и оборудование
            SectorLayout = sectorLayout;
            Equipment = equipment;

            // Парсинг информации о секторах
            ParseSectorLayout(sectorLayout);
        }

        // Парсинг строки с расположением секторов
        private void ParseSectorLayout(string layout)
        {
            sectors.Clear();
            string[] sectorParts = layout.Split(',');

            foreach (var part in sectorParts)
            {
                string[] kv = part.Split(':');
                if (kv.Length == 2)
                {
                    string sectorName = kv[0].Trim();
                    if (int.TryParse(kv[1], out int totalSeats))
                    {
                        // Определяем количество рядов и мест в ряду на основе общей вместимости сектора
                        // Упрощенная логика: для партера обычно 10-20 рядов, для балкона меньше
                        int rows;
                        int seatsPerRow;

                        if (sectorName.Contains("партер"))
                        {
                            rows = Math.Min(12, totalSeats / 15);
                        }
                        else if (sectorName.Contains("бельэтаж"))
                        {
                            rows = Math.Min(8, totalSeats / 12);
                        }
                        else if (sectorName.Contains("балкон"))
                        {
                            rows = Math.Min(6, totalSeats / 10);
                        }
                        else if (sectorName.Contains("ложи"))
                        {
                            rows = Math.Min(2, totalSeats / 8);
                        }
                        else
                        {
                            rows = Math.Min(10, totalSeats / 12);
                        }

                        seatsPerRow = (int)Math.Ceiling((double)totalSeats / rows);
                        sectors[sectorName] = (rows, seatsPerRow, totalSeats);
                    }
                }
            }

            // Если сектора не распарсились, добавляем стандартный
            if (sectors.Count == 0)
            {
                sectors["партер"] = (15, 20, 300);
                Console.WriteLine($"Внимание: не удалось распарсить расположение секторов для зала {Name}. Используется стандартная схема.");
            }
        }

        // Получить список секторов
        public List<string> GetSectors()
        {
            return sectors.Keys.ToList();
        }

        // Получить информацию о секторе
        public (int rows, int seatsPerRow, int totalSeats) GetSectorInfo(string sector)
        {
            if (sectors.ContainsKey(sector))
                return sectors[sector];
            return (0, 0, 0);
        }

        // TODO 2: Инициализировать карту мест на дату
        public void InitializeSeatMap(DateTime date)
        {
            // Если карта уже существует, не пересоздаем
            if (seatMaps.ContainsKey(date))
                return;

            var sectorMap = new Dictionary<string, bool[,]>();

            foreach (var sector in sectors)
            {
                // Создать новый массив bool[rows, seatsPerRow]
                bool[,] seats = new bool[sector.Value.rows, sector.Value.seatsPerRow];

                // Все места свободны (false)
                for (int i = 0; i < sector.Value.rows; i++)
                {
                    for (int j = 0; j < sector.Value.seatsPerRow; j++)
                    {
                        // Некоторые места могут быть техническими (проходы)
                        // Для простоты считаем все места доступными
                        seats[i, j] = false;
                    }
                }

                sectorMap[sector.Key] = seats;
            }

            // Добавить в seatMaps для указанной даты
            seatMaps[date] = sectorMap;
        }

        // TODO 2: Забронировать место
        public bool BookSeat(DateTime date, string sector, int row, int seat)
        {
            // Проверить что для даты существует карта мест
            if (!seatMaps.ContainsKey(date))
            {
                InitializeSeatMap(date);
            }

            // Проверить что сектор существует
            if (!sectors.ContainsKey(sector))
            {
                Console.WriteLine($"Ошибка: сектор '{sector}' не существует.");
                return false;
            }

            var sectorInfo = sectors[sector];

            // Проверить что место в пределах массива
            if (row < 1 || row > sectorInfo.rows || seat < 1 || seat > sectorInfo.seatsPerRow)
            {
                Console.WriteLine($"Ошибка: ряд {row}, место {seat} вне допустимого диапазона.");
                return false;
            }

            // Проверить что место свободно
            var seatMap = seatMaps[date];
            if (seatMap[sector][row - 1, seat - 1])
            {
                Console.WriteLine($"Ошибка: место {sector}, ряд {row}, место {seat} уже занято.");
                return false;
            }

            // Установить место как занятое (true)
            seatMap[sector][row - 1, seat - 1] = true;
            return true;
        }

        // TODO 2: Освободить место
        public bool FreeSeat(DateTime date, string sector, int row, int seat)
        {
            // Проверить что для даты существует карта мест
            if (!seatMaps.ContainsKey(date))
            {
                Console.WriteLine($"Ошибка: карта мест для даты {date:dd.MM.yyyy} не инициализирована.");
                return false;
            }

            // Проверить что сектор существует
            if (!sectors.ContainsKey(sector))
            {
                Console.WriteLine($"Ошибка: сектор '{sector}' не существует.");
                return false;
            }

            var sectorInfo = sectors[sector];

            // Проверить что место в пределах массива
            if (row < 1 || row > sectorInfo.rows || seat < 1 || seat > sectorInfo.seatsPerRow)
            {
                Console.WriteLine($"Ошибка: ряд {row}, место {seat} вне допустимого диапазона.");
                return false;
            }

            var seatMap = seatMaps[date];

            // Проверить что место занято
            if (!seatMap[sector][row - 1, seat - 1])
            {
                Console.WriteLine($"Ошибка: место {sector}, ряд {row}, место {seat} уже свободно.");
                return false;
            }

            // Освободить место (false)
            seatMap[sector][row - 1, seat - 1] = false;
            return true;
        }

        // TODO 3: Получить количество свободных мест
        public int GetAvailableSeats(DateTime date)
        {
            int available = 0;

            // Если для даты есть карта мест
            if (seatMaps.ContainsKey(date))
            {
                var seatMap = seatMaps[date];
                foreach (var sector in seatMap)
                {
                    bool[,] seats = sector.Value;
                    for (int i = 0; i < seats.GetLength(0); i++)
                    {
                        for (int j = 0; j < seats.GetLength(1); j++)
                        {
                            if (!seats[i, j])
                                available++;
                        }
                    }
                }
            }
            else
            {
                // Вернуть Capacity (все места свободны)
                available = Capacity;
            }

            return available;
        }

        // TODO 3: Проверить доступность места
        public bool IsSeatAvailable(DateTime date, string sector, int row, int seat)
        {
            // Проверить что сектор существует
            if (!sectors.ContainsKey(sector))
                return false;

            var sectorInfo = sectors[sector];

            // Проверить что место в пределах массива
            if (row < 1 || row > sectorInfo.rows || seat < 1 || seat > sectorInfo.seatsPerRow)
                return false;

            // Если для даты нет карты мест, место свободно
            if (!seatMaps.ContainsKey(date))
                return true;

            var seatMap = seatMaps[date];

            // Проверить что место свободно
            return !seatMap[sector][row - 1, seat - 1];
        }

        // TODO 1: Получить лучшие места
        public List<(string sector, int row, int seat)> GetBestSeats(DateTime date)
        {
            List<(string, int, int)> bestSeats = new List<(string, int, int)>();

            // Определить лучшие места (первые ряды партера)
            if (sectors.ContainsKey("партер"))
            {
                var sectorInfo = sectors["партер"];

                // Проверить доступность мест в первых 3 рядах
                for (int row = 1; row <= Math.Min(3, sectorInfo.rows); row++)
                {
                    for (int seat = 1; seat <= sectorInfo.seatsPerRow; seat++)
                    {
                        // Центральные места (примерно середина)
                        if (seat >= sectorInfo.seatsPerRow / 3 &&
                            seat <= 2 * sectorInfo.seatsPerRow / 3)
                        {
                            if (IsSeatAvailable(date, "партер", row, seat))
                            {
                                bestSeats.Add(("партер", row, seat));
                            }
                        }
                    }
                }
            }

            // Если нет мест в партере, ищем в бельэтаже
            if (bestSeats.Count == 0 && sectors.ContainsKey("бельэтаж"))
            {
                var sectorInfo = sectors["бельэтаж"];

                for (int row = 1; row <= Math.Min(2, sectorInfo.rows); row++)
                {
                    for (int seat = 1; seat <= sectorInfo.seatsPerRow; seat++)
                    {
                        if (IsSeatAvailable(date, "бельэтаж", row, seat))
                        {
                            bestSeats.Add(("бельэтаж", row, seat));
                        }
                    }
                }
            }

            return bestSeats;
        }

        // TODO 3: Получить статистику заполняемости
        public (int totalSeats, int bookedSeats, decimal occupancyRate) GetOccupancyStats(DateTime date)
        {
            int total = Capacity;
            int booked = 0;
            decimal rate = 0;

            if (seatMaps.ContainsKey(date))
            {
                var seatMap = seatMaps[date];
                foreach (var sector in seatMap)
                {
                    bool[,] seats = sector.Value;
                    for (int i = 0; i < seats.GetLength(0); i++)
                    {
                        for (int j = 0; j < seats.GetLength(1); j++)
                        {
                            if (seats[i, j])
                                booked++;
                        }
                    }
                }
            }

            if (total > 0)
                rate = (decimal)booked / total;

            return (total, booked, rate);
        }

        public void ShowHallInfo(DateTime? date = null)
        {
            Console.WriteLine($"Зал: {Name} (№{Number})");
            Console.WriteLine($"Вместимость: {Capacity} мест");

            // TODO 1: Вывести расположение секторов
            Console.WriteLine($"Расположение секторов: {SectorLayout}");
            Console.WriteLine($"Оборудование: {Equipment}");

            if (date.HasValue)
            {
                var stats = GetOccupancyStats(date.Value);
                Console.WriteLine($"\nНа {date:dd.MM.yyyy}:");
                Console.WriteLine($"  Всего мест: {stats.totalSeats}");
                Console.WriteLine($"  Занято мест: {stats.bookedSeats}");
                Console.WriteLine($"  Заполняемость: {stats.occupancyRate:P0}");

                int available = GetAvailableSeats(date.Value);
                Console.WriteLine($"  Свободных мест: {available}");

                if (available > 0)
                {
                    var bestSeats = GetBestSeats(date.Value);
                    if (bestSeats.Count > 0)
                    {
                        Console.WriteLine($"  Доступные лучшие места: {bestSeats.Count}");
                    }
                }
            }
        }

        // TODO 2: Показать схему зала
        public void ShowSeatMap(DateTime date)
        {
            Console.WriteLine($"\n=== Схема зала '{Name}' на {date:dd.MM.yyyy HH:mm} ===");
            Console.WriteLine("   [СЦЕНА]");
            Console.WriteLine("   =========");

            if (!seatMaps.ContainsKey(date))
            {
                InitializeSeatMap(date);
            }

            var seatMap = seatMaps[date];

            foreach (var sector in seatMap)
            {
                Console.WriteLine($"\n--- Сектор: {sector.Key} ---");
                bool[,] seats = sector.Value;

                // Заголовок с номерами мест
                Console.Write("Ряд\\Место ");
                for (int seat = 1; seat <= Math.Min(seats.GetLength(1), 20); seat++)
                {
                    Console.Write($"{seat,3}");
                }
                Console.WriteLine();

                // Схема мест
                for (int row = 0; row < Math.Min(seats.GetLength(0), 10); row++)
                {
                    Console.Write($"  {row + 1,2}      ");

                    for (int seat = 0; seat < Math.Min(seats.GetLength(1), 20); seat++)
                    {
                        if (seats[row, seat])
                            Console.Write(" [X]");
                        else
                            Console.Write(" [ ]");
                    }
                    Console.WriteLine();
                }

                if (seats.GetLength(0) > 10 || seats.GetLength(1) > 20)
                {
                    Console.WriteLine($"  ... (всего {seats.GetLength(0)} рядов, {seats.GetLength(1)} мест в ряду)");
                }
            }

            Console.WriteLine("\n[ ] - свободно, [X] - занято");
        }
    }
}