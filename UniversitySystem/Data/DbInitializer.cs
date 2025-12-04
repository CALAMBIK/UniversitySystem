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

                // 1. Создаем кафедры
                var departaments = new Departament[]
                {
                    new Departament { Name = "Информатика и вычислительная техника" },
                    new Departament { Name = "Прикладная математика" },
                    new Departament { Name = "Общая физика" },
                    new Departament { Name = "Высшая математика" },
                    new Departament { Name = "Экономика" },
                    new Departament { Name = "Социология" },
                    new Departament { Name = "Иностранные языки" },
                    new Departament { Name = "Химия" },
                    new Departament { Name = "История" },
                    new Departament { Name = "Философия" }
                };
                context.Departaments.AddRange(departaments);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено кафедр: {departaments.Length}");

                // 2. Создаем учебные группы
                var groups = new StudentGroup[]
                {
                    // Кафедра Информатика и ВТ
                    new StudentGroup { IdDepartament = 1, NumberGroup = 101, Departament = departaments[0] },
                    new StudentGroup { IdDepartament = 1, NumberGroup = 102, Departament = departaments[0] },
                    new StudentGroup { IdDepartament = 1, NumberGroup = 103, Departament = departaments[0] },
                    
                    // Кафедра Прикладная математика
                    new StudentGroup { IdDepartament = 2, NumberGroup = 201, Departament = departaments[1] },
                    new StudentGroup { IdDepartament = 2, NumberGroup = 202, Departament = departaments[1] },
                    
                    // Кафедра Общая физика
                    new StudentGroup { IdDepartament = 3, NumberGroup = 301, Departament = departaments[2] },
                    new StudentGroup { IdDepartament = 3, NumberGroup = 302, Departament = departaments[2] },
                    
                    // Кафедра Высшая математика
                    new StudentGroup { IdDepartament = 4, NumberGroup = 401, Departament = departaments[3] },
                    
                    // Кафедра Экономика
                    new StudentGroup { IdDepartament = 5, NumberGroup = 501, Departament = departaments[4] },
                    new StudentGroup { IdDepartament = 5, NumberGroup = 502, Departament = departaments[4] },
                    
                    // Кафедра Социология
                    new StudentGroup { IdDepartament = 6, NumberGroup = 601, Departament = departaments[5] },
                    
                    // Кафедра Иностранные языки
                    new StudentGroup { IdDepartament = 7, NumberGroup = 701, Departament = departaments[6] },
                    
                    // Кафедра Химия
                    new StudentGroup { IdDepartament = 8, NumberGroup = 801, Departament = departaments[7] },
                    
                    // Кафедра История
                    new StudentGroup { IdDepartament = 9, NumberGroup = 901, Departament = departaments[8] },
                    
                    // Кафедра Философия
                    new StudentGroup { IdDepartament = 10, NumberGroup = 1001, Departament = departaments[9] }
                };
                context.StudentGroups.AddRange(groups);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено групп: {groups.Length}");

                // 3. Создаем дисциплины
                var disciplines = new Discipline[]
                {
                    new Discipline { Name = "Программирование на C#" },
                    new Discipline { Name = "Базы данных" },
                    new Discipline { Name = "Веб-разработка" },
                    new Discipline { Name = "Алгоритмы и структуры данных" },
                    new Discipline { Name = "Математический анализ" },
                    new Discipline { Name = "Линейная алгебра" },
                    new Discipline { Name = "Общая физика" },
                    new Discipline { Name = "Квантовая физика" },
                    new Discipline { Name = "Экономика предприятия" },
                    new Discipline { Name = "Микроэкономика" },
                    new Discipline { Name = "Социология труда" },
                    new Discipline { Name = "Английский язык" },
                    new Discipline { Name = "Немецкий язык" },
                    new Discipline { Name = "Органическая химия" },
                    new Discipline { Name = "История России" },
                    new Discipline { Name = "Философия науки" },
                    new Discipline { Name = "Операционные системы" },
                    new Discipline { Name = "Сети и телекоммуникации" },
                    new Discipline { Name = "Искусственный интеллект" },
                    new Discipline { Name = "Кибербезопасность" }
                };
                context.Disciplines.AddRange(disciplines);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено дисциплин: {disciplines.Length}");

                // 4. Создаем студентов
                var students = new List<Student>();
                string[] firstNames = { "Иван", "Алексей", "Мария", "Анна", "Дмитрий", "Екатерина", "Никита", "Светлана", "Андрей", "Ольга", "Виктор", "Татьяна", "Сергей", "Юлия", "Константин", "Надежда", "Михаил", "Людмила", "Анатолий", "Ирина", "Владимир", "Ксения", "Роман", "Елена", "Павел", "Марина", "Григорий", "Софья", "Евгений", "Алёна", "Денис", "Вероника", "Илья", "Оксана", "Артем", "Дарья", "Федор", "Валерия", "Александр", "Полина", "Вадим", "Наталья", "Тимур", "Лариса", "Даниил", "Анжелика", "Вячеслав", "Маргарита", "Станислав" };
                string[] lastNames = { "Иванов", "Петров", "Смирнов", "Кузнецова", "Соколов", "Лебедева", "Попов", "Орлова", "Морозов", "Федоров", "Васильева", "Гусев", "Жукова", "Кириллов", "Логинова", "Данилов", "Власова", "Кузьмин", "Семенова", "Фролов", "Захаров", "Романова", "Степанова", "Григорьева", "Тихонова", "Алексеева", "Сергеева", "Фомина", "Денисова", "Максимова", "Антонова", "Тимофеева", "Николаева", "Белова", "Калинина", "Афанасьева", "Воронова", "Ковалева", "Зайцева", "Павлова" };
                string[] patronymics = { "Александрович", "Сергеевич", "Игоревич", "Владимирович", "Павлович", "Михайлович", "Николаевич", "Анатольевич", "Федорович", "Дмитриевич", "Александровна", "Сергеевна", "Игоревна", "Владимировна", "Павловна", "Михайловна", "Николаевна", "Анатольевна", "Федоровна", "Дмитриевна" };

                Random rand = new Random();
                int studentCounter = 1;

                // Распределяем студентов по группам
                foreach (var group in groups)
                {
                    // В каждой группе будет 5-8 студентов
                    int studentsInGroup = rand.Next(5, 9);

                    for (int i = 0; i < studentsInGroup; i++)
                    {
                        string firstName = firstNames[rand.Next(firstNames.Length)];
                        string lastName = lastNames[rand.Next(lastNames.Length)];
                        string patronymic = patronymics[rand.Next(patronymics.Length)];

                        // Для падежей выбираем соответствующие отчества
                        if (firstName.EndsWith("а") || firstName.EndsWith("я"))
                        {
                            patronymic = patronymics[rand.Next(10, patronymics.Length)];
                        }
                        else
                        {
                            patronymic = patronymics[rand.Next(0, 10)];
                        }

                        students.Add(new Student
                        {
                            IdGroup = group.IdGroup,
                            Name = firstName,
                            SecondName = lastName,
                            Patronymic = patronymic,
                            PhoneNumber = $"+7(999){rand.Next(100, 1000)}-{rand.Next(10, 100)}-{rand.Next(10, 100)}",
                            DateBirthday = new DateTime(rand.Next(1998, 2003), rand.Next(1, 13), rand.Next(1, 28)),
                            Login = $"student{studentCounter}",
                            Password = "123",
                            Group = group
                        });
                        studentCounter++;
                    }
                }

                context.Students.AddRange(students);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено студентов: {students.Count}");

                // 5. Создаем преподавателей
                var teachers = new List<Teacher>
                {
                    // Кафедра Информатика и ВТ
                    new Teacher {
                        IdDepartament = 1,
                        Name = "Алексей",
                        SecondName = "Смирнов",
                        Patronymic = "Владимирович",
                        PhoneNumber = "+7(999)111-22-33",
                        Login = "teacher1",
                        Password = "123",
                        Departament = departaments[0]
                    },
                    new Teacher {
                        IdDepartament = 1,
                        Name = "Ирина",
                        SecondName = "Жукова",
                        Patronymic = "Сергеевна",
                        PhoneNumber = "+7(999)222-33-44",
                        Login = "teacher2",
                        Password = "123",
                        Departament = departaments[0]
                    },
                    
                    // Кафедра Прикладная математика
                    new Teacher {
                        IdDepartament = 2,
                        Name = "Ольга",
                        SecondName = "Кузнецова",
                        Patronymic = "Павловна",
                        PhoneNumber = "+7(999)333-44-55",
                        Login = "teacher3",
                        Password = "123",
                        Departament = departaments[1]
                    },
                    new Teacher {
                        IdDepartament = 2,
                        Name = "Павел",
                        SecondName = "Кириллов",
                        Patronymic = "Владимирович",
                        PhoneNumber = "+7(999)444-55-66",
                        Login = "teacher4",
                        Password = "123",
                        Departament = departaments[1]
                    },
                    
                    // Кафедра Общая физика
                    new Teacher {
                        IdDepartament = 3,
                        Name = "Николай",
                        SecondName = "Морозов",
                        Patronymic = "Сергеевич",
                        PhoneNumber = "+7(999)555-66-77",
                        Login = "teacher5",
                        Password = "123",
                        Departament = departaments[2]
                    },
                    
                    // Кафедра Высшая математика
                    new Teacher {
                        IdDepartament = 4,
                        Name = "Татьяна",
                        SecondName = "Орлова",
                        Patronymic = "Александровна",
                        PhoneNumber = "+7(999)666-77-88",
                        Login = "teacher6",
                        Password = "123",
                        Departament = departaments[3]
                    },
                    
                    // Кафедра Экономика
                    new Teacher {
                        IdDepartament = 5,
                        Name = "Сергей",
                        SecondName = "Федоров",
                        Patronymic = "Игоревич",
                        PhoneNumber = "+7(999)777-88-99",
                        Login = "teacher7",
                        Password = "123",
                        Departament = departaments[4]
                    },
                    
                    // Кафедра Социология
                    new Teacher {
                        IdDepartament = 6,
                        Name = "Елена",
                        SecondName = "Васильева",
                        Patronymic = "Михайловна",
                        PhoneNumber = "+7(999)888-99-00",
                        Login = "teacher8",
                        Password = "123",
                        Departament = departaments[5]
                    },
                    
                    // Кафедра Иностранные языки
                    new Teacher {
                        IdDepartament = 7,
                        Name = "Владимир",
                        SecondName = "Гусев",
                        Patronymic = "Александрович",
                        PhoneNumber = "+7(999)999-00-11",
                        Login = "teacher9",
                        Password = "123",
                        Departament = departaments[6]
                    },
                    
                    // Кафедра Химия
                    new Teacher {
                        IdDepartament = 8,
                        Name = "Марина",
                        SecondName = "Логинова",
                        Patronymic = "Александровна",
                        PhoneNumber = "+7(999)000-11-22",
                        Login = "teacher10",
                        Password = "123",
                        Departament = departaments[7]
                    }
                };
                context.Teachers.AddRange(teachers);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено преподавателей: {teachers.Count}");

                // 6. Создаем пользователей
                var users = new List<User>();

                // Администратор
                users.Add(new User
                {
                    Login = "admin",
                    Password = "123",
                    Role = "Admin",
                    CreatedDate = DateTime.Now,
                    LastLogin = DateTime.Now
                });

                // Пользователи для студентов
                for (int i = 0; i < students.Count; i++)
                {
                    users.Add(new User
                    {
                        Login = students[i].Login,
                        Password = "123",
                        Role = "Student",
                        IdStudent = students[i].IdStudent,
                        CreatedDate = DateTime.Now,
                        LastLogin = DateTime.Now
                    });
                }

                // Пользователи для преподавателей
                for (int i = 0; i < teachers.Count; i++)
                {
                    users.Add(new User
                    {
                        Login = teachers[i].Login,
                        Password = "123",
                        Role = "Teacher",
                        IdTeacher = teachers[i].IdTeacher,
                        CreatedDate = DateTime.Now,
                        LastLogin = DateTime.Now
                    });
                }

                context.Users.AddRange(users);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено пользователей: {users.Count}");

                // 7. Создаем профили пользователей
                var userProfiles = new List<UserProfile>();
                foreach (var user in users)
                {
                    userProfiles.Add(new UserProfile
                    {
                        IdUser = user.IdUser,
                        Email = $"{user.Login}@university.ru",
                        Phone = $"+7(999){rand.Next(100, 1000)}-{rand.Next(10, 100)}-{rand.Next(10, 100)}",
                        UpdatedDate = DateTime.Now
                    });
                }
                context.UserProfiles.AddRange(userProfiles);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено профилей: {userProfiles.Count}");

                // 8. Создаем связи преподавателей с дисциплинами
                var teacherDisciplines = new List<TeacherDiscipline>();

                // Преподаватель 1 (Смирнов) ведет дисциплины для групп информатики
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 1, IdDiscipline = 1, IdGroup = 1 }); // Программирование группа 101
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 1, IdDiscipline = 2, IdGroup = 1 }); // Базы данных группа 101
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 1, IdDiscipline = 3, IdGroup = 2 }); // Веб-разработка группа 102

                // Преподаватель 2 (Жукова) ведет дисциплины для групп информатики
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 2, IdDiscipline = 4, IdGroup = 1 }); // Алгоритмы группа 101
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 2, IdDiscipline = 17, IdGroup = 3 }); // ОС группа 103

                // Преподаватель 3 (Кузнецова) ведет математику
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 3, IdDiscipline = 5, IdGroup = 4 }); // Матан группа 201
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 3, IdDiscipline = 6, IdGroup = 5 }); // Линал группа 202

                // Преподаватель 4 (Кириллов) ведет математику
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 4, IdDiscipline = 5, IdGroup = 1 }); // Матан группа 101
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 4, IdDiscipline = 6, IdGroup = 2 }); // Линал группа 102

                // Преподаватель 5 (Морозов) ведет физику
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 5, IdDiscipline = 7, IdGroup = 6 }); // Общая физика группа 301
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 5, IdDiscipline = 8, IdGroup = 7 }); // Квантовая физика группа 302

                // Преподаватель 6 (Орлова) ведет высшую математику
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 6, IdDiscipline = 5, IdGroup = 8 }); // Матан группа 401

                // Преподаватель 7 (Федоров) ведет экономику
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 7, IdDiscipline = 9, IdGroup = 9 }); // Экономика предприятия группа 501
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 7, IdDiscipline = 10, IdGroup = 10 }); // Микроэкономика группа 502

                // Преподаватель 8 (Васильева) ведет социологию
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 8, IdDiscipline = 11, IdGroup = 11 }); // Социология труда группа 601

                // Преподаватель 9 (Гусев) ведет иностранные языки
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 9, IdDiscipline = 12, IdGroup = 12 }); // Английский группа 701
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 9, IdDiscipline = 13, IdGroup = 12 }); // Немецкий группа 701

                // Преподаватель 10 (Логинова) ведет химию
                teacherDisciplines.Add(new TeacherDiscipline { IdTeacher = 10, IdDiscipline = 14, IdGroup = 13 }); // Органическая химия группа 801

                context.TeacherDisciplines.AddRange(teacherDisciplines);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено связей преподаватель-дисциплина: {teacherDisciplines.Count}");

                // 9. Создаем учебные материалы
                var courseMaterials = new List<CourseMaterial>
                {
                    // Материалы для группы 101 (Информатика)
                    new CourseMaterial {
                        IdTeacher = 1,
                        IdGroup = 1,
                        IdDiscipline = 1,
                        Title = "Введение в C#",
                        Description = "Базовые концепции программирования на языке C#. Введение в ООП, типы данных, операторы.",
                        FileUrl = "/materials/csharp_intro.pdf",
                        CreatedDate = DateTime.Now.AddDays(-10),
                        Teacher = teachers[0],
                        Group = groups[0],
                        Discipline = disciplines[0]
                    },
                    new CourseMaterial {
                        IdTeacher = 1,
                        IdGroup = 1,
                        IdDiscipline = 2,
                        Title = "Основы SQL",
                        Description = "Введение в реляционные базы данных. Основные команды SQL: SELECT, INSERT, UPDATE, DELETE.",
                        FileUrl = "/materials/sql_basics.pdf",
                        CreatedDate = DateTime.Now.AddDays(-8),
                        Teacher = teachers[0],
                        Group = groups[0],
                        Discipline = disciplines[1]
                    },
                    new CourseMaterial {
                        IdTeacher = 2,
                        IdGroup = 1,
                        IdDiscipline = 4,
                        Title = "Алгоритмы сортировки",
                        Description = "Изучение основных алгоритмов сортировки: пузырьковая, быстрая, сортировка слиянием.",
                        FileUrl = "/materials/sorting_algorithms.pdf",
                        CreatedDate = DateTime.Now.AddDays(-6),
                        Teacher = teachers[1],
                        Group = groups[0],
                        Discipline = disciplines[3]
                    },
                    
                    // Материалы для группы 102 (Информатика)
                    new CourseMaterial {
                        IdTeacher = 1,
                        IdGroup = 2,
                        IdDiscipline = 3,
                        Title = "HTML и CSS для начинающих",
                        Description = "Основы веб-разработки: структура HTML документа, стилизация с помощью CSS.",
                        FileUrl = "/materials/html_css.pdf",
                        CreatedDate = DateTime.Now.AddDays(-7),
                        Teacher = teachers[0],
                        Group = groups[1],
                        Discipline = disciplines[2]
                    },
                    
                    // Материалы для группы 201 (Математика)
                    new CourseMaterial {
                        IdTeacher = 3,
                        IdGroup = 4,
                        IdDiscipline = 5,
                        Title = "Пределы и производные",
                        Description = "Основные понятия математического анализа: пределы функций, производные, правила дифференцирования.",
                        FileUrl = "/materials/limits_derivatives.pdf",
                        CreatedDate = DateTime.Now.AddDays(-9),
                        Teacher = teachers[2],
                        Group = groups[3],
                        Discipline = disciplines[4]
                    },
                    
                    // Материалы для группы 301 (Физика)
                    new CourseMaterial {
                        IdTeacher = 5,
                        IdGroup = 6,
                        IdDiscipline = 7,
                        Title = "Механика Ньютона",
                        Description = "Законы Ньютона, кинематика, динамика, законы сохранения энергии и импульса.",
                        FileUrl = "/materials/newton_mechanics.pdf",
                        CreatedDate = DateTime.Now.AddDays(-5),
                        Teacher = teachers[4],
                        Group = groups[5],
                        Discipline = disciplines[6]
                    },
                    
                    // Материалы для группы 501 (Экономика)
                    new CourseMaterial {
                        IdTeacher = 7,
                        IdGroup = 9,
                        IdDiscipline = 9,
                        Title = "Основы бухгалтерского учета",
                        Description = "Принципы бухгалтерского учета на предприятии, основные проводки, баланс.",
                        FileUrl = "/materials/accounting_basics.pdf",
                        CreatedDate = DateTime.Now.AddDays(-4),
                        Teacher = teachers[6],
                        Group = groups[8],
                        Discipline = disciplines[8]
                    },
                    
                    // Материалы для группы 701 (Иностранные языки)
                    new CourseMaterial {
                        IdTeacher = 9,
                        IdGroup = 12,
                        IdDiscipline = 12,
                        Title = "Английская грамматика",
                        Description = "Основы английской грамматики: времена, артикли, предлоги, модальные глаголы.",
                        FileUrl = "/materials/english_grammar.pdf",
                        CreatedDate = DateTime.Now.AddDays(-3),
                        Teacher = teachers[8],
                        Group = groups[11],
                        Discipline = disciplines[11]
                    }
                };
                context.CourseMaterials.AddRange(courseMaterials);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено учебных материалов: {courseMaterials.Count}");

                // 10. Создаем новости
                var news = new List<News>
                {
                    new News {
                        Title = "Открытие нового компьютерного класса",
                        Content = "В нашем вузе открылся новый современный компьютерный класс, оборудованный последними моделями компьютеров и профессиональным программным обеспечением для обучения программированию и дизайну.",
                        Author = "Администрация",
                        PublishDate = DateTime.Now.AddDays(-2),
                        IsPublished = true,
                        CreatedDate = DateTime.Now.AddDays(-2)
                    },
                    new News {
                        Title = "Студенческая олимпиада по программированию",
                        Content = "Приглашаем всех студентов принять участие в ежегодной олимпиаде по программированию. Победители получат ценные призы и возможность пройти стажировку в ведущих IT-компаниях.",
                        Author = "Кафедра информатики",
                        PublishDate = DateTime.Now.AddDays(-5),
                        IsPublished = true,
                        CreatedDate = DateTime.Now.AddDays(-5)
                    },
                    new News {
                        Title = "Обновление библиотечного фонда",
                        Content = "Библиотека пополнилась новыми учебниками и научными изданиями по современным технологиям, экономике и гуманитарным наукам. Все книги доступны для студентов и преподавателей.",
                        Author = "Библиотека",
                        PublishDate = DateTime.Now.AddDays(-7),
                        IsPublished = true,
                        CreatedDate = DateTime.Now.AddDays(-7)
                    },
                    new News {
                        Title = "Научная конференция 'Инновации в образовании'",
                        Content = "Приглашаем преподавателей и студентов принять участие в научной конференции, посвященной современным методам обучения и инновационным педагогическим технологиям.",
                        Author = "Научный отдел",
                        PublishDate = DateTime.Now.AddDays(-10),
                        IsPublished = true,
                        CreatedDate = DateTime.Now.AddDays(-10)
                    },
                    new News {
                        Title = "Спортивные достижения наших студентов",
                        Content = "Студенты нашего вуза завоевали 3 золотые медали на региональных соревнованиях по легкой атлетике. Поздравляем победителей и их тренеров!",
                        Author = "Спортивный клуб",
                        PublishDate = DateTime.Now.AddDays(-12),
                        IsPublished = true,
                        CreatedDate = DateTime.Now.AddDays(-12)
                    }
                };
                context.News.AddRange(news);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено новостей: {news.Count}");

                // 11. Создаем акции и спецпредложения
                var promotions = new List<Promotion>
                {
                    new Promotion {
                        Title = "Скидка на общежитие для отличников",
                        Description = "Студенты, имеющие средний балл 4.5 и выше, получают скидку 50% на проживание в общежитии.",
                        Discount = "50%",
                        StartDate = DateTime.Now.AddDays(-5),
                        EndDate = DateTime.Now.AddDays(25),
                        IsActive = true
                    },
                    new Promotion {
                        Title = "Бесплатные курсы повышения квалификации",
                        Description = "Для преподавателей вуза доступны бесплатные курсы повышения квалификации по современным педагогическим технологиям.",
                        Discount = "Бесплатно",
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(2),
                        IsActive = true
                    },
                    new Promotion {
                        Title = "Стипендия для активистов",
                        Description = "Студенты, активно участвующие в научной и общественной жизни вуза, могут претендовать на повышенную стипендию.",
                        Discount = "Повышенная стипендия",
                        StartDate = DateTime.Now.AddDays(-15),
                        EndDate = DateTime.Now.AddDays(45),
                        IsActive = true
                    }
                };
                context.Promotions.AddRange(promotions);
                context.SaveChanges();
                Console.WriteLine($"✅ Добавлено акций: {promotions.Count}");

                Console.WriteLine("=========================================");
                Console.WriteLine("База данных успешно инициализирована!");
                Console.WriteLine("=========================================");
                Console.WriteLine($"ИТОГО:");
                Console.WriteLine($"• Кафедр: {departaments.Length}");
                Console.WriteLine($"• Групп: {groups.Length}");
                Console.WriteLine($"• Студентов: {students.Count}");
                Console.WriteLine($"• Преподавателей: {teachers.Count}");
                Console.WriteLine($"• Дисциплин: {disciplines.Length}");
                Console.WriteLine($"• Пользователей: {users.Count}");
                Console.WriteLine($"• Профилей: {userProfiles.Count}");
                Console.WriteLine($"• Связей преподаватель-дисциплина: {teacherDisciplines.Count}");
                Console.WriteLine($"• Учебных материалов: {courseMaterials.Count}");
                Console.WriteLine($"• Новостей: {news.Count}");
                Console.WriteLine($"• Акций: {promotions.Count}");
                Console.WriteLine("=========================================");
                Console.WriteLine("");
                Console.WriteLine("Тестовые учетные записи:");
                Console.WriteLine("• Администратор: admin / 123");
                Console.WriteLine("• Преподаватель: teacher1 / 123");
                Console.WriteLine("• Студент: student1 / 123");
                Console.WriteLine("=========================================");

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