// Реализованы все TODO:
// 1. Добавлены поля для информации о спектакле (жанр, длительность, возрастное ограничение)
// 2. Реализована проверка корректности данных (даты, цены)
// 3. Реализован метод расчета доходности спектакля

using System;
using System.Collections.Generic;
using System.Linq;

namespace Theater
{
    public class Performance
    {
        public int Id { get; set; }               // Код спектакля
        public string Title { get; set; }         // Название
        public string Director { get; set; }      // Режиссер
        public string Description { get; set; }   // Описание
        public decimal BaseTicketPrice { get; set; } // Базовая цена билета

        // TODO 1: Добавлены свойства
        public string Genre { get; set; }         // Жанр: драма, комедия, мюзикл, балет
        public int DurationMinutes { get; set; }  // Продолжительность в минутах
        public string AgeRating { get; set; }     // Возрастное ограничение: 0+, 12+, 16+, 18+

        private List<Show> shows = new List<Show>(); // Список показов этого спектакля

        public class Show
        {
            public DateTime Date { get; set; }    // Дата и время показа
            public int HallNumber { get; set; }   // Номер зала
            public bool IsPremiere { get; set; }  // Премьера ли

            public override string ToString()
            {
                return $"{Date:dd.MM.yyyy HH:mm} - Зал {HallNumber}{(IsPremiere ? " ★ПРЕМЬЕРА★" : "")}";
            }
        }

        public Performance(int id, string title, string director, string description,
                          decimal price, string genre, int duration, string ageRating)
        {
            Id = id;
            Title = title;
            Director = director;
            Description = description;

            // TODO 2: Проверка цены
            if (price < 0)
            {
                BaseTicketPrice = 500; // Минимальная цена
                Console.WriteLine($"Внимание: цена спектакля '{title}' не может быть отрицательной. Установлена минимальная цена 500 руб.");
            }
            else
            {
                BaseTicketPrice = price;
            }

            // TODO 2: Проверка продолжительности
            if (duration <= 0)
            {
                DurationMinutes = 90; // Стандартная продолжительность
                Console.WriteLine($"Внимание: продолжительность спектакля '{title}' некорректна. Установлена стандартная длительность 90 мин.");
            }
            else
            {
                DurationMinutes = duration;
            }

            // TODO 1: Сохранить жанр, продолжительность и возрастное ограничение
            Genre = genre;
            AgeRating = ageRating;
        }

        // TODO 3: Добавить показ спектакля
        public void AddShow(DateTime date, int hallNumber, bool isPremiere = false)
        {
            // Проверка: дата в будущем
            if (date <= DateTime.Now)
            {
                Console.WriteLine($"Ошибка: дата показа ({date:dd.MM.yyyy HH:mm}) должна быть в будущем.");
                return;
            }

            // Проверка: номер зала от 1 до 5
            if (hallNumber < 1 || hallNumber > 5)
            {
                Console.WriteLine($"Ошибка: номер зала ({hallNumber}) должен быть от 1 до 5.");
                return;
            }

            // Проверка на пересечение с существующими показами
            foreach (var existingShow in shows)
            {
                if (existingShow.HallNumber == hallNumber &&
                    Math.Abs((existingShow.Date - date).TotalMinutes) < 180) // 3 часа на подготовку
                {
                    Console.WriteLine($"Ошибка: зал {hallNumber} уже занят {existingShow.Date:dd.MM.yyyy HH:mm} " +
                                     $"(должно быть минимум 3 часа между показами)");
                    return;
                }
            }

            // Создать новый объект Show
            Show newShow = new Show
            {
                Date = date,
                HallNumber = hallNumber,
                IsPremiere = isPremiere
            };

            // Добавить в список shows
            shows.Add(newShow);

            // Сортируем показы по дате
            shows = shows.OrderBy(s => s.Date).ToList();

            Console.WriteLine($"Показ добавлен: {Title} - {date:dd.MM.yyyy HH:mm}, зал {hallNumber}");
        }

