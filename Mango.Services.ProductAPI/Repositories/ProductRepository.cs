using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly IMapper mapper;

        public ProductRepository(AppDbContext appDbContext, IMapper mapper)
        {
            this.appDbContext = appDbContext;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            var Products = await appDbContext.Products.ToListAsync();

            return mapper.Map<IEnumerable<ProductDto>>(Products);
        }

        public async Task<ProductDto?> GetProductById(int id)
        {
            var Product = await appDbContext.Products.FirstOrDefaultAsync(c => c.ProductId == id);

            return Product is not null ? mapper.Map<ProductDto>(Product) : null;
        }

        public async Task<ProductDto?> GetProductByName(string name)
        {
            var Product = await appDbContext.Products.FirstOrDefaultAsync(c => c.Name.ToLower().Equals(name.ToLower()));

            return Product is not null ? mapper.Map<ProductDto>(Product) : null;
        }

        public async Task<ProductDto?> CreateProduct(ProductDto ProductDto)
        {
            var Product = mapper.Map<Product>(ProductDto);

            await appDbContext.Products.AddAsync(Product);

            appDbContext.SaveChanges();

            return Product is not null ? mapper.Map<ProductDto>(Product) : null;
        }

        public Task<ProductDto?> UpdateProduct(ProductDto ProductDto)
        {
            var Product = mapper.Map<Product>(ProductDto);

            appDbContext.Products.Update(Product);

            appDbContext.SaveChanges();

            return Task.FromResult(Product is not null ? mapper.Map<ProductDto>(Product) : null);
        }

        public async Task<int> DeleteProduct(int id)
        {
            var Product = await appDbContext.Products.FirstAsync(c => c.ProductId == id);

            if (Product is null) return 0;

            appDbContext.Products.Remove(Product);

            var result = appDbContext.SaveChanges();

            return result;
        }
    }
}
