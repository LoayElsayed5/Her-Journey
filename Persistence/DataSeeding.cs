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

                if (!_userManager.Users.Any())
                {
                    var adminUser = new ApplicationUser()
                    {
                        Email = "loayayad2017@gmail.com",
                        DisplayName = "Loay ELsayed",
                        PhoneNumber = "01123062981",
                        UserName = "Loay05",
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(adminUser, "Lol01012345$");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(adminUser, "Admin");
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