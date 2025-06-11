using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers;

[Route("api/product")]
[ApiController]
//[Authorize]
public class ProductAPIController : ControllerBase
{
    private const string Admin = "ADMIN";

    private ResponseDto responseDto = new();
    private readonly IProductRepository ProductRepository;
    private readonly ILogger<ProductAPIController> logger;

    public ProductAPIController(IProductRepository productRepository, ILogger<ProductAPIController> logger)
    {
        this.ProductRepository = productRepository;
        this.logger = logger;
    }
    
    [HttpGet]
    public async Task<ResponseDto> Get()
    {
        try
        {
            logger.LogInformation("Returning all Products");

            var result = await ProductRepository.GetAllProducts();

            responseDto.Result = result;
            responseDto.IsSuccess = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            responseDto.IsSuccess = false;
            responseDto.Message = ex.Message;
        }

        return responseDto;
    }

    [HttpGet("{id:int}")]
    public async Task<ResponseDto> GetById([FromRoute] int id)
    {
        try
        {
            logger.LogInformation($"Returning Product by Id {id}");

            var result = await ProductRepository.GetProductById(id);

            responseDto.Result = result;
            if (result is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = $"Product Id {id} not found";
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            responseDto.IsSuccess = false;
            responseDto.Message = ex.Message;
        }

        return responseDto;
    }

    [HttpGet("GetByName/{name}")]
    public async Task<ResponseDto> GetByCode([FromRoute] string name)
    {
        try
        {
            logger.LogInformation($"Returning Product by Name {name}");

            var result = await ProductRepository.GetProductByName(name);

            responseDto.Result = result;
            if (result is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = $"Product Name {name} not found";
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            responseDto.IsSuccess = false;
            responseDto.Message = ex.Message;
        }

        return responseDto;
    }

    [HttpPost]
    //[Authorize(Roles = Admin)]
    public async Task<ResponseDto> CreateProduct([FromBody] ProductDto ProductDto)
    {
        try
        {
            logger.LogInformation("Creating new Product");

            var result = await ProductRepository.CreateProduct(ProductDto);

            responseDto.Result = result;
            if (result is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = $"Product not created";
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            responseDto.IsSuccess = false;
            responseDto.Message = ex.Message;
        }

        return responseDto;
    }

    [HttpPut]
    //[Authorize(Roles = Admin)]
    public Task<ResponseDto> UpdateProduct([FromBody] ProductDto ProductDto)
    {
        try
        {
            logger.LogInformation("Updating new Product");

            var result = ProductRepository.UpdateProduct(ProductDto);

            responseDto.Result = result;
            if (result is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = $"Product not updated";
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            responseDto.IsSuccess = false;
            responseDto.Message = ex.Message;
        }

        return Task.FromResult(responseDto);
    }

    [HttpDelete("{id:int}")]
    //[Authorize(Roles = Admin)]
    public async Task<ResponseDto> DeleteProduct(int id)
    {
        try
        {
            logger.LogInformation($"Deleting Product Id {id}");

            var result = await ProductRepository.DeleteProduct(id);

            responseDto.Result = result;
            if (result <= 0)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = $"Product not removed";
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            responseDto.IsSuccess = false;
            responseDto.Message = ex.Message;
        }

        return responseDto;
    }
}