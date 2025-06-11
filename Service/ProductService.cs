using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service;

public class ProductService : IProductService
{
    private readonly string ProductUrl = StaticDetail.ProductAPIBase + "/api/product";
    private readonly IBaseService baseService;

    public ProductService(IBaseService baseService)
    {
        this.baseService = baseService;
    }

    public async Task<ResponseDto?> GetAllProductsAsync()
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.GET,
            Url = ProductUrl
        });
    }

    public async Task<ResponseDto?> GetProductByNameAsync(string name)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.GET,
            Url = ProductUrl + "/GetByName/" + name
        });
    }

    public async Task<ResponseDto?> GetProductByIdAsync(int id)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.GET,
            Url = ProductUrl + $"/{id}"
        });
    }

    public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.POST,
            Data = productDto,
            Url = ProductUrl
        });
    }

    public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.PUT,
            Data = productDto,
            Url = ProductUrl
        });
    }

    public async Task<ResponseDto?> DeleteProductAsync(int id)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.DELETE,
            Url = ProductUrl + $"/{id}"
        });
    }
}