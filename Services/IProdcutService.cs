using Mango.Web.Blazor.Models;

namespace Mango.Web.Blazor.Services;

public interface IProductService
{
    Task<ResponseDto?> GetAllProductsAsync();

    Task<ResponseDto?> GetProductByIdAsync(int id);

    Task<ResponseDto?> GetProductByNameAsync(string productName);

    Task<ResponseDto?> CreateProductAsync(ProductDto productDto);

    Task<ResponseDto?> UpdateProductAsync(ProductDto productDto);

    Task<ResponseDto> DeleteProductAsync(int id);

}