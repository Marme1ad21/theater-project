// TODO:
// 1. Реализован просмотр репертуара и информации о спектаклях
// 2. Реализована продажа и бронирование билетов
// 3. Реализовано управление труппой и отчетность

using System;
using System.Collections.Generic;
using System.Linq;

namespace Theater
{
    public class TheaterMenu
    {
        private TheaterManager manager;

        public TheaterMenu()
        {
            manager = new TheaterManager();
            InitializeData();
        }

        private void InitializeData()
        {
            // Инициализация залов
            manager.AddHall(new Hall(1, "Большой зал", 500, "партер:200,бельэтаж:150,балкон:150",
                "современное звуковое и световое оборудование, проектор 4K"));
            manager.AddHall(new Hall(2, "Малый зал", 150, "партер:100,ложи:50",
                "камерная акустика, рояль"));
            manager.AddHall(new Hall(3, "Экспериментальная сцена", 80, "амфитеатр:80",
                "мультимедийное оборудование, светодиодные экраны"));

            // Инициализация спектаклей
            Performance performance1 = new Performance(1, "Вишневый сад", "А.П. Чехов",
                                                      "Классическая русская драма о дворянской усадьбе на пороге перемен",
                                                      1500, "драма", 120, "12+");
            performance1.AddShow(DateTime.Now.AddDays(1).AddHours(19), 1, false);
            performance1.AddShow(DateTime.Now.AddDays(3).AddHours(18), 1, false);
            performance1.AddShow(DateTime.Now.AddDays(7).AddHours(19), 1, false);
            manager.AddPerformance(performance1);

            Performance performance2 = new Performance(2, "Сон в летнюю ночь", "У. Шекспир",
                                                      "Волшебная комедия о любви, снах и превращениях",
                                                      1800, "комедия", 110, "6+");
            performance2.AddShow(DateTime.Now.AddDays(2).AddHours(20), 1, true);
            performance2.AddShow(DateTime.Now.AddDays(5).AddHours(19), 1, false);
            performance2.AddShow(DateTime.Now.AddDays(8).AddHours(19), 1, false);
            manager.AddPerformance(performance2);

            Performance performance3 = new Performance(3, "Щелкунчик", "П.И. Чайковский",
                                                      "Рождественский балет в двух действиях",
                                                      2500, "балет", 90, "0+");
            performance3.AddShow(DateTime.Now.AddDays(4).AddHours(12), 2, false);
            performance3.AddShow(DateTime.Now.AddDays(6).AddHours(15), 2, false);
            performance3.AddShow(DateTime.Now.AddDays(10).AddHours(12), 2, false);
            manager.AddPerformance(performance3);

            Performance performance4 = new Performance(4, "Ревизор", "Н.В. Гоголь",
                                                      "Сатирическая комедия о чиновниках и мнимом ревизоре",
                                                      1600, "комедия", 130, "16+");
            performance4.AddShow(DateTime.Now.AddDays(9).AddHours(19), 3, false);
            performance4.AddShow(DateTime.Now.AddDays(12).AddHours(18), 3, true);
            manager.AddPerformance(performance4);

            // Инициализация актеров
            Actor actor1 = new Actor(1, "Сергеев Иван Петрович", new DateTime(1985, 3, 15),
                                    new DateTime(2010, 9, 1), "трагик", "вокал, фехтование");
            actor1.AddRole(performance1, "Гаев", true);
            actor1.AddRole(performance4, "Городничий", true);
            manager.AddActor(actor1);

            Actor actor2 = new Actor(2, "Петрова Елена Владимировна", new DateTime(1990, 7, 22),
                                    new DateTime(2015, 3, 10), "лирический", "вокал, танец, фехтование");
            actor2.AddRole(performance2, "Титания", true);
            actor2.AddRole(performance3, "Мари", true);
            manager.AddActor(actor2);

            Actor actor3 = new Actor(3, "Козлов Александр Дмитриевич", new DateTime(1978, 11, 5),
                                    new DateTime(2005, 1, 15), "комик", "импровизация, акробатика, вокал");
            actor3.AddRole(performance2, "Пэк", true);
            actor3.AddRole(performance4, "Хлестаков", true);
            manager.AddActor(actor3);

            Actor actor4 = new Actor(4, "Соколова Анна Михайловна", new DateTime(1995, 2, 10),
                                    new DateTime(2018, 8, 20), "характерный", "вокал, танец");
            actor4.AddRole(performance1, "Раневская", true);
            actor4.AddRole(performance3, "Фея Драже", false);
            manager.AddActor(actor4);

            Console.WriteLine("Театр успешно инициализирован!");
        }

