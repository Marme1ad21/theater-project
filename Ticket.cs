// TODO:
// 1. Добавлены поля для информации о билете (место, сектор, ряд)
// 2. Реализована проверка корректности данных (место существует)
// 3. Реализован метод расчета итоговой цены билета

using System;

namespace Theater
{
    public class Ticket
    {
        public int Id { get; set; }               // Номер билета
        public Performance Performance { get; set; } // Спектакль
        public Performance.Show Show { get; set; }            // Конкретный показ
        public decimal Price { get; set; }        // Цена
        public DateTime PurchaseDate { get; set; } // Дата покупки

        // TODO 1: Добавлены свойства
        public string Sector { get; set; }        // Сектор: партер, бельэтаж, балкон, ложи
        public int Row { get; set; }              // Ряд: 1-30
        public int Seat { get; set; }             // Место: 1-50
        public bool IsUsed { get; set; }          // Использован ли билет

        public Ticket(int id, Performance performance, Performance.Show show, string sector, int row, int seat)
        {
            Id = id;
            Performance = performance;
            Show = show;
            PurchaseDate = DateTime.Now;
            IsUsed = false;

            // TODO 2: Проверить что ряд и место в допустимых пределах
            // Ряд: 1-30, место: 1-50
            if (row < 1)
                Row = 1;
            else if (row > 30)
                Row = 30;
            else
                Row = row;

            if (seat < 1)
                Seat = 1;
            else if (seat > 50)
                Seat = 50;
            else
                Seat = seat;

            // TODO 1: Сохранить сектор, ряд и место
            Sector = sector;

            // TODO 3: Рассчитать цену на основе базовой цены и сектора
            Price = CalculateFinalPrice(performance.BaseTicketPrice, sector, show);
            Price = Math.Round(Price, 2);
        }

        // TODO 3: Рассчитать итоговую цену
        private decimal CalculateFinalPrice(decimal basePrice, string sector, Performance.Show show)
        {
            decimal finalPrice = basePrice;

            // Коэффициенты для секторов:
            // - Партер: 1.5
            // - Бельэтаж: 1.2
            // - Балкон: 0.8
            // - Ложи: 2.0
            // - Амфитеатр: 1.0

            switch (sector.ToLower())
            {
                case "партер":
                    finalPrice *= 1.5m;
                    break;
                case "бельэтаж":
                    finalPrice *= 1.2m;
                    break;
                case "балкон":
                    finalPrice *= 0.8m;
                    break;
                case "ложи":
                    finalPrice *= 2.0m;
                    break;
                case "амфитеатр":
                    finalPrice *= 1.0m;
                    break;
                default:
                    finalPrice *= 1.0m;
                    break;
            }

            // Для премьер +30%
            if (show.IsPremiere)
                finalPrice *= 1.3m;

            // Для вечерних показов (после 18:00) +25%
            if (show.Date.Hour >= 18)
                finalPrice *= 1.25m;

            // Для утренних показов (до 12:00) -20%
            if (show.Date.Hour < 12)
                finalPrice *= 0.8m;

            return finalPrice;
        }

        // TODO 1: Получить информацию о месте
        public string GetSeatInfo()
        {
            return $"Сектор: {Sector}, Ряд: {Row}, Место: {Seat}";
        }

        // TODO 2: Проверить действительность билета
        public bool IsValid()
        {
            // Проверить что дата показа в будущем
            if (Show.Date <= DateTime.Now)
                return false;

            // Проверить что билет не был использован
            if (IsUsed)
                return false;

            return true;
        }

        // TODO 1: Отметить билет как использованный
        public void MarkAsUsed()
        {
            IsUsed = true;
        }

        public void ShowTicketInfo()
        {
            Console.WriteLine($"=== БИЛЕТ №{Id} ===");
            Console.WriteLine($"Спектакль: {Performance.Title}");
            Console.WriteLine($"Дата и время: {Show.Date:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Зал: {Show.HallNumber}");
            Console.WriteLine($"Место: {GetSeatInfo()}");
            Console.WriteLine($"Цена: {Price} руб.");
            Console.WriteLine($"Дата покупки: {PurchaseDate:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Статус: {(IsValid() ? "ДЕЙСТВИТЕЛЕН" : "НЕДЕЙСТВИТЕЛЕН")}");

            if (Show.IsPremiere)
                Console.WriteLine("★ ПРЕМЬЕРНЫЙ ПОКАЗ ★");
        }
    }
}