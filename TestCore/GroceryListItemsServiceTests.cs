using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Grocery.Core.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TestCore
{
    [TestFixture]
    public class GroceryListItemsServiceTests
    // Unit test voor UC2 : Tonen inhoud van een boodschappenlijst
    {
        private Mock<IGroceryListItemsRepository> _groceryListItemsRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private GroceryListItemsService _service;

        [SetUp]
        public void Setup()
        {
            // Arrange
            _groceryListItemsRepositoryMock = new Mock<IGroceryListItemsRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();

            _service = new GroceryListItemsService(
                _groceryListItemsRepositoryMock.Object,
                _productRepositoryMock.Object
            );
        }

        [Test]
        public void GetAllOnGroceryListId_WhenCalled_ReturnsFilteredAndFilledItems()
        {
            // Arrange
            int targetGroceryListId = 1;
            var product1 = new Product(101, "Milk", 10);
            var product2 = new Product(102, "Bread", 20);

            var allItems = new List<GroceryListItem>
            {
                new(1, targetGroceryListId, product1.Id, 2), 
                new(2, 2, product2.Id, 1), // Belongs to different list
                new(3, targetGroceryListId, product2.Id, 3)  
            };

            // Setup repository mocks
            _groceryListItemsRepositoryMock.Setup(r => r.GetAll()).Returns(allItems);
            _productRepositoryMock.Setup(r => r.Get(product1.Id)).Returns(product1);
            _productRepositoryMock.Setup(r => r.Get(product2.Id)).Returns(product2);

            // Act
            var result = _service.GetAllOnGroceryListId(targetGroceryListId);

            // Assert
            // Verify that only items for the target list are returned
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(item => item.GroceryListId == targetGroceryListId));

            // Verify that the Product property was filled correctly
            var item1 = result.FirstOrDefault(i => i.Id == 1);
            var item3 = result.FirstOrDefault(i => i.Id == 3);

            Assert.IsNotNull(item1);
            Assert.AreEqual("Milk", item1.Product.Name);

            Assert.IsNotNull(item3);
            Assert.AreEqual("Bread", item3.Product.Name);

            _groceryListItemsRepositoryMock.Verify(r => r.GetAll(), Times.Once);
            _productRepositoryMock.Verify(r => r.Get(product1.Id), Times.Once);
            _productRepositoryMock.Verify(r => r.Get(product2.Id), Times.Once);
        }
    }
}