        // TODO 1: Показать репертуар
        public void ShowRepertoire()
        {
            Console.Clear();
            Console.WriteLine("=== РЕПЕРТУАР ТЕАТРА ===\n");

            var performances = manager.GetAllPerformances();

            if (performances.Count == 0)
            {
                Console.WriteLine("В репертуаре нет спектаклей.");
                return;
            }

            foreach (var performance in performances)
            {
                Console.WriteLine($"ID: {performance.Id}");
                Console.WriteLine($"Название: {performance.Title}");
                Console.WriteLine($"Режиссер: {performance.Director}");
                Console.WriteLine($"Жанр: {performance.Genre}, {performance.DurationMinutes} мин., {performance.AgeRating}");
                Console.WriteLine($"Цена билета: от {performance.BaseTicketPrice} руб.");

                var nextShow = performance.GetNextShow();
                if (nextShow != null)
                {
                    Console.WriteLine($"Ближайший показ: {nextShow.Date:dd.MM.yyyy HH:mm}, зал {nextShow.HallNumber}" +
                                     $"{(nextShow.IsPremiere ? " ★ПРЕМЬЕРА★" : "")}");
                }
                else
                {
                    Console.WriteLine("Нет ближайших показов");
                }

                Console.WriteLine(new string('-', 50));
            }

            Console.WriteLine($"\nВсего спектаклей: {performances.Count}");
        }

        // TODO 1: Показать информацию о спектакле
        public void ShowPerformanceDetails()
        {
            Console.Clear();
            Console.WriteLine("=== ИНФОРМАЦИЯ О СПЕКТАКЛЕ ===\n");
            Console.Write("Введите название спектакля: ");
            string title = Console.ReadLine();

            var performances = manager.FindAllPerformancesByTitle(title);

            if (performances.Count == 0)
            {
                Console.WriteLine("Спектакль не найден");
                return;
            }

            if (performances.Count > 1)
            {
                Console.WriteLine("\nНайдено несколько спектаклей:");
                for (int i = 0; i < performances.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {performances[i].Title}");
                }

                Console.Write("Выберите номер: ");
                if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > performances.Count)
                {
                    Console.WriteLine("Неверный выбор");
                    return;
                }

                performances[choice - 1].ShowPerformanceInfo();

                // Показать доступные места на ближайший показ
                var nextShow = performances[choice - 1].GetNextShow();
                if (nextShow != null)
                {
                    Hall hall = manager.GetHallByNumber(nextShow.HallNumber);
                    if (hall != null)
                    {
                        Console.WriteLine("\nДоступность мест:");
                        hall.ShowHallInfo(nextShow.Date);

                        Console.Write("\nПоказать схему зала? (д/н): ");
                        if (Console.ReadLine()?.ToLower() == "д")
                        {
                            hall.ShowSeatMap(nextShow.Date);
                        }
                    }
                }
            }
            else
            {
                performances[0].ShowPerformanceInfo();

                var nextShow = performances[0].GetNextShow();
                if (nextShow != null)
                {
                    Hall hall = manager.GetHallByNumber(nextShow.HallNumber);
                    if (hall != null)
                    {
                        Console.WriteLine("\nДоступность мест:");
                        hall.ShowHallInfo(nextShow.Date);

                        Console.Write("\nПоказать схему зала? (д/н): ");
                        if (Console.ReadLine()?.ToLower() == "д")
                        {
                            hall.ShowSeatMap(nextShow.Date);
                        }
                    }
                }
            }
        }

        // TODO 2: Купить билет (ИСПРАВЛЕНО)
        public void BuyTicket()
        {
            Console.Clear();
            Console.WriteLine("=== ПОКУПКА БИЛЕТА ===\n");

            // 1. Показать репертуар
            ShowRepertoire();

            // 2. Выбрать спектакль
            Console.Write("\nВведите ID спектакля: ");
            if (!int.TryParse(Console.ReadLine(), out int perfId))
            {
                Console.WriteLine("Неверный ID");
                return;
            }

            Performance performance = manager.GetPerformanceById(perfId);
            if (performance == null)
            {
                Console.WriteLine("Спектакль не найден");
                return;
            }

            // 3. Выбрать дату показа
            var upcomingShows = performance.GetUpcomingShows();
            if (upcomingShows.Count == 0)
            {
                Console.WriteLine("Нет предстоящих показов этого спектакля");
                return;
            }

            Console.WriteLine("\nДоступные показы:");
            for (int i = 0; i < upcomingShows.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {upcomingShows[i].Date:dd.MM.yyyy HH:mm}, зал {upcomingShows[i].HallNumber}" +
                                 $"{(upcomingShows[i].IsPremiere ? " ★ПРЕМЬЕРА★" : "")}");
            }

            Console.Write("Выберите показ (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int showIndex) ||
                showIndex < 1 || showIndex > upcomingShows.Count)
            {
                Console.WriteLine("Неверный выбор");
                return;
            }

            var selectedShow = upcomingShows[showIndex - 1];

            // Проверка возрастного ограничения
            Console.Write("\nВведите ваш возраст: ");
            if (!int.TryParse(Console.ReadLine(), out int age))
            {
                Console.WriteLine("Неверный возраст");
                return;
            }

            if (!performance.IsAgeAppropriate(age))
            {
                Console.WriteLine($"Ошибка: возрастное ограничение {performance.AgeRating}. Продажа билетов недоступна.");
                return;
            }

            // 4. Выбрать место
            Hall hall = manager.GetHallByNumber(selectedShow.HallNumber);
            if (hall == null)
            {
                Console.WriteLine("Ошибка: зал не найден");
                return;
            }

            // Показать доступные сектора
            var sectors = hall.GetSectors();
            Console.WriteLine("\nДоступные сектора:");
            foreach (var sector in sectors)
            {
                var sectorInfo = hall.GetSectorInfo(sector);
                int available = 0;

                // Подсчет доступных мест в секторе (упрощенно) - ИСПРАВЛЕНО: переименованы переменные цикла
                for (int r = 1; r <= Math.Min(sectorInfo.rows, 5); r++)
                {
                    for (int s = 1; s <= Math.Min(sectorInfo.seatsPerRow, 10); s++)
                    {
                        if (hall.IsSeatAvailable(selectedShow.Date, sector, r, s))
                            available++;
                    }
                }

                Console.WriteLine($"  {sector}: {sectorInfo.rows} рядов, {sectorInfo.seatsPerRow} мест в ряду (доступно ~{available}+ мест)");
            }

            Console.Write("\nВыберите сектор: ");
            string selectedSector = Console.ReadLine();

            if (!sectors.Contains(selectedSector))
            {
                Console.WriteLine("Неверный сектор");
                return;
            }

            var selSectorInfo = hall.GetSectorInfo(selectedSector);
            Console.WriteLine($"Ряды: 1-{selSectorInfo.rows}, места в ряду: 1-{selSectorInfo.seatsPerRow}");

