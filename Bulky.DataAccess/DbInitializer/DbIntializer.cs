using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer
{
    public class DbIntializer : IDbIntializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _DB;

        public DbIntializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext DB)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _DB = DB;
        }
        public void Initialize()
        {
            //Migrations If Not Applied
            try
            {
                if (_DB.Database.GetPendingMigrations().Count() > 0)
                {
                    _DB.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }


            //Create rules if not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();


                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "AmrMansour10",
                    Email = "amrmansour1968@gmail.com",
                    PhoneNumber = "1234567890",
                    StreetAddress = "ElShaeed Wael Refat",
                    PostalCode = "02",
                    State = "United Electric",
                    City = "USA",
                },"Admin1234@").GetAwaiter().GetResult();

                var User = _DB.ApplicationUsers.FirstOrDefault(U=>U.Email == "amrmansour1968@gmail.com");

                _userManager.AddToRoleAsync(User, SD.Role_Admin);
            }
            return;
        }
    }
}
