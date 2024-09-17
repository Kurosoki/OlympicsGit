using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Olympics.Database.Services;
using Olympics.Database;
using Olympics.Presentation.Components;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services nécessaires à l'application
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents().AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024);

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

// Enregistrement des services
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Middleware pour créer un cookie lors de l'arrivée sur le site
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
