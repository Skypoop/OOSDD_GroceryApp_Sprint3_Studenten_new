using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> products;
        public ProductRepository()
        {
            products = [
                new Product(1, "Melk", 300),
                new Product(2, "Kaas", 100),
                new Product(3, "Brood", 400),
                new Product(4, "Cornflakes", 0),
                new Product(5, "Boter", 200),
                new Product(6, "Eieren", 150),
                new Product(7, "Appels", 250),
                new Product(8, "Bananen", 180),
                new Product(9, "Tomaten", 120),
                new Product(10, "Aardappelen", 500),
            ];
        }
        public List<Product> GetAll()
        {
            return products;
        }

        public Product? Get(int id)
        {
            return products.FirstOrDefault(p => p.Id == id);
        }

        public Product Add(Product item)
        {
            products.Add(item);
            return item;
        }

        public Product? Delete(Product item)
        {
            products.Remove(item);
            return item;
        }

        public Product? Update(Product item)
        {
            Product? product = products.FirstOrDefault(p => p.Id == item.Id);
            if (product == null) return null;
            product.Id = item.Id;
            return product;
        }
    }
}