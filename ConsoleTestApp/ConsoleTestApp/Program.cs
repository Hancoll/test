using ConsoleTestApp.Entities;
using ConsoleTestApp.Entities.Common;
using ConsoleTestApp.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleTestApp;

internal class Program
{
    private static string DbConnectionString = "Data Source=AppDb.db";

    static void Main(string[] args)
    {
        if (args.Length == 0)   
            return;

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite(DbConnectionString);
        var context = new ApplicationDbContext(optionsBuilder.Options);

        if (args[0] == "1")
        {
            context.Database.EnsureCreated();
            return;
        }

        if (!context.Database.CanConnect())
        {
            Console.WriteLine("Таблица не найдена.");
            return;
        }

        if (args[0] == "2")
        {
            try
            {
                if (args.Length != 6)
                    throw new Exception("Не все требуемые аргументы введены.");

                string fullName;
                DateOnly birthDay;
                Gender gender;

                fullName = args[1] + " " + args[2] + " " + args[3];

                if (!DateOnly.TryParse(args[4], out DateOnly result))
                    throw new Exception("Не корректно введена дата.");

                birthDay = result;

                gender = args[5] switch
                {
                    "мужской" => Gender.Male,
                    "женский" => Gender.Female,
                    _ => throw new Exception("Не корректно введен пол.")
                };

                var user = new User() { FullName = fullName, BirthDay = birthDay, Gender = gender };

                context.Users.Add(user);
                context.SaveChanges();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
                

            return;
        }

        if (args[0] == "3")
        {
            var uniqueUsers = context.Users.GroupBy(x => new
            {
                FullName = x.FullName,
                BirthDay = x.BirthDay
            }).Select(x => new
            {
                FullName = x.Key.FullName,
                BirthDay = x.Key.BirthDay,
                Count = x.Count()
            });

            var users = context.Users.Where(x => uniqueUsers.Any(y =>
            y.Count == 1 &&
            y.FullName == x.FullName &&
            y.BirthDay == x.BirthDay)).ToList();

            foreach (var user in users)
            {
                var today = DateTime.Today;
                var age = today.Year - user.BirthDay.Year;

                if (user.BirthDay.ToDateTime(new TimeOnly()) > today.AddYears(-age))
                    age--;

                Console.WriteLine(
                    user.FullName + " | " + 
                    user.BirthDay + " | " + 
                    user.Gender + " | " +
                    age);
            }

            return;
        }

        if (args[0] == "4")
        {
            var birhtDay = new DateOnly(2000, 1, 12);

            for (int i = 0; i < 1000000; i++)
            {
                var user = new User();

                var pool = "ЯЧСМИТБЮФЫВАПРОЛДЖЦУКЕНГШЩЗХ";
                var randomLetter = pool[new Random().Next(pool.Length)].ToString();

                user.FullName = randomLetter + "ахов Глеб Васильевич";
                user.Gender = (Gender)new Random().Next(2);
                user.BirthDay = birhtDay;

                context.Users.Add(user);
            }

            for (int i = 0; i < 100; i++)
            {
                var user = new User();

                user.FullName = "Fахов Глеб Васильевич";
                user.Gender = Gender.Male;
                user.BirthDay = birhtDay;

                context.Users.Add(user);
            }

            context.SaveChanges();

            return;
        }

        if (args[0] == "5")
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var users = context.Users.Where(x => EF.Functions.Like(x.FullName, "F%") && x.Gender == Gender.Male).ToList();
            stopwatch.Stop();

            foreach (var user in users)
            {
                Console.WriteLine(
                    user.FullName + " | " +
                    user.BirthDay + " | " + 
                    user.Gender);
            }

            Console.WriteLine("Время, затраченное на выборку в милисекундах: " + stopwatch.ElapsedMilliseconds);

            return;
        }
    }

    static void PrintUser(User user)
    {

    }
}