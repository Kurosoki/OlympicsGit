using Blazored.LocalStorage;
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

// Ajouter les services nécessaires à l'application
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024);

builder.Services.AddControllers();
builder.Services.AddRadzenComponents();
builder.Services.AddHttpClient();
builder.Services.AddAuthorizationCore();

// Ajouter DbContext avec PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Ajouter Data Protection
builder.Services.AddDataProtection();

// Enregistrement des services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<PanierService>();
builder.Services.AddScoped<PayementService>();
builder.Services.AddScoped<OffresService>();

// Ajouter Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Ajouter le cache distribué et les sessions avant la construction de l'application
builder.Services.AddDistributedMemoryCache(); // Nécessaire pour stocker les sessions en mémoire
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Expiration de la session après 30 minutes d'inactivité
    options.Cookie.HttpOnly = true; // Empêche l'accès au cookie par JavaScript
    options.Cookie.IsEssential = true; // Le cookie est nécessaire même si les cookies non essentiels sont refusés
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Le cookie est uniquement transmis via HTTPS
});

var app = builder.Build();

// Middleware pour gérer la session
app.UseSession();

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

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Redirection des requêtes HTTP vers HTTPS
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
