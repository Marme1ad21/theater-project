// TODO:
// 1. Реализовано хранение спектаклей, билетов, актеров и залов
// 2. Реализована продажа и бронирование билетов
// 3. Реализован учет кассовых сборов и посещаемости

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;

namespace Theater
{
    public class TheaterManager
    {
        private List<Performance> performances = new List<Performance>();
        private List<Ticket> tickets = new List<Ticket>();
        private List<Actor> actors = new List<Actor>();
        private List<Hall> halls = new List<Hall>();

        private int nextTicketId = 10000;
        private decimal dailyRevenue = 0;
        private int dailyTicketsSold = 0;
        private DateTime currentDate = DateTime.Now.Date;

        // TODO 1: Добавить спектакль
        public void AddPerformance(Performance performance)
        {
            // Проверка на дубликат
            if (performances.Any(p => p.Id == performance.Id))
            {
                Console.WriteLine($"Ошибка: спектакль с ID {performance.Id} уже существует.");
                return;
            }

            performances.Add(performance);
            Console.WriteLine($"Спектакль '{performance.Title}' добавлен в репертуар.");
        }

        // TODO 1: Добавить актера
        public void AddActor(Actor actor)
        {
            // Проверка на дубликат
            if (actors.Any(a => a.Id == actor.Id))
            {
                Console.WriteLine($"Ошибка: актер с ID {actor.Id} уже существует.");
                return;
            }

            actors.Add(actor);
            Console.WriteLine($"Актер {actor.FullName} добавлен в труппу.");
        }

        // TODO 1: Добавить зал
        public void AddHall(Hall hall)
        {
            // Проверка на дубликат
            if (halls.Any(h => h.Number == hall.Number))
            {
                Console.WriteLine($"Ошибка: зал с номером {hall.Number} уже существует.");
                return;
            }

            halls.Add(hall);
            Console.WriteLine($"Зал '{hall.Name}' (№{hall.Number}) добавлен.");
        }

        // TODO 2: Найти спектакль по названию
        public Performance FindPerformanceByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            // Пройти по всем спектаклям, ищем по части названия
            foreach (var performance in performances)
            {
                if (performance.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0)
                    return performance;
            }

            return null;
        }

        // Найти все спектакли по названию
        public List<Performance> FindAllPerformancesByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return new List<Performance>();

            return performances.Where(p => p.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0)
                              .ToList();
        }

        // TODO 2: Продать билет
        public Ticket SellTicket(Performance performance, Performance.Show show, string sector, int row, int seat, int buyerAge = 18)
        {
            // Проверка возрастного ограничения
            if (!performance.IsAgeAppropriate(buyerAge))
            {
                Console.WriteLine($"Ошибка: возрастное ограничение {performance.AgeRating}. Покупка недоступна для возраста {buyerAge}.");
                return null;
            }

            // Найти зал по номеру из show.HallNumber
            Hall hall = GetHallByNumber(show.HallNumber);
            if (hall == null)
            {
                Console.WriteLine($"Ошибка: зал №{show.HallNumber} не найден.");
                return null;
            }

            // Проверить что место доступно
            if (!hall.IsSeatAvailable(show.Date, sector, row, seat))
            {
                Console.WriteLine($"Ошибка: место {sector}, ряд {row}, место {seat} недоступно.");
                return null;
            }

            // Забронировать место
            if (!hall.BookSeat(show.Date, sector, row, seat))
            {
                Console.WriteLine($"Ошибка: не удалось забронировать место.");
                return null;
            }

            // Создать новый Ticket
            Ticket ticket = new Ticket(nextTicketId, performance, show, sector, row, seat);

            // Увеличить nextTicketId
            nextTicketId++;

            // Добавить билет в список tickets
            tickets.Add(ticket);

            // Обновить статистику
            dailyRevenue += ticket.Price;
            dailyTicketsSold++;

            Console.WriteLine($"Билет №{ticket.Id} продан успешно. Цена: {ticket.Price} руб.");
            return ticket;
        }

        // TODO 3: Вернуть билет
        public bool ReturnTicket(int ticketId)
        {
            // Найти билет по Id
            Ticket ticket = null;
            foreach (var t in tickets)
            {
                if (t.Id == ticketId)
                {
                    ticket = t;
                    break;
                }
            }

            if (ticket == null)
            {
                Console.WriteLine($"Ошибка: билет №{ticketId} не найден.");
                return false;
            }

            // Проверить что билет действителен
            if (!ticket.IsValid())
            {
                Console.WriteLine($"Ошибка: билет №{ticketId} недействителен (показ уже прошел или билет использован).");
                return false;
            }

            // Найти зал
            Hall hall = GetHallByNumber(ticket.Show.HallNumber);
            if (hall == null)
            {
                Console.WriteLine($"Ошибка: зал №{ticket.Show.HallNumber} не найден.");
                return false;
            }

            // Освободить место
            if (!hall.FreeSeat(ticket.Show.Date, ticket.Sector, ticket.Row, ticket.Seat))
            {
                Console.WriteLine($"Ошибка: не удалось освободить место.");
                return false;
            }

            // Обновить статистику
            dailyRevenue -= ticket.Price;
            dailyTicketsSold--;

            // Пометить билет как возвращенный (удаляем из списка)
            tickets.Remove(ticket);

            Console.WriteLine($"Билет №{ticketId} возвращен. Сумма возврата: {ticket.Price} руб.");
            return true;
        }

        // TODO 3: Получить статистику театра
        public (decimal revenue, int ticketsSold, int performancesCount, double avgOccupancy) GetTheaterStats()
        {
            decimal revenue = dailyRevenue;
            int ticketsSold = dailyTicketsSold;
            int performancesCount = performances.Count;

            // Расчет средней заполняемости залов
            double avgOccupancy = 0;
            int totalShows = 0;

            foreach (var performance in performances)
            {
                var upcomingShows = performance.GetUpcomingShows();
                foreach (var show in upcomingShows)
                {
                    Hall hall = GetHallByNumber(show.HallNumber);
                    if (hall != null)
                    {
                        var stats = hall.GetOccupancyStats(show.Date);
                        avgOccupancy += (double)stats.occupancyRate;
                        totalShows++;
                    }
                }
            }

            if (totalShows > 0)
                avgOccupancy /= totalShows;

            return (Math.Round(revenue, 2), ticketsSold, performancesCount, Math.Round(avgOccupancy, 1));
        }

        // TODO 2: Найти актера по имени
        public Actor FindActorByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            // Пройти по всем актерам, ищем по части имени
            foreach (var actor in actors)
            {
                if (actor.FullName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                    return actor;
            }

            return null;
        }

        // Найти всех актеров по имени
        public List<Actor> FindAllActorsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Actor>();

            return actors.Where(a => a.FullName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
        }

        // TODO 3: Получить ближайшие спектакли
        public List<Performance> GetUpcomingPerformances(int daysAhead = 7)
        {
            List<Performance> upcoming = new List<Performance>();
            DateTime endDate = DateTime.Now.AddDays(daysAhead);

            // Пройти по всем спектаклям
            foreach (var performance in performances)
            {
                // Проверить есть ли показы в ближайшие daysAhead дней
                var upcomingShows = performance.GetUpcomingShows()
                    .Where(s => s.Date <= endDate)
                    .ToList();

                if (upcomingShows.Count > 0)
                    upcoming.Add(performance);
            }

            return upcoming;
        }

        // Получить самые популярные спектакли (по количеству проданных билетов)
        public List<(Performance performance, int ticketsSold)> GetPopularPerformances(int count = 3)
        {
            var result = new Dictionary<Performance, int>();

            foreach (var ticket in tickets)
            {
                if (result.ContainsKey(ticket.Performance))
                    result[ticket.Performance]++;
                else
                    result[ticket.Performance] = 1;
            }

            return result.OrderByDescending(kv => kv.Value)
                        .Take(count)
                        .Select(kv => (kv.Key, kv.Value))
                        .ToList();
        }

        // Готовые методы:
        public List<Performance> GetAllPerformances()
        {
            return performances;
        }

        public List<Actor> GetAllActors()
        {
            return actors;
        }

        public List<Hall> GetAllHalls()
        {
            return halls;
        }

        public List<Ticket> GetAllTickets()
        {
            return tickets;
        }

        public Performance GetPerformanceById(int id)
        {
            foreach (var performance in performances)
            {
                if (performance.Id == id)
                    return performance;
            }
            return null;
        }

        public Hall GetHallByNumber(int number)
        {
            foreach (var hall in halls)
            {
                if (hall.Number == number)
                    return hall;
            }
            return null;
        }

        public int GetNextTicketId()
        {
            return nextTicketId++;
        }

        public void ResetDailyStats()
        {
            if (currentDate.Date != DateTime.Now.Date)
            {
                dailyRevenue = 0;
                dailyTicketsSold = 0;
                currentDate = DateTime.Now.Date;
            }
        }
    }
}