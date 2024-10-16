using Blazored.LocalStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Moq;
using Olympics.Database;
using Olympics.Database.Services;
using Olympics.Metier.Models;
using Olympics.Metier.Utils;
using Olympics.Services;

namespace OlympicsTest
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<SessionService> _mockSessionService;
        private Mock<PanierService> _mockPanierService;
        private Mock<SecurityManager> _mockSecurityManager;

        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IDataProtectionProvider> _mockDataProtectionProvider;
        private Mock<ILocalStorageService> _mockLocalStorageService;


        private UserService _userService;
        private ApplicationDbContext _context;

        [TestInitialize]
        public void Setup()
        {
            // DbContext en mémoire pour les tests
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                          .UseInMemoryDatabase(databaseName: "TestDatabase")
                          .Options;

            _context = new ApplicationDbContext(options);

            // Mock et initialisation des autres dépendances
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
            _mockLocalStorageService = new Mock<ILocalStorageService>();

            _mockSecurityManager = new Mock<SecurityManager>();

            // Mock de IJSRuntime
            var mockJsRuntime = new Mock<IJSRuntime>();

            // Création du PanierService et SessionService avec les mocks requis
            _mockPanierService = new Mock<PanierService>(_context, mockJsRuntime.Object);

            _mockSessionService = new Mock<SessionService>(
                _mockHttpContextAccessor.Object,
                _mockDataProtectionProvider.Object,
                _mockLocalStorageService.Object,
                _mockSecurityManager.Object
            );

            // Initialisation de UserService avec les dépendances nécessaires
            _userService = new UserService(
                _context,
                _mockHttpContextAccessor.Object,
                _mockPanierService.Object,
                _mockDataProtectionProvider.Object,
                _mockSessionService.Object,
                _mockLocalStorageService.Object,
                _mockSecurityManager.Object
            );
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldReturnFalse_WhenUserAlreadyExists()
        {
            // Arrange
            var existingUser = new cUtilisateurBase
            {
                EmailClient = "test@example.com",
                NomClient = "LoL",
                PrenomClient = "Ezreal",
                ShaMotDePasse = "password",
                Salt = "randomSalt",
                Key = "randomKey"
            };

            _context.Utilisateurs.Add(existingUser);
            await _context.SaveChangesAsync(); // Ajoute cet utilisateur au contexte

            var newUser = new cUtilisateurBase
            {
                EmailClient = "test@example.com",
                NomClient = "LoL",
                PrenomClient = "Ezreal",
                ShaMotDePasse = "password",
                Salt = "randomSalt",
                Key = "randomKey"
            };

            // Act
            var result = await _userService.RegisterUserAsync(newUser);

            // Assert
            Assert.IsFalse(result); // Vérifie que l'inscription échoue car l'utilisateur existe déjà
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldReturnTrue_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var newUser = new cUtilisateurBase
            {
                EmailClient = "quadruple@floraison.com",
                NomClient = "Quatre",
                PrenomClient = "Jhin",
                ShaMotDePasse = "password",
                Salt = "randomSalt",
                Key = "randomKey"
            };

            // Act
            var result = await _userService.RegisterUserAsync(newUser);

            // Assert
            Assert.IsTrue(result); // Vérifie que l'utilisateur est inscrit avec succès
            Assert.AreEqual(1, _context.Utilisateurs.Count()); // Vérifie qu'un utilisateur a bien été ajouté
        }

     


        [TestCleanup]
        public void Cleanup()
        {
            // Nettoyage de la base de données en mémoire après chaque test
            _context.Database.EnsureDeleted();
        }
    }
}