            Console.Write("Введите ряд: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedRow))
            {
                Console.WriteLine("Неверный ряд");
                return;
            }

            Console.Write("Введите место: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedSeat))
            {
                Console.WriteLine("Неверное место");
                return;
            }

            // 6. Продать билет
            Ticket ticket = manager.SellTicket(performance, selectedShow, selectedSector, selectedRow, selectedSeat, age);

            if (ticket != null)
            {
                Console.WriteLine("\nБилет успешно куплен!");
                ticket.ShowTicketInfo();
            }
        }

        // TODO 2: Вернуть билет
        public void ReturnTicket()
        {
            Console.Clear();
            Console.WriteLine("=== ВОЗВРАТ БИЛЕТА ===\n");

            Console.Write("Введите номер билета: ");
            if (int.TryParse(Console.ReadLine(), out int ticketId))
            {
                if (manager.ReturnTicket(ticketId))
                {
                    Console.WriteLine("Билет успешно возвращен. Деньги будут возвращены на карту.");
                }
                else
                {
                    Console.WriteLine("Не удалось вернуть билет. Проверьте номер билета и дату показа.");
                }
            }
            else
            {
                Console.WriteLine("Неверный номер билета");
            }
        }

        // TODO 1: Показать труппу
        public void ShowActors()
        {
            Console.Clear();
            Console.WriteLine("=== ТРУППА ТЕАТРА ===\n");

            var actors = manager.GetAllActors();

            if (actors.Count == 0)
            {
                Console.WriteLine("В труппе нет актеров.");
                return;
            }

            foreach (var actor in actors)
            {
                actor.ShowActorInfo();
                Console.WriteLine(new string('-', 60));
            }

            Console.WriteLine($"\nВсего актеров: {actors.Count}");
        }

        // TODO 1: Показать информацию о залах
        public void ShowHallsInfo()
        {
            Console.Clear();
            Console.WriteLine("=== ЗАЛЫ ТЕАТРА ===\n");

            var halls = manager.GetAllHalls();

            if (halls.Count == 0)
            {
                Console.WriteLine("Залы не найдены.");
                return;
            }

            foreach (var hall in halls)
            {
                hall.ShowHallInfo();
                Console.WriteLine(new string('-', 50));
            }

            Console.WriteLine($"\nВсего залов: {halls.Count}");
        }

        // TODO 3: Забронировать место (ИСПРАВЛЕНО)
        public void ReserveSeat()
        {
            Console.Clear();
            Console.WriteLine("=== БРОНИРОВАНИЕ МЕСТА ===\n");

            // 1. Выбрать спектакль и дату
            ShowRepertoire();

            Console.Write("\nВведите ID спектакля: ");
            if (!int.TryParse(Console.ReadLine(), out int perfId))
            {
                Console.WriteLine("Неверный ID");
                return;
            }

            Performance performance = manager.GetPerformanceById(perfId);
            if (performance == null)
            {
                Console.WriteLine("Спектакль не найден");
                return;
            }

            var upcomingShows = performance.GetUpcomingShows();
            if (upcomingShows.Count == 0)
            {
                Console.WriteLine("Нет предстоящих показов этого спектакля");
                return;
            }

            Console.WriteLine("\nДоступные показы:");
            for (int i = 0; i < upcomingShows.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {upcomingShows[i].Date:dd.MM.yyyy HH:mm}, зал {upcomingShows[i].HallNumber}");
            }

            Console.Write("Выберите показ (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int showIndex) ||
                showIndex < 1 || showIndex > upcomingShows.Count)
            {
                Console.WriteLine("Неверный выбор");
                return;
            }

            var selectedShow = upcomingShows[showIndex - 1];

            // 2. Показать схему зала с доступными местами
            Hall hall = manager.GetHallByNumber(selectedShow.HallNumber);
            if (hall == null)
            {
                Console.WriteLine("Ошибка: зал не найден");
                return;
            }

            hall.ShowSeatMap(selectedShow.Date);

            // 3. Выбрать место
            Console.Write("\nВведите сектор: ");
            string selectedSector = Console.ReadLine();

            // Проверка существования сектора
            var sectors = hall.GetSectors();
            if (!sectors.Contains(selectedSector))
            {
                Console.WriteLine("Неверный сектор");
                return;
            }

            Console.Write("Введите ряд: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedRow))
            {
                Console.WriteLine("Неверный ряд");
                return;
            }

            Console.Write("Введите место: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedSeat))
            {
                Console.WriteLine("Неверное место");
                return;
            }

            // 4. Забронировать без покупки
            if (hall.BookSeat(selectedShow.Date, selectedSector, selectedRow, selectedSeat))
            {
                string reservationNumber = $"BR{DateTime.Now:yyMMddHHmmss}{new Random().Next(100, 999)}";
                Console.WriteLine($"\nМесто успешно забронировано!");
                Console.WriteLine($"Номер брони: {reservationNumber}");
                Console.WriteLine($"Бронь действительна 24 часа до {DateTime.Now.AddHours(24):dd.MM.yyyy HH:mm}");
                Console.WriteLine($"\nСпектакль: {performance.Title}");
                Console.WriteLine($"Дата: {selectedShow.Date:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"Место: {selectedSector}, ряд {selectedRow}, место {selectedSeat}");
                Console.WriteLine($"Цена: {new Ticket(9999, performance, selectedShow, selectedSector, selectedRow, selectedSeat).Price} руб.");
                Console.WriteLine("\nДля покупки билета обратитесь в кассу или используйте пункт меню 'Купить билет'.");
            }
            else
            {
                Console.WriteLine("Не удалось забронировать место. Возможно, оно уже занято.");
            }
        }

        // TODO 3: Показать отчет
        public void ShowTheaterReport()
        {
            Console.Clear();
            Console.WriteLine("=== ОТЧЕТ ТЕАТРА ===\n");

            manager.ResetDailyStats();
            var stats = manager.GetTheaterStats();

            Console.WriteLine($"Дата отчета: {DateTime.Now:dd.MM.yyyy}");
            Console.WriteLine($"Выручка за день: {stats.revenue} руб.");
            Console.WriteLine($"Продано билетов: {stats.ticketsSold}");
            Console.WriteLine($"Количество спектаклей в репертуаре: {stats.performancesCount}");
            Console.WriteLine($"Средняя заполняемость залов: {stats.avgOccupancy}%");

            // Показать самые популярные спектакли
            var popularPerformances = manager.GetPopularPerformances(3);
            if (popularPerformances.Count > 0)
            {
                Console.WriteLine("\n=== САМЫЕ ПОПУЛЯРНЫЕ СПЕКТАКЛИ ===");
                for (int i = 0; i < popularPerformances.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {popularPerformances[i].performance.Title} - {popularPerformances[i].ticketsSold} билетов");
                }
            }

            // Показать ближайшие премьеры
            Console.WriteLine("\n=== БЛИЖАЙШИЕ ПРЕМЬЕРЫ ===");
            var allPerformances = manager.GetAllPerformances();
            bool hasPremieres = false;

            foreach (var perf in allPerformances)
            {
                var upcomingShows = perf.GetUpcomingShows();
                foreach (var show in upcomingShows)
                {
                    if (show.IsPremiere)
                    {
                        Console.WriteLine($"{perf.Title} - {show.Date:dd.MM.yyyy HH:mm}, зал {show.HallNumber}");
                        hasPremieres = true;
                        break;
                    }
                }
            }

            if (!hasPremieres)
            {
                Console.WriteLine("Нет ближайших премьер");
            }

            // Показать загруженность актеров
            Console.WriteLine("\n=== ЗАГРУЖЕННОСТЬ АКТЕРОВ ===");
            var actors = manager.GetAllActors();
            foreach (var actor in actors.Take(5))
            {
                var actorStats = actor.GetActorStats();
                Console.WriteLine($"{actor.FullName}: {actorStats.currentPerformances} текущих ролей, опыт {actor.CalculateExperienceYears()} лет");
            }
        }

        // TODO 2: Найти актера
        public void FindActor()
        {
            Console.Clear();
            Console.WriteLine("=== ПОИСК АКТЕРА ===\n");

            Console.Write("Введите имя актера: ");
            string name = Console.ReadLine();

            var actors = manager.FindAllActorsByName(name);

            if (actors.Count == 0)
            {
                Console.WriteLine("Актер не найден");
            }
            else if (actors.Count == 1)
            {
                actors[0].ShowActorInfo();
            }
            else
            {
                Console.WriteLine("\nНайдено несколько актеров:");
                for (int i = 0; i < actors.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {actors[i].FullName} - {actors[i].RoleType}, {actors[i].CalculateAge()} лет");
                }

                Console.Write("\nВыберите номер для просмотра детальной информации (Enter - пропустить): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= actors.Count)
                {
                    actors[choice - 1].ShowActorInfo();
                }
            }
        }

        // TODO 3: Показать ближайшие спектакли
        public void ShowUpcomingPerformances()
        {
            Console.Clear();
            Console.WriteLine("=== БЛИЖАЙШИЕ СПЕКТАКЛИ (7 дней) ===\n");

            var upcoming = manager.GetUpcomingPerformances(7);

            if (upcoming.Count == 0)
            {
                Console.WriteLine("Нет спектаклей в ближайшие 7 дней.");
                return;
            }

            foreach (var performance in upcoming)
            {
                Console.WriteLine($"\n{performance.Title} - {performance.Genre}, {performance.DurationMinutes} мин., {performance.AgeRating}");

                var upcomingShows = performance.GetUpcomingShows()
                    .Where(s => s.Date <= DateTime.Now.AddDays(7))
                    .OrderBy(s => s.Date)
                    .ToList();

                foreach (var show in upcomingShows)
                {
                    Hall hall = manager.GetHallByNumber(show.HallNumber);
                    string hallName = hall?.Name ?? $"Зал {show.HallNumber}";

                    Console.WriteLine($"  - {show.Date:dd.MM.yyyy HH:mm}, {hallName}" +
                                     $"{(show.IsPremiere ? " ★ПРЕМЬЕРА★" : "")}");

                    if (hall != null)
                    {
                        int available = hall.GetAvailableSeats(show.Date);
                        int total = hall.Capacity;
                        Console.WriteLine($"    Доступно мест: {available} из {total}");
                    }
                }
            }
        }

        // Дополнительная функция: Добавление нового спектакля
        public void AddNewPerformance()
        {
            Console.Clear();
            Console.WriteLine("=== ДОБАВЛЕНИЕ НОВОГО СПЕКТАКЛЯ ===\n");

            Console.Write("ID спектакля: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный ID");
                return;
            }

            if (manager.GetPerformanceById(id) != null)
            {
                Console.WriteLine("Спектакль с таким ID уже существует");
                return;
            }

            Console.Write("Название: ");
            string title = Console.ReadLine();

            Console.Write("Режиссер: ");
            string director = Console.ReadLine();

            Console.Write("Описание: ");
            string description = Console.ReadLine();

            Console.Write("Базовая цена билета: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Неверная цена");
                return;
            }

            Console.Write("Жанр (драма/комедия/мюзикл/балет): ");
            string genre = Console.ReadLine();

            Console.Write("Длительность (минут): ");
            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                Console.WriteLine("Неверная длительность");
                return;
            }

            Console.Write("Возрастное ограничение (0+/6+/12+/16+/18+): ");
            string ageRating = Console.ReadLine();

            Performance newPerformance = new Performance(id, title, director, description,
                                                       price, genre, duration, ageRating);

            manager.AddPerformance(newPerformance);

            Console.Write("Добавить показ? (д/н): ");
            if (Console.ReadLine()?.ToLower() == "д")
            {
                Console.Write("Дата и время (ГГГГ-ММ-ДД ЧЧ:ММ): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime showDate))
                {
                    Console.Write("Номер зала (1-5): ");
                    if (int.TryParse(Console.ReadLine(), out int hallNum))
                    {
                        Console.Write("Премьера? (д/н): ");
                        bool isPremiere = Console.ReadLine()?.ToLower() == "д";

                        newPerformance.AddShow(showDate, hallNum, isPremiere);
                    }
                }
            }
        }

        // Готовый метод - главное меню
        public void ShowMainMenu()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════╗");
                Console.WriteLine("║        ТЕАТР 'МЕЛОДИЯ'            ║");
                Console.WriteLine("╚════════════════════════════════════╝\n");
                Console.WriteLine("1. Репертуар театра");
                Console.WriteLine("2. Информация о спектакле");
                Console.WriteLine("3. Купить билет");
                Console.WriteLine("4. Вернуть билет");
                Console.WriteLine("5. Труппа театра");
                Console.WriteLine("6. Залы театра");
                Console.WriteLine("7. Бронирование места");
                Console.WriteLine("8. Отчет театра");
                Console.WriteLine("9. Найти актера");
                Console.WriteLine("10. Ближайшие спектакли");
                Console.WriteLine("11. Добавить спектакль (админ)");
                Console.WriteLine("12. Выход");
                Console.Write("\nВыберите: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowRepertoire();
                        break;
                    case "2":
                        ShowPerformanceDetails();
                        break;
                    case "3":
                        BuyTicket();
                        break;
                    case "4":
                        ReturnTicket();
                        break;
                    case "5":
                        ShowActors();
                        break;
                    case "6":
                        ShowHallsInfo();
                        break;
                    case "7":
                        ReserveSeat();
                        break;
                    case "8":
                        ShowTheaterReport();
                        break;
                    case "9":
                        FindActor();
                        break;
                    case "10":
                        ShowUpcomingPerformances();
                        break;
                    case "11":
                        AddNewPerformance();
                        break;
                    case "12":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nНажмите Enter...");
                    Console.ReadLine();
                }
            }
        }
    }
}