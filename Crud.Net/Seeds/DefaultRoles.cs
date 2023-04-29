
                       // THIS CLASS TO SEEDING ROLES
                       // write code which we need execute at the frist time of the program



using Crud.Net.Constant;
using Microsoft.AspNetCore.Identity;


namespace Crud.Net.Seeds
{
    public static class DefaultRoles        
    {
       public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)  // using static methods to create roles
        {
                                           // check that the Db contain any rol or not 'must db be empty becaus is the frist run of programm'
            if(! roleManager.Roles.Any()) // if exist any role then return true but we use applying code if no exist role in DB sao using (!)
            {
                                                                                               // Roles Are needing to seeding in DB withn the frist run program
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString())); // using Roles.SuperAdmin.ToString instead of write name of role "superadmin" becaouse using more strings in code not the best and may damage program
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));     // using ToString becaus we used Enum within define roles in class Roles.cs
                await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
            }
        }
    }
}
