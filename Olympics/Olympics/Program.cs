using AuthentificationServer.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Olympics.Database;
using Olympics.Database.Services;
using Olympics.Metier.Utils;
using Olympics.Presentation.Components;
using Olympics.Services;
using ProtectedLocalStore;
using Radzen;


var builder = WebApplication.CreateBuilder(args);

//le service de nettoyage des sessions
builder.Services.AddHostedService<SessionCleanupService>();

// les services n�cessaires � l'application
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024);

builder.Services.AddControllers();
builder.Services.AddRadzenComponents();
builder.Services.AddHttpClient();
builder.Services.AddAuthorizationCore();
// Cl� et IV (�quivalent � Aes.Create())
//builder.Services.AddProtectedLocalStore(new EncryptionService(
//                new ProtectedLocalStore.KeyInfo("z4i7MT3dmNnlcTAzEsLvf66sLqW1e7lrohv2BD6Luyg=", "SM8A/pEln/rNlpEfyDN3gw==")));
//  DbContext avec PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//  IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

//  Data Protection
builder.Services.AddDataProtection();

// Enregistrement des services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<PanierService>();
builder.Services.AddScoped<PayementService>();
builder.Services.AddScoped<OffreService>();


//Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

//le cache distribu� et les sessions avant la construction de l'application
builder.Services.AddDistributedMemoryCache(); // N�cessaire pour stocker les sessions en m�moire
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Expiration de la session apr�s 30 minutes d'inactivit�
    options.Cookie.HttpOnly = true; // Emp�che l'acc�s au cookie par JavaScript
    options.Cookie.IsEssential = true; // Le cookie est n�cessaire m�me si les cookies non essentiels sont refus�s
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Le cookie est uniquement transmis via HTTPS
});

var app = builder.Build();

// Middleware pour g�rer la session
app.UseSession();

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

// Redirection des requ�tes HTTP vers HTTPS
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
