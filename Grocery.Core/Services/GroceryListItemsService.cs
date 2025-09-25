using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository, IProductService productService)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
            _productService = productService;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillProducts(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillProducts(groceryListItems);
            return groceryListItems;
        }

        public List<Product> GetAvailableProducts(int groceryListId, string? query = null)
        {
            var allProducts = _productService.GetAll();
            var currentItemIds = GetAllOnGroceryListId(groceryListId).Select(i => i.ProductId);

            var filteredProducts = allProducts.Where(p =>
                !currentItemIds.Contains(p.Id) && p.Stock > 0);

            if (!string.IsNullOrWhiteSpace(query))
            {
                filteredProducts = filteredProducts.Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            return filteredProducts.ToList();
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            return _groceriesRepository.Delete(item);
        }

        public GroceryListItem? Get(int id)
        {
            var item = _groceriesRepository.Get(id);
            if (item != null)
            {
                FillProducts(new List<GroceryListItem> { item });
            }
            return item;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        private void FillProducts(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "Not Found", 0);
            }
        }
    }
}
