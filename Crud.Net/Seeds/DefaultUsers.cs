
                                   //  seeding data for user 'Basic , SuperAdmin'
                                  //   creation permisions of users
using Crud.Net.Constant;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Crud.Net.Seeds
{
    public static class DefaultUsers
    {
        public static async Task  SeedBasicUserAsync(UserManager<IdentityUser> usermanager )
        { 
            var defaultuser = new IdentityUser
            {
                                                    // now define the values of the basic user
                UserName = "basicuser@domain.com", // we can use email different from UserName if we need
                Email = "basicuser@domain.com",
                EmailConfirmed = true,
            };

            // if run program then stoped it ant rerunning again program run as the frist time so
           // must check if this user exist in  DB or not to prevent error of duplicate data of basic user
       
             var user= await usermanager.FindByEmailAsync(defaultuser.Email);
                
             if(user==null) // if this email not exist in DB then creat it
            {
                await usermanager.CreateAsync(defaultuser, "P@assword123"); // create async recieve 2 parameter 'email , password'
                                                                           // pass must be need speccial char , number ,.... we can chanage this requirement bby edit its configration
                
                                                                                         // assigne permision to basic user role
                await usermanager.AddToRoleAsync(defaultuser, Roles.Basic.ToString());  //AddToRoleAsync recieve 2 parameter 'user , name of roles '
            }
        }

        
        // create super admin role same the code of basic user but this assigned all roles because is the superAdmin
        public static async Task SeedSuperAdminAsync(UserManager<IdentityUser> usermanager, RoleManager<IdentityRole> roleManager)
        {
            var defaultuser = new IdentityUser 
            {
                                                     // now define the values of the super admin user
                UserName = "superadmin@domain.com", // we can use email different from UserName
                Email = "superadmin@domain.com",
                EmailConfirmed = true,
            };

            // if run program then stoped it ant rerunning again program run as the frist time so
           // must check if this user exist in  DB or not to prevent error of duplicate data of user

            var user = await usermanager.FindByEmailAsync(defaultuser.Email);

            if (user == null) // if this email not exist in DB then creat it
            {
                                                                            //creation of user 
                await usermanager.CreateAsync(defaultuser, "P@assword123"); // create async recieve 2 parameter 'email , password'
                                                                          // pass must be need speccial char , number ,.... we can chanage this requirement bby edit its configration
                                                                         // assigne to basic user role 

                await usermanager.AddToRolesAsync(defaultuser, new List<string> { Roles.Admin.ToString() , Roles.SuperAdmin.ToString() , Roles.Basic.ToString()});  //AddToRolesAsync recieve 2 parameter 'user , list of roles'
            }

            await roleManager.SeedClaimsOfSuperUser();    // seed claims 'permissions' of the super Admin

        }

        // method to make seed claims of superAdmin
        // this is used as reflection method 'we can calling this method from inside rolemanger ' 
        public static async Task SeedClaimsOfSuperUser(this RoleManager<IdentityRole> rolemanager ) // recieving type  of thing we need to reflect it
        {
            var adminrole = await rolemanager.FindByNameAsync(Roles.SuperAdmin.ToString()); // select the role to make permissions of  this

                                                                         // now using fun which generate all name of permisions
            await rolemanager.AddPermisionClaims(adminrole , "Products"); //AddPermisionClaims recieve : role , module name
        }

        //this method to assigned permissions for the role  'must make this for all module in the app'
        public static async Task AddPermisionClaims(this RoleManager<IdentityRole> rolemanager , IdentityRole role , string module ) // in this we need 1- type of role which we need assigne permision to it
        {                                                                                                                           // module  which assigned its permissions to this role                          
            var allclaims = await rolemanager.GetClaimsAsync(role);  // check if there exist claims or not    

            // we need generate name of this permision so we nead creating method or function make this
            // instead of write code each time need this
          
            var allpermisions = Permisions.GeneratePermisionsList(module);  // now using the method GeneratePermisionsList which we created 

            foreach (var Permisions in allpermisions)   // now making loop to check all defined permisions not exist in DB  befor create it
            {
                if (!allclaims.Any(c => c.Type == "Permisions" && c.Value == Permisions)) // if there are no permisions which  we defined then create it
                    await rolemanager.AddClaimAsync(role, new Claim("Permisions", Permisions)); //claim fin also recieve type , value of parameter           
                        
            }


        }

    }
}
