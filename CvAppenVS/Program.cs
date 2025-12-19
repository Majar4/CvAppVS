using CvAppenVS.Models;

var builder = WebApplication.CreateBuilder(args);

//i nyare versioner behövs inte startup-klassen.

// Add services to the container.
builder.Services.AddControllersWithViews();

//Connection string:
//builder.Services.AddDbContext<CvContext>(options => 
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

// builder.Services.AddScoped<UserService>(); <-- tror den är bäst med ickestatisk data
//Detta ska alltså ersätta IConfiguration-delen som hanif hade i sitt exempel.


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

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
