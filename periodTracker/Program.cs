using System.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using periodTracker.Data;
using periodTracker.Models;
using periodTracker.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PeriodDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));

builder.Services.AddIdentity<User, IdentityRole>(
    options =>
    {
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
    }
    )
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PeriodDbContext>()
    .AddDefaultTokenProviders();
// Add services to the container.
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailService, MailService>();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Get the RoleManager service
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        // Define the roles you want to create
        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            // Check if the role already exists
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Create the role if it does not exist
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to create role '{roleName}'");
                }
            }
        }
        // Set the default admin username and password
        const string adminUsername = "admin@gmail.com";
        const string adminPassword = "Admin@1234"; // Replace with your secure password
        const string dateString = "2024-02-06";
        // Check if the admin user exists, create if not
        var adminUser = await userManager.FindByEmailAsync(adminUsername);
        if (adminUser == null)
        {
            // Create a new admin user
            var user = new User
            {
                FirstName="Abebech",
                LastName="Lemma",
                Age=35,
                UserName = adminUsername,
                Email = adminUsername,
                DateOfBirth = DateTime.Parse(dateString),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                // Add the admin user to the Admin role
                var addToRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception("Failed to add admin user to the Admin role.");
                }
            }
            else
            {
                throw new Exception($"Failed to create admin user.Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
    catch (Exception ex)
    {
        // Log the exception if the role creation or user creation fails
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating roles or admin user.");
    }
}
    



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index2}/{id?}");

app.Run();



