using Microsoft.EntityFrameworkCore;
using Moq;
using Olympics.Database;
using Olympics.Metier.Models;
using Olympics.Services;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blazored.SessionStorage;

namespace OlympicsTest
{
    [TestClass]
    public class PanierServiceTest
    {
        private PanierService _panierService;
        private ApplicationDbContext _context;
        private Mock<ISessionStorageService> _mockSessionStorageService;

        [TestInitialize]
        public void Setup()
        {
            // DbContext en mémoire pour les tests
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                          .UseInMemoryDatabase(databaseName: "TestDatabase")
                          .Options;

            _context = new ApplicationDbContext(options);

            // Initialisation des mocks
            _mockSessionStorageService = new Mock<ISessionStorageService>();

            // Instanciation correcte du PanierService
            _panierService = new PanierService(_context, _mockSessionStorageService.Object);
        }

        [TestMethod]
        public async Task GetCartFromSessionAsync_ReturnsEmptyList_WhenSessionCartIsEmpty()
        {
            // Arrange
            _mockSessionStorageService.Setup(s => s.GetItemAsync<string>("cart", It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty); // Session cart vide

            // Act
            var result = await _panierService.GetCartFromSessionAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetCartFromSessionAsync_ReturnsList_WhenSessionCartIsNotEmpty()
        {
            // Arrange
            var tickets = new List<cTicket> { new cTicket { IDTicket = 1 } };
            var cartJson = JsonSerializer.Serialize(tickets);

            _mockSessionStorageService.Setup(s => s.GetItemAsync<string>("cart", It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartJson); // Session cart non vide

            // Act
            var result = await _panierService.GetCartFromSessionAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(tickets.Count, result.Count);
        }


        [TestMethod]
        public async Task SetCartInSessionAsync_SetsCartInSessionStorage()
        {
            // Arrange
            var cart = new List<cTicket> { new cTicket { IDTicket = 1 } };

            // Act
            await _panierService.SetCartInSessionAsync(cart);

            // Assert
            _mockSessionStorageService.Verify(s => s.SetItemAsync("cart", It.IsAny<List<cTicket>>(), CancellationToken.None), Times.Once);

        }

        [TestMethod]
        public async Task ClearCartFromSessionAsync_RemovesCartFromSessionStorage()
        {
            // Act
            await _panierService.ClearCartFromSessionAsync();

            // Assert
            _mockSessionStorageService.Verify(s => s.RemoveItemAsync("cart", It.IsAny<CancellationToken>()), Times.Once);
        }


        [TestMethod]
        public async Task CreatePanierAsync_AddsPanierToDb()
        {
            // Arrange
            var newPanier = new cPanierBase { /* Initialize properties */ };

            // Act
            await _panierService.CreatePanierAsync(newPanier);

            // Assert
            var panierCount = await _context.Panier.CountAsync();
            Assert.AreEqual(1, panierCount);

            var panierInDb = await _context.Panier.FirstOrDefaultAsync();
            Assert.IsNotNull(panierInDb);
            Assert.AreEqual(newPanier.IDPanier, panierInDb.IDPanier);
        }
    }
}
