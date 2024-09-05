using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Olympics.Database;
using Olympics.Database.Services;
using Olympics.Presentation.ServicesAuthen;

var builder = WebApplication.CreateBuilder(args);

// Configuration des services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login"; // Redirection vers la page de connexion si non authentifié
        options.LogoutPath = "/logout"; // Chemin pour la déconnexion
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Durée de vie du cookie
        options.SlidingExpiration = true; // Prolonge la durée de vie du cookie
    });

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Ajouter DbContext avec PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Enregistrement des Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthenticationService>();

var app = builder.Build();

// Vérification de la connexion à la base de données
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        if (dbContext.Database.CanConnect())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Connexion à PostgreSQL réussie.");
        }
        else
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Échec de la connexion à PostgreSQL.");
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Une erreur est survenue lors de la connexion à la base de données.");
    }
}

// Configurer le pipeline de requêtes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); // Gestion des erreurs en production
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Authentification avant d'autoriser
app.UseAuthorization(); // Autorisation

app.UseAntiforgery(); // Support des jetons anti-forgery

app.MapControllers();

app.Run();
