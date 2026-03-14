using DomainLayer.Contracts;
using DomainLayer.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Persistence.Data;

namespace Persistence
{
    public class DataSeeding(UserManager<ApplicationUser> _userManager,
        RoleManager<IdentityRole> _roleManager
        , StoreIdentityDbContext _identityDbContext) : IDataSeeding
    {
        public async Task IdentityDataSeedingAsync()
        {
            try
            {
                var roles = new List<string> { "Admin", "Doctor", "Patient" };

                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                }


                var admins = new List<ApplicationUser>
                {
                    new ApplicationUser()
                    {
                        Email = "Loay@gmail.com",
                        DisplayName = "Loay",
                        PhoneNumber = "01123062981",
                        UserName = "Loay",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now
                    },
                    new ApplicationUser()
                    {
                        Email = "Mariam@gmail.com",
                        DisplayName = "Mariam",
                        PhoneNumber = "01151660962",
                        UserName = "Mariam",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now
                    },
                    new ApplicationUser()
                    {
                        Email = "Maha@gmail.com",
                        DisplayName = "Maha",
                        PhoneNumber = "01000769828",
                        UserName = "Maha",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now
                    },
                    new ApplicationUser()
                    {
                        Email = "Shahd@gmail.com",
                        DisplayName = "Shahd",
                        PhoneNumber = "01010861461",
                        UserName = "Shahd",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now
                    },

                    };
                foreach (var adminUser in admins)
                {
                    var existingAdmin = await _userManager.FindByEmailAsync(adminUser.Email!);
                    if (existingAdmin == null)
                    {
                        var result = await _userManager.CreateAsync(adminUser, "Hj01012345$");

                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(adminUser, "Admin");
                        }
                    }
                }


                await _identityDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }
}