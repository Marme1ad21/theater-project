// TODO:
// 1. Добавлены поля для информации об актере (амплуа, опыт, роли)
// 2. Реализован учет участия в спектаклях
// 3. Реализован расчет загруженности актера

using System;
using System.Collections.Generic;
using System.Linq;

namespace Theater
{
    public class Actor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime HireDate { get; set; }

        // TODO 1: Добавлены свойства
        public string RoleType { get; set; }       // Амплуа: трагик, комик, характерный, лирический
        public string SpecialSkills { get; set; }  // Особые навыки: вокал, танец, фехтование

        private List<ActorRole> roles = new List<ActorRole>();

        public class ActorRole
        {
            public Performance Performance { get; set; } // Спектакль
            public string RoleName { get; set; }         // Название роли
            public bool IsMainRole { get; set; }         // Главная ли роль
            public DateTime StartDate { get; set; }      // Дата начала исполнения

            public override string ToString()
            {
                return $"{RoleName} в '{Performance.Title}' {(IsMainRole ? "(главная)" : "")}";
            }
        }

        public Actor(int id, string name, DateTime birthDate, DateTime hireDate,
                    string roleType, string skills)
        {
            Id = id;
            FullName = name;
            BirthDate = birthDate;
            HireDate = hireDate;

            // TODO 1: Сохранить амплуа и навыки
            RoleType = roleType;
            SpecialSkills = skills;
        }

        // TODO 2: Добавить роль актеру
        public void AddRole(Performance performance, string roleName, bool isMainRole = false)
        {
            // Проверка на дубликат роли
            foreach (var role in roles)
            {
                if (role.Performance.Id == performance.Id && role.RoleName == roleName)
                {
                    Console.WriteLine($"Ошибка: роль '{roleName}' в спектакле '{performance.Title}' уже назначена актеру {FullName}.");
                    return;
                }
            }

            // Создать новый ActorRole
            ActorRole newRole = new ActorRole
            {
                Performance = performance,
                RoleName = roleName,
                IsMainRole = isMainRole,
                StartDate = DateTime.Now
            };

            // Добавить в список roles
            roles.Add(newRole);
            Console.WriteLine($"Актеру {FullName} добавлена роль '{roleName}' в спектакле '{performance.Title}'.");
        }

        // TODO 3: Рассчитать опыт работы
        public int CalculateExperienceYears()
        {
            DateTime today = DateTime.Now;
            int years = today.Year - HireDate.Year;

            if (HireDate.Date > today.AddYears(-years))
                years--;

            return Math.Max(0, years);
        }

        // TODO 3: Получить текущие роли актера
        public List<ActorRole> GetCurrentRoles()
        {
            List<ActorRole> currentRoles = new List<ActorRole>();

            // Вернуть роли, где спектакль еще идет (есть будущие показы)
            foreach (var role in roles)
            {
                var upcomingShows = role.Performance.GetUpcomingShows();
                if (upcomingShows.Count > 0)
                {
                    currentRoles.Add(role);
                }
            }

            return currentRoles;
        }

        // TODO 2: Проверить доступность актера на дату
        public bool IsAvailable(DateTime date)
        {
            // Проверить что у актера нет спектакля на эту дату
            // Учесть репетиции (занят за 3 часа до спектакля)

            DateTime startBuffer = date.AddHours(-3);
            DateTime endBuffer = date.AddHours(3);

            foreach (var role in roles)
            {
                var shows = role.Performance.GetAllShows();
                foreach (var show in shows)
                {
                    if (show.Date >= startBuffer && show.Date <= endBuffer)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // TODO 1: Рассчитать возраст
        public int CalculateAge()
        {
            DateTime today = DateTime.Now;
            int age = today.Year - BirthDate.Year;

            if (BirthDate.Date > today.AddYears(-age))
                age--;

            return age;
        }

        // TODO 3: Получить статистику актера
        public (int totalRoles, int mainRoles, int currentPerformances) GetActorStats()
        {
            int total = roles.Count;
            int main = 0;
            int current = GetCurrentRoles().Count;

            // Посчитать главные роли
            foreach (var role in roles)
            {
                if (role.IsMainRole)
                    main++;
            }

            return (total, main, current);
        }

        public void ShowActorInfo()
        {
            Console.WriteLine($"=== Актер: {FullName} ===");
            Console.WriteLine($"Дата рождения: {BirthDate:dd.MM.yyyy}");
            Console.WriteLine($"Возраст: {CalculateAge()} лет");
            Console.WriteLine($"Дата приема: {HireDate:dd.MM.yyyy}");
            Console.WriteLine($"Опыт работы: {CalculateExperienceYears()} лет");

            // TODO 1: Вывести амплуа и навыки
            Console.WriteLine($"Амплуа: {RoleType}");
            Console.WriteLine($"Особые навыки: {SpecialSkills}");

            var stats = GetActorStats();
            Console.WriteLine($"\nСтатистика:");
            Console.WriteLine($"  Всего ролей: {stats.totalRoles}");
            Console.WriteLine($"  Главных ролей: {stats.mainRoles}");
            Console.WriteLine($"  Текущих спектаклей: {stats.currentPerformances}");

            var currentRoles = GetCurrentRoles();
            if (currentRoles.Count > 0)
            {
                Console.WriteLine("\nТекущие роли:");
                foreach (var role in currentRoles)
                {
                    Console.WriteLine($"  - {role.Performance.Title}: {role.RoleName} " +
                                     $"{(role.IsMainRole ? "(главная)" : "")}");

                    var nextShow = role.Performance.GetNextShow();
                    if (nextShow != null)
                    {
                        Console.WriteLine($"    Ближайший показ: {nextShow.Date:dd.MM.yyyy HH:mm}");
                    }
                }
            }

            if (roles.Count > currentRoles.Count)
            {
                Console.WriteLine($"\nПрошлые роли: {roles.Count - currentRoles.Count}");
            }
        }
    }
}