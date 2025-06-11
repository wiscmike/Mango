using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService productService;

        public HomeController(IProductService ProductService)
        {
            this.productService = ProductService;
        }

        public async Task<IActionResult> Index()
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

        //[Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDto? product = new();

            ResponseDto? response = await productService.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
