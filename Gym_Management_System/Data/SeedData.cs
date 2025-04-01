using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            string[] roles = { "Admin", "Trainer", "Receptionist", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var branches = context.GymBranches.OrderBy(b => b.BranchId).ToList();
            var random = new Random();
            if (branches.Count < 5)
            {
                Console.WriteLine("❌ 请先确保你在 OnModelCreating 中添加了 5 个 GymBranch。");
                return;
            }

            // ✅ Admin
            string adminEmail = "admin@gym.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    Name = "Admin User",
                    JoinDate = DateTime.UtcNow,
                    DOB = new DateTime(1985, 1, 1) // ✅ 加生日
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ✅ Trainer
            var trainerInfos = new[]
            {
                new { Email = "anna.trainer@gym.com", Name = "Anna Strong", Spec = "Yoga" },
                new { Email = "bob.trainer@gym.com", Name = "Bob Flex", Spec = "Weightlifting" },
                new { Email = "carol.trainer@gym.com", Name = "Carol Fit", Spec = "Cardio" },
                new { Email = "dave.trainer@gym.com", Name = "Dave Sprint", Spec = "HIIT" },
                new { Email = "eva.trainer@gym.com", Name = "Eva Calm", Spec = "Pilates" }
            };

            for (int i = 0; i < trainerInfos.Length; i++)
            {
                var info = trainerInfos[i];
                if (await userManager.FindByEmailAsync(info.Email) == null)
                {
                    var user = new Trainer
                    {
                        UserName = info.Email.Split('@')[0],
                        Email = info.Email,
                        Name = info.Name,
                        JoinDate = DateTime.UtcNow,
                        DOB = new DateTime(1980 + i, 3, 15), // ✅ 加生日（1980-1984）
                        Specialization = info.Spec,
                        ExperienceStarted = DateTime.UtcNow.AddYears(-2),
                        BranchId = branches[random.Next(branches.Count)].BranchId
                    };

                    var result = await userManager.CreateAsync(user, "Trainer@123");
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(user, "Trainer");
                }
            }

            // ✅ Customer
            var customers = new[]
            {
                new { Email = "amy.cust@gym.com", Name = "Amy Runner", Dob = new DateTime(1996, 7, 20) },
                new { Email = "brian.cust@gym.com", Name = "Brian Lifter", Dob = new DateTime(1998, 2, 10) },
                new { Email = "chloe.cust@gym.com", Name = "Chloe Swimmer", Dob = new DateTime(1995, 11, 5) }
            };

            foreach (var info in customers)
            {
                if (await userManager.FindByEmailAsync(info.Email) == null)
                {
                    var customer = new Customer
                    {
                        UserName = info.Email.Split('@')[0],
                        Email = info.Email,
                        Name = info.Name,
                        JoinDate = DateTime.UtcNow,
                        DOB = info.Dob, // ✅ 加生日
                        MembershipType = "Monthly",
                        SubscriptionDate = DateTime.UtcNow,
                        GymBranchId = branches[random.Next(branches.Count)].BranchId
                    };

                    var result = await userManager.CreateAsync(customer, "Customer@123");
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(customer, "Customer");
                }
            }

            // ✅ Receptionist
            var receps = new[]
            {
                new { Email = "jane.recep@gym.com", Name = "Jane Desk", Available = true, Dob = new DateTime(1990, 6, 18) },
                new { Email = "liam.recep@gym.com", Name = "Liam Welcome", Available = false, Dob = new DateTime(1992, 9, 5) }
            };

            for (int i = 0; i < receps.Length; i++)
            {
                var info = receps[i];
                if (await userManager.FindByEmailAsync(info.Email) == null)
                {
                    var user = new Receptionist
                    {
                        UserName = info.Email.Split('@')[0],
                        Email = info.Email,
                        Name = info.Name,
                        JoinDate = DateTime.UtcNow,
                        DOB = info.Dob, // ✅ 加生日
                        Responsibilities = "Front desk, Booking, Customer Support",
                        Notes = info.Available ? "Always early" : "On vacation",
                        IsAvailable = info.Available,
                        BranchId = branches[random.Next(branches.Count)].BranchId
                    };

                    var result = await userManager.CreateAsync(user, "Receptionist@123");
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(user, "Receptionist");
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
