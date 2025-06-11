using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService productService;

    public ProductController(IProductService ProductService)
    {
        this.productService = ProductService;
    }

    public async Task<IActionResult> ProductIndex()
    {
        List<ProductDto>? productList = [];

        var response = await productService.GetAllProductsAsync();

        if (response != null && response.IsSuccess)
        {
            var result = Convert.ToString(response.Result);

            if (result is not null)
            {
                productList = JsonConvert.DeserializeObject<List<ProductDto>>(result);
            }
            else
            {
                TempData["error"] = "Unknown Error";
            }
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(productList);
    }

    public IActionResult ProductCreate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProductCreate(ProductDto productDto)
    {
        if (ModelState.IsValid)
        {
            var response = await productService.CreateProductAsync(productDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product created successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
        }

        return View(productDto);
    }

    public async Task<IActionResult> ProductEdit(int productId)
    {
        var response = await productService.GetProductByIdAsync(productId);

        if (response != null && response.IsSuccess)
        {
            var result = Convert.ToString(response.Result);

            if (result is not null)
            {
                var product = JsonConvert.DeserializeObject<ProductDto>(result);

                return View(product);
            }
            else
            {
                TempData["error"] = "Unknown Error";
            }
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductEdit(ProductDto productDto)
    {
        if (ModelState.IsValid)
        {
            var response = await productService.UpdateProductAsync(productDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
        }

        return View(productDto);
    }


    public async Task<IActionResult> ProductDelete(int productId)
    {
        var response = await productService.GetProductByIdAsync(productId);

        if (response != null && response.IsSuccess)
        {
            var result = Convert.ToString(response.Result);

            if (result is not null)
            {
                var Product = JsonConvert.DeserializeObject<ProductDto>(result);

                return View(Product);
            }
            else
            {
                TempData["error"] = "Unknown Error";
            }
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductDelete(ProductDto productDto)
    {
        var response = await productService.DeleteProductAsync(productDto.ProductId);

        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(ProductIndex));
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(productDto);
    }
}
