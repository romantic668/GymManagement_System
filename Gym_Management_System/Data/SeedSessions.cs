using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GymManagement.Models;

namespace GymManagement.Data
{
    public static class SeedSessions
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, bool clearBeforeSeed = false)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            await context.Database.EnsureCreatedAsync();

            if (clearBeforeSeed)
            {
                var existingCount = context.Sessions.Count();
                context.Sessions.RemoveRange(context.Sessions);
                await context.SaveChangesAsync();
                Console.WriteLine($"🧹 Cleared {existingCount} existing Sessions");
            }

            string[] roles = { "Admin", "Trainer", "Receptionist", "Customer" };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            var branches = context.GymBranches.OrderBy(b => b.BranchId).ToList();
            if (branches.Count < 5)
            {
                Console.WriteLine("❌ Please ensure you've added 5 GymBranch entries in OnModelCreating.");
                return;
            }

            var trainerMap = context.Users.OfType<Trainer>().ToDictionary(t => t.Name, t => t.Id);
            var receptionistMap = context.Users.OfType<Receptionist>().ToDictionary(r => r.Name, r => r.Id);

            if (trainerMap.Count == 0 || receptionistMap.Count == 0)
            {
                Console.WriteLine("❌ No Trainer or Receptionist found. Please run SeedUsers first.");
                return;
            }

            // ✅ Seed GymClasses if not exist
            if (!context.GymClasses.Any())
            {
                var gymClasses = new List<GymClass>();
                var random = new Random();
                var trainerIds = trainerMap.Values.Take(10).ToList();

                for (int i = 0; i < 10; i++)
                {
                    gymClasses.Add(new GymClass
                    {
                        ClassName = $"Class {i + 1}",
                        AvailableTime = DateTime.Today.AddDays(i),
                        Duration = random.Next(30, 120),
                        Description = $"This is a description for Class {i + 1}.",
                        TrainerId = trainerIds[i % trainerIds.Count]
                    });
                }
                context.GymClasses.AddRange(gymClasses);
                await context.SaveChangesAsync();
            }

            var gymClassIds = context.GymClasses.Select(c => c.GymClassId).Take(10).ToList();
            var roomIds = context.Rooms.Select(r => r.RoomId).Take(30).ToList();

            string[] names = trainerMap.Keys.Take(30).ToArray();
            string[] receptionistNames = receptionistMap.Keys.Take(30).ToArray();
            var rand = new Random();
            var sessionCategories = Enum.GetValues<SessionCategory>();

            var sessionNames = new[]
            {
                "Power Yoga", "Core Crusher", "Full Body Burn", "Spin & Sweat",
                "Strength Surge", "Cardio Blaze", "Mobility Flow", "Endurance Engine",
                "Flex Fusion", "Zen Stretch", "HIIT Riot", "Bootcamp Blast",
                "Core Revival", "Pulse Ride", "Iron Circuit", "Balance Burn"
            };

            var sessionDays = 7;
            var sessionsPerDay = 6;

            for (int dayOffset = 0; dayOffset < sessionDays; dayOffset++)
            {
                var date = DateTime.Today.AddDays(dayOffset);
                var shuffledCategories = sessionCategories.OrderBy(_ => rand.Next()).ToArray();

                for (int j = 0; j < sessionsPerDay; j++)
                {
                    var index = (dayOffset * sessionsPerDay + j);

                    var trainerName = names[index % names.Length];
                    var receptionistName = receptionistNames[index % receptionistNames.Length];

                    var sessionDate = date.AddHours(9 + (j % 4) * 2);
                    var roomId = roomIds[index % roomIds.Count];

                    bool exists = context.Sessions.Any(s =>
                        s.SessionDateTime == sessionDate && s.RoomId == roomId);

                    if (!exists)
                    {
                        var session = new Session
                        {
                            SessionName = sessionNames[index % sessionNames.Length],
                            SessionDateTime = sessionDate,
                            Capacity = rand.Next(10, 25),
                            Category = shuffledCategories[j % shuffledCategories.Length],
                            GymClassId = gymClassIds[index % gymClassIds.Count],
                            RoomId = roomId,
                            TrainerId = trainerMap[trainerName],
                            ReceptionistId = receptionistMap[receptionistName].ToString() // ✅ 修复
                        };
                        context.Sessions.Add(session);
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}