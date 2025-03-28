using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using GymManagement.Models;

namespace GymManagement.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            await context.Database.EnsureCreatedAsync();

            // 1️⃣ 创建角色
            string[] roles = { "Admin", "Trainer", "Receptionist", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2️⃣ 添加 Admin 用户
            string adminEmail = "admin@gym.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    Name = "Admin User",
                    JoinDate = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // 3️⃣ 添加 Trainer 用户
            string trainerEmail = "trainer@gym.com";
            User trainerUser = await userManager.FindByEmailAsync(trainerEmail);
            if (trainerUser == null)
            {
                trainerUser = new User
                {
                    UserName = "john.trainer",
                    Email = trainerEmail,
                    Name = "John Trainer",
                    JoinDate = new DateTime(2024, 1, 1)
                };
                var result = await userManager.CreateAsync(trainerUser, "Trainer@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(trainerUser, "Trainer");
                }
            }

            // 4️⃣ 添加 Receptionist 用户
            string receptionistEmail = "receptionist@gym.com";
            if (await userManager.FindByEmailAsync(receptionistEmail) == null)
            {
                var receptionist = new User
                {
                    UserName = "mike.reception",
                    Email = receptionistEmail,
                    Name = "Mike Receptionist",
                    JoinDate = new DateTime(2024, 1, 1)
                };
                var result = await userManager.CreateAsync(receptionist, "Receptionist@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(receptionist, "Receptionist");
                }
            }

            // 5️⃣ 添加 GymClass 课程
            if (!context.GymClasses.Any())
            {
                context.GymClasses.Add(new GymClass
                {
                    GymClassId = 1,
                    ClassName = "Beginner Yoga",
                    AvailableTime = new DateTime(2025, 3, 15, 10, 0, 0),
                    Duration = 60,
                    TrainerId = trainerUser.Id // ✅ 正确引用 Identity 用户 ID
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
