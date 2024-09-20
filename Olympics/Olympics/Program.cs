using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Olympics.Database;
using Olympics.Database.Services;
using Olympics.Metier.Utils;
using Olympics.Presentation.Components;
using Olympics.Services;
using Radzen;


var builder = WebApplication.CreateBuilder(args);

// Ajouter les services n�cessaires � l'application
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024);

builder.Services.AddControllers();
builder.Services.AddRadzenComponents();
builder.Services.AddHttpClient();

// Ajouter DbContext avec PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter les services d'authentification
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
    });

// Ajouter IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Ajouter les services de session
builder.Services.AddDistributedMemoryCache(); // Utiliser un cache en m�moire pour les sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Dur�e de vie de la session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Enregistrement des services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<PanierService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

var app = builder.Build();

// Middleware pour g�rer la session
app.UseSession();

// Middleware pour cr�er un cookie lors de l'arriv�e sur le site
app.Use(async (context, next) =>
{
    if (!context.Request.Cookies.ContainsKey("VisitorId"))
    {
        var visitorId = Guid.NewGuid().ToString(); // ID unique pour chaque visiteur
        context.Response.Cookies.Append("VisitorId", visitorId, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30) // Expiration dans 30 jours
        });
    }

    await next();
});

// V�rification de la connexion � la base de donn�es
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        if (dbContext.Database.CanConnect())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Connexion � PostgreSQL r�ussie.");
        }
        else
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("�chec de la connexion � PostgreSQL.");
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Une erreur est survenue lors de la connexion � la base de donn�es.");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Activer l'authentification
app.UseAuthorization();  // Activer l'autorisation

app.UseAntiforgery();   // Activer la protection anti-forgery

app.MapControllers();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
