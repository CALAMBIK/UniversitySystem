using UniversitySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace UniversitySystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(UniversityContext context)
        {
            try
            {
                Console.WriteLine("Проверяем существование базы...");

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Console.WriteLine("База создана. Начинаем заполнение...");

                // ========== Кафедры ==========
                var departaments = new Departament[]
                {
                    new Departament { Name = "Информатика и ВТ" },
                    new Departament { Name = "Прикладная математика" },
                    new Departament { Name = "Общая физика" },
                    new Departament { Name = "Высшая математика" },
                    new Departament { Name = "Экономика" },
                    new Departament { Name = "Социология" },
                    new Departament { Name = "Иностранные языки" }
                };
                context.Departaments.AddRange(departaments);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено кафедр: {departaments.Length}");

                // ========== Группы ==========
                var groups = new StudentGroup[]
                {
                    new StudentGroup { IdDepartament = 1, NumberGroup = 101 },
                    new StudentGroup { IdDepartament = 1, NumberGroup = 102 },
                    new StudentGroup { IdDepartament = 2, NumberGroup = 201 },
                    new StudentGroup { IdDepartament = 2, NumberGroup = 202 },
                    new StudentGroup { IdDepartament = 3, NumberGroup = 301 },
                    new StudentGroup { IdDepartament = 3, NumberGroup = 302 },
                    new StudentGroup { IdDepartament = 4, NumberGroup = 401 },
                    new StudentGroup { IdDepartament = 5, NumberGroup = 501 },
                    new StudentGroup { IdDepartament = 6, NumberGroup = 601 },
                    new StudentGroup { IdDepartament = 7, NumberGroup = 701 }
                };
                context.StudentGroups.AddRange(groups);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено групп: {groups.Length}");

                // ========== Генерация студентов ==========
                var students = new List<Student>();
                string[] firstNames = { "Иван", "Алексей", "Мария", "Анна", "Дмитрий", "Екатерина", "Никита", "Светлана", "Андрей", "Ольга", "Виктор", "Татьяна", "Сергей", "Юлия", "Константин", "Надежда", "Михаил", "Людмила", "Анатолий", "Ирина", "Владимир", "Ксения", "Роман", "Елена", "Павел", "Марина", "Григорий", "Софья", "Евгений", "Алёна", "Денис", "Вероника", "Илья", "Оксана", "Артем", "Дарья", "Федор", "Валерия", "Александр", "Полина", "Вадим", "Наталья", "Тимур", "Лариса", "Даниил", "Анжелика", "Вячеслав", "Маргарита", "Станислав" };
                string[] lastNames = { "Иванов", "Петров", "Смирнов", "Кузнецова", "Соколов", "Лебедева", "Попов", "Орлова", "Морозов", "Федоров", "Васильева", "Гусев", "Жукова", "Кириллов", "Логинова", "Данилов", "Власова", "Кузьмин", "Семенова", "Фролов" };
                string[] patronymics = { "Александрович", "Сергеевич", "Игоревич", "Владимирович", "Павлович", "Михайлович", "Николаевич", "Анатольевич", "Федорович", "Дмитриевич", "Александровна", "Сергеевна", "Игоревна", "Владимировна", "Павловна", "Михайловна", "Николаевна", "Анатольевна", "Федоровна", "Дмитриевна" };

                Random rand = new Random();
                for (int i = 1; i <= 50; i++)
                {
                    students.Add(new Student
                    {
                        IdGroup = (i % 10) + 1,
                        Name = firstNames[rand.Next(firstNames.Length)],
                        SecondName = lastNames[rand.Next(lastNames.Length)],
                        Patronymic = patronymics[rand.Next(patronymics.Length)],
                        PhoneNumber = $"+7999{rand.Next(10000000, 99999999)}",
                        DateBirthday = new DateTime(2000 + rand.Next(0, 4), rand.Next(1, 13), rand.Next(1, 28)),
                        Login = $"student{i}",
                        Password = "123"
                    });
                }
                context.Students.AddRange(students);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено студентов: {students.Count}");

                // ========== Преподаватели ==========
                var teachers = new List<Teacher>
                {
                    new Teacher { IdDepartament = 1, Name = "Алексей", SecondName = "Смирнов", Patronymic = "Владимирович", PhoneNumber = "+79992220001", Login = "teacher1", Password = "123" },
                    new Teacher { IdDepartament = 2, Name = "Ольга", SecondName = "Кузнецова", Patronymic = "Павловна", PhoneNumber = "+79992220002", Login = "teacher2", Password = "123" },
                    new Teacher { IdDepartament = 3, Name = "Николай", SecondName = "Морозов", Patronymic = "Сергеевич", PhoneNumber = "+79992220003", Login = "teacher3", Password = "123" },
                    new Teacher { IdDepartament = 4, Name = "Татьяна", SecondName = "Орлова", Patronymic = "Александровна", PhoneNumber = "+79992220004", Login = "teacher4", Password = "123" },
                    new Teacher { IdDepartament = 5, Name = "Сергей", SecondName = "Федоров", Patronymic = "Игоревич", PhoneNumber = "+79992220005", Login = "teacher5", Password = "123" },
                    new Teacher { IdDepartament = 6, Name = "Елена", SecondName = "Васильева", Patronymic = "Михайловна", PhoneNumber = "+79992220006", Login = "teacher6", Password = "123" },
                    new Teacher { IdDepartament = 7, Name = "Владимир", SecondName = "Гусев", Patronymic = "Александрович", PhoneNumber = "+79992220007", Login = "teacher7", Password = "123" },
                    new Teacher { IdDepartament = 1, Name = "Ирина", SecondName = "Жукова", Patronymic = "Сергеевна", PhoneNumber = "+79992220008", Login = "teacher8", Password = "123" },
                    new Teacher { IdDepartament = 2, Name = "Павел", SecondName = "Кириллов", Patronymic = "Владимирович", PhoneNumber = "+79992220009", Login = "teacher9", Password = "123" },
                    new Teacher { IdDepartament = 3, Name = "Марина", SecondName = "Логинова", Patronymic = "Александровна", PhoneNumber = "+79992220010", Login = "teacher10", Password = "123" }
                };
                context.Teachers.AddRange(teachers);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено преподавателей: {teachers.Count}");

                // ========== Дисциплины ==========
                var disciplines = new Discipline[]
                {
                    new Discipline { Name = "Программирование" },
                    new Discipline { Name = "Базы данных" },
                    new Discipline { Name = "Математика" },
                    new Discipline { Name = "Физика" },
                    new Discipline { Name = "Экономика предприятия" },
                    new Discipline { Name = "Социология труда" },
                    new Discipline { Name = "Английский язык" },
                    new Discipline { Name = "Французский язык" },
                    new Discipline { Name = "Химия" },
                    new Discipline { Name = "История" }
                };
                context.Disciplines.AddRange(disciplines);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено дисциплин: {disciplines.Length}");

                // ========== Пользователи ==========
                var users = new List<User> { new User { Login = "admin", Password = "123", Role = "Admin" } };

                for (int i = 1; i <= students.Count; i++)
                {
                    users.Add(new User
                    {
                        Login = $"student{i}",
                        Password = "123",
                        Role = "Student",
                        IdStudent = i
                    });
                }

                for (int i = 1; i <= teachers.Count; i++)
                {
                    users.Add(new User
                    {
                        Login = $"teacher{i}",
                        Password = "123",
                        Role = "Teacher",
                        IdTeacher = i
                    });
                }

                context.Users.AddRange(users);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено пользователей: {users.Count}");

                // ========== Новости ==========
                var news = new List<News>
                {
                    new News { Title = "Открытие нового компьютерного класса", Content = "В нашем вузе открылся новый современный компьютерный класс.", Author = "Администрация", PublishDate = DateTime.Now.AddDays(-2), IsPublished = true },
                    new News { Title = "Студенческая олимпиада по программированию", Content = "Приглашаем всех студентов принять участие в олимпиаде по программированию.", Author = "Кафедра информатики", PublishDate = DateTime.Now.AddDays(-5), IsPublished = true },
                    new News { Title = "Обновление библиотечного фонда", Content = "Библиотека пополнилась новыми учебниками и научными изданиями.", Author = "Библиотека", PublishDate = DateTime.Now.AddDays(-7), IsPublished = true }
                };
                context.News.AddRange(news);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено новостей: {news.Count}");

                // ========== Акции ==========
                var promotions = new List<Promotion>
                {
                    new Promotion { Title = "Скидка на общежитие", Description = "Скидка 50% на проживание для отличников.", Discount = "50%", StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(25), IsActive = true },
                    new Promotion { Title = "Бесплатные курсы", Description = "Бесплатное повышение квалификации для преподавателей.", Discount = "Бесплатно", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(2), IsActive = true }
                };
                context.Promotions.AddRange(promotions);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено акций: {promotions.Count}");

                Console.WriteLine("🎉 База данных успешно инициализирована!");
                Console.WriteLine($"📊 ИТОГО: {departaments.Length} кафедр, {groups.Length} групп, {students.Count} студентов, {teachers.Count} преподавателей, {disciplines.Length} дисциплин, {users.Count} пользователей, {news.Count} новостей, {promotions.Count} акций");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при инициализации: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
    }
}
