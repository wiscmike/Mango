using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI.Repositories
{
    public interface IProductRepository
    {
        Task<ProductDto?> CreateProduct(ProductDto ProductDto);
        Task<int> DeleteProduct(int id);
        Task<IEnumerable<ProductDto>> GetAllProducts();
        Task<ProductDto?> GetProductByName(string name);
        Task<ProductDto?> GetProductById(int id);
        Task<ProductDto?> UpdateProduct(ProductDto ProductDto);
    }
}