using BioMech.Models;
using BioMech.Repositories;
using BioMech.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Firebase.Storage;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddHttpContextAccessor();


builder.Services.AddTransient<SupportService>();
builder.Services.AddTransient<AuthenticationService>();
builder.Services.AddTransient<AuthenticationRepository>();
builder.Services.AddTransient<PersonalAccountService>();
builder.Services.AddTransient<PersonalAccountRepository>();
builder.Services.AddTransient<DiagnosticsService>();
builder.Services.AddTransient<DiagnosticsRepository>();
builder.Services.AddTransient<ArticlesService>();
builder.Services.AddTransient<ArticlesRepository>();
builder.Services.AddTransient<TrainingProgramsService>();
builder.Services.AddTransient<TrainingProgramsRepository>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();  // сервисы сессии

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(365); // Установка срока действия сессии на 365 дней
});


var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();
builder.Services.AddSingleton(apiSettings);

builder.Services.AddHttpClient();


var app = builder.Build();

app.UseSession();


app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        ctx.Context.Response.Headers.Append("Pragma", "no-cache");
        ctx.Context.Response.Headers.Append("Expires", "0");
    }
});



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
