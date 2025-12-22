using CvAppenVS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//i nyare versioner behövs inte startup-klassen.

// Add services to the container.
builder.Services.AddControllersWithViews();

//Connection string:
builder.Services.AddDbContext<CvContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

// builder.Services.AddScoped<UserService>(); <-- tror den är bäst med ickestatisk data
//Detta ska alltså ersätta IConfiguration-delen som hanif hade i sitt exempel.

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<CvContext>()
.AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var scope = app.Services.CreateScope();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

async Task SeedUsers() //har lagt en bit av exempel-datan i program pga arvet av IdentityUser
{
    if (await userManager.FindByEmailAsync("anna@example.com") == null)
    {
        var user1 = new User
        {
            Id = "user-1",
            UserName = "anna@example.com",
            Email = "anna@example.com",
            Name = "Anna Andersson",
            Address = "Stockholm",
            IsPrivate = false,
            Image = "anna.jpg"
        };
        await userManager.CreateAsync(user1, "Password123!");
    }

    if (await userManager.FindByEmailAsync("erik@example.com") == null)
    {
        var user2 = new User
        {
            Id = "user-2", 
            UserName = "erik@example.com",
            Email = "erik@example.com",
            Name = "Erik Svensson",
            Address = "Göteborg",
            IsPrivate = true,
            Image = "erik.jpg"
        };
        await userManager.CreateAsync(user2, "Password123!");
    }
}

await SeedUsers();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
