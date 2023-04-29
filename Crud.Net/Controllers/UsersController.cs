                               //create actions of UserIndex 'view'
using Crud.Net.Data;
using Crud.Net.Models;
using Crud.Net.ViewModels;
using Crud.Net.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crud.Net.Controllers
{
    [Authorize(Roles ="SuperAdmin")]  // authorize to only users exist in superadmin role
    public class UsersController :Controller
    {
        //define usermanager , rolemanager

        private readonly UserManager<IdentityUser> _userManager;  
        private readonly RoleManager<IdentityRole> _roleManager;    
        private readonly SignInManager<IdentityUser> _signInManager;    

        // to use previus definations must inject (pass) them in constructor

        //creat constructor

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager; 
            _signInManager = signInManager; 

        }
        //define action of UsersIndex 
        public async Task<IActionResult> UsersIndex()
        {
            // define variable of usermanager to work with properties of user
            // define which appear from returne view(users)

            var users = await _userManager.Users.Select
                (user => new UsersViewModel    //castomize into UsersFormViewModel to making view as i need instead of the default  
                {
                    Id = user.Id,
                    UserName = user.UserName,  //ID ,USERNAME ... ARE DEFINED IN USERFORMVIEW
                    Email = user.Email,       //LINKING WITH builtin methods defined in .net
                    Roles = _userManager.GetRolesAsync(user).Result  //GetRolesAsync not support async so using Result instead of async 
                }).ToListAsync();

                return View(users);
        }

        // creat actions of manage role , update role

        public async Task<IActionResult> ManageRoles (string userId) // action of Manage rol button
        {
            // ManageRoles : is get method work with recieved Id
            // check if id exist in db or not

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roles = await _roleManager.Roles.ToListAsync(); // to feach all roles in DB

            var viewmodel = new UsersRolesViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                Roles = roles.Select(role => new CheackBoxViewModel  // convert view to the view which i need
                {
                    RoleName = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result  //isselected return true if the user assigen rols which i need to know about it
                }).ToList()                                                        //IsInRoleAsync(user, role.Name) takin user , name of role to cheack
        };
            return View(viewmodel); 
        }
        // action of UpdateRoles 'action apply after submit'

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UpdateRoles(UsersRolesViewModel model) // action of save button in manage roles view
        {

            // cheack userid exist or not in DB
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return NotFound();

            // if id exist determine roles assigend to this user

            var userRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, userRoles); //admin remove all roles from user
            await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName)); //add roles and cheack boxs for user
            return RedirectToAction(nameof(UsersIndex));

            //code if using foreach  but not the best
            //foreach (var role in model.Roles)
            //{
            //    if (userRoles.Any(r => r == role.RoleName) && !role.IsSelected)
            //        await _userManager.RemoveFromRoleAsync(user, role.RoleName);

            //    if (!userRoles.Any(r => r == role.RoleName) && role.IsSelected)
            //        await _userManager.AddToRoleAsync(user, role.RoleName);
            //}
        }





    }
    }

