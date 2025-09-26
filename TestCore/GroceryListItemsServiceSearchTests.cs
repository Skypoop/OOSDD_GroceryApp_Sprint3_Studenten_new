using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.Core.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TestCore
{
    [TestFixture]
    public class GroceryListItemsServiceSearchTests
    // Unit tests for UC8 - Search for products to add to a grocery list
    {
        private Mock<IGroceryListItemsRepository> _groceryListItemsRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IProductService> _productServiceMock;
        private GroceryListItemsService _service;

        [SetUp]
        public void Setup()
        {
            _groceryListItemsRepositoryMock = new Mock<IGroceryListItemsRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _productServiceMock = new Mock<IProductService>();

            _service = new GroceryListItemsService(
                _groceryListItemsRepositoryMock.Object,
                _productRepositoryMock.Object,
                _productServiceMock.Object
            );
        }

        [TestCase("an", 2, new[] { "Banana", "Orange" })]
        [TestCase(null, 3, new[] { "Apple", "Banana", "Orange" })]
        [TestCase("xyz", 0, new string[0])]
        public void GetAvailableProducts_WithVariousSearchQueries_ReturnsFilteredProducts(string? searchQuery, int expectedCount, string[] expectedNames)
        {
            // Arrange
            int groceryListId = 1;

            var allProducts = new List<Product>
            {
                new(1, "Apple", 10),
                new(2, "Banana", 10),
                new(3, "Orange", 10),
                new(4, "Milk", 10),      // This will be on the list already
                new(5, "Bread", 0)       // This is out of stock
            };
            _productServiceMock.Setup(s => s.GetAll()).Returns(allProducts);

            var milkProduct = allProducts.First(p => p.Id == 4);
            var itemsOnList = new List<GroceryListItem>
            {
                new(101, groceryListId, milkProduct.Id, 1)
            };
            
            _groceryListItemsRepositoryMock.Setup(r => r.GetAll()).Returns(itemsOnList);
            
            _productRepositoryMock.Setup(r => r.Get(It.IsAny<int>()))
                                  .Returns((int id) => allProducts.FirstOrDefault(p => p.Id == id));

            // Act
            var result = _service.GetAvailableProducts(groceryListId, searchQuery);

            // Assert
            Assert.AreEqual(expectedCount, result.Count);
            CollectionAssert.AreEquivalent(expectedNames, result.Select(p => p.Name));

            // Verify mocks were called
            _productServiceMock.Verify(s => s.GetAll(), Times.Once);
            _groceryListItemsRepositoryMock.Verify(r => r.GetAll(), Times.Once);
        }
    }
}