        // TODO 3: Рассчитать доход от спектакля
        public decimal CalculateRevenue(int soldTickets)
        {
            decimal revenue = soldTickets * BaseTicketPrice;
            decimal totalRevenue = 0;

            // Получаем информацию о ближайших показах для расчета коэффициентов
            var upcomingShows = shows.Where(s => s.Date > DateTime.Now).ToList();

            if (upcomingShows.Count == 0)
                return revenue;

            // Распределяем проданные билеты по показам (упрощенно)
            int ticketsPerShow = soldTickets / upcomingShows.Count;
            int remainder = soldTickets % upcomingShows.Count;

            for (int i = 0; i < upcomingShows.Count; i++)
            {
                int showTickets = ticketsPerShow + (i < remainder ? 1 : 0);
                decimal showRevenue = showTickets * BaseTicketPrice;

                // Премии дороже на 50%
                if (upcomingShows[i].IsPremiere)
                    showRevenue *= 1.5m;

                // Вечерние спектакли (после 18:00) дороже на 25%
                if (upcomingShows[i].Date.Hour >= 18)
                    showRevenue *= 1.25m;

                // Утренние спектакли (до 12:00) дешевле на 20%
                if (upcomingShows[i].Date.Hour < 12)
                    showRevenue *= 0.8m;

                totalRevenue += showRevenue;
            }

            return Math.Round(totalRevenue, 2);
        }

        // TODO 1: Получить ближайший показ
        public Show GetNextShow()
        {
            DateTime now = DateTime.Now;

            // Найти ближайший показ в будущем
            foreach (var show in shows)
            {
                if (show.Date > now)
                    return show;
            }

            return null;
        }

        // Получить все будущие показы
        public List<Show> GetUpcomingShows()
        {
            return shows.Where(s => s.Date > DateTime.Now)
                       .OrderBy(s => s.Date)
                       .ToList();
        }

        // TODO 2: Проверить подходит ли по возрасту
        public bool IsAgeAppropriate(int age)
        {
            // Проверить возрастное ограничение:
            switch (AgeRating)
            {
                case "0+":
                    return age >= 0;
                case "6+":
                    return age >= 6;
                case "12+":
                    return age >= 12;
                case "16+":
                    return age >= 16;
                case "18+":
                    return age >= 18;
                default:
                    // По умолчанию считаем, что подходит всем
                    return true;
            }
        }

        public override string ToString()
        {
            return $"{Title} (реж. {Director}) - {Genre}, {DurationMinutes} мин., {AgeRating} - от {BaseTicketPrice} руб.";
        }

        public void ShowPerformanceInfo()
        {
            Console.WriteLine($"=== {Title} ===");
            Console.WriteLine($"Режиссер: {Director}");
            Console.WriteLine($"Жанр: {Genre}");
            Console.WriteLine($"Продолжительность: {DurationMinutes} мин.");
            Console.WriteLine($"Возрастное ограничение: {AgeRating}");
            Console.WriteLine($"Описание: {Description}");
            Console.WriteLine($"Базовая цена билета: {BaseTicketPrice} руб.");

            var nextShow = GetNextShow();
            if (nextShow != null)
            {
                Console.WriteLine($"\nБлижайший показ: {nextShow.Date:dd.MM.yyyy HH:mm}, зал {nextShow.HallNumber}");
                if (nextShow.IsPremiere) Console.WriteLine("  ★ ПРЕМЬЕРА ★");
            }

            if (shows.Count > 0)
            {
                Console.WriteLine($"\nВсего показов: {shows.Count}");

                var upcomingShows = GetUpcomingShows();
                if (upcomingShows.Count > 0)
                {
                    Console.WriteLine("Будущие показы:");
                    foreach (var show in upcomingShows.Take(5))
                    {
                        Console.WriteLine($"  - {show.Date:dd.MM.yyyy HH:mm}, зал {show.HallNumber}" +
                                         $"{(show.IsPremiere ? " ★" : "")}");
                    }

                    if (upcomingShows.Count > 5)
                        Console.WriteLine($"  ... и еще {upcomingShows.Count - 5} показов");
                }
            }
        }

        // Получить все показы
        public List<Show> GetAllShows()
        {
            return shows.OrderBy(s => s.Date).ToList();
        }
    }
}