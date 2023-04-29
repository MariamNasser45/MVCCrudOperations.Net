using System;
using Crud.Net.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Crud.Net.Seeds;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Crud.Net.Migrations;
using Crud.Net.Constant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreHero.ToastNotification;
using NToastNotify;
using AspNetCoreHero.ToastNotification.Extensions;


var builder = WebApplication.CreateBuilder(args);


// register of NOTIFICATIONS
builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
{
    ProgressBar = true,
    Timeout = 3000,

});


//builder.Services.AddRazorPages().AddNToastNotifyToastr(new ToastrOptions()
//{
//    ProgressBar = true,
//    PositionClass = ToastPositions.TopRight,
//    PreventDuplicates = true,
//    CloseButton = true //to user closs tab if need
//});
//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Add ToastNotification

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//since we used IdentityRoles in cod then must define it here : replace .AddDefaultIdentity by .AddIdentity then add IdentityRole

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{ //options can be used to add spicifc options for email , pass
    options.SignIn.RequireConfirmedAccount = true;

   // options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._ ";
  
}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

//var roleManager = builder.Services.AddIdentityCore<RoleManager<IdentityRole>>();
//var userManager = builder.Services.AddIdentityCore<UserManager<IdentityUser>>(); this don't return service only build method


//builder.Services.AddScoped(DefaultRoles.SeedAsync(roleManager));
//builder.Services.AddScoped(DefaultUsers.SeedBasicUserAsync(userManager));
//builder.Services.AddScoped(DefaultUsers.SeedSuperAdminAsync(userManager, roleManager)); 


builder.Services.AddMvc().AddNToastNotifyNoty();
builder.Services.AddMvc().AddNToastNotifyToastr();

var app = builder.Build();

// command from  line 49 - 55 must be writen after line 57 

using var scope = app.Services.CreateScope(); // creating scope

var services = scope.ServiceProvider;
var loggerFactory = services.GetRequiredService<ILoggerProvider>();
var logger = loggerFactory.CreateLogger("app"); 

try
{
    // define variables to access roles , users in its classes

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    //seeding data to DB

    await Crud.Net.Seeds.DefaultRoles.SeedAsync(roleManager);
    await Crud.Net.Seeds.DefaultUsers.SeedBasicUserAsync(userManager);
    await Crud.Net.Seeds.DefaultUsers.SeedSuperAdminAsync(userManager, roleManager);

    // to returne notifications

    logger.LogInformation("Data seeded");
    logger.LogInformation("Application Started");
}
catch (System.Exception ex)
{
    logger.LogWarning(ex, "An error occurred while seeding data");
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseNToastNotify(); // tu define toast notification

app.UseNotyf();

app.MapRazorPages();

app.Run();
