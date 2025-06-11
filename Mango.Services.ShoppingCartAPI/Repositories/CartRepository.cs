using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly IMapper mapper;
        private readonly IProductService productService;

        public CartRepository(AppDbContext appDbContext, IMapper mapper, IProductService productService)
        {
            this.appDbContext = appDbContext;
            this.mapper = mapper;
            this.productService = productService;
        }

        public async Task<CartDto?> GetCart(string userId)
        {
            try
            {
                var cartHeader = await appDbContext.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

                if (cartHeader == null) 
                {
                    return null;
                }
                var cart = new CartDto
                {
                    CartHeader = mapper.Map<CartHeaderDto>(cartHeader)
                };

                cart.CartDetails = mapper.Map<IEnumerable<CartDetailDto>>(appDbContext.CartDetails
                                                                                        .Where(c => c.CartHeaderId == cart.CartHeader.CartHeaderId));

                var productDtos = await productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(p => p.ProductId == item.ProductId);

                    if (item.Product is not null)
                    {
                        cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                    }
                }

                return cart;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CartDto> ShoppingCartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await appDbContext.CartHeaders
                                                            .AsNoTracking()
                                                            .FirstOrDefaultAsync(h => h.UserId == cartDto.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    // create the cart header and details
                    CartHeader cartHeader = mapper.Map<CartHeader>(cartDto.CartHeader);
                    appDbContext.CartHeaders.Add(cartHeader);
                    await appDbContext.SaveChangesAsync();

                    cartDto.CartHeader.CartHeaderId = cartHeader.CartHeaderId;
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    CartDetail cartDetail = mapper.Map<CartDetail>(cartDto.CartDetails.First());
                    appDbContext.CartDetails.Add(cartDetail);
                    await appDbContext.SaveChangesAsync();
                }
                else
                {
                    // if header is not null
                    // check if details has the same product
                    var cartDetailsFromDb = await appDbContext.CartDetails
                                                                .AsNoTracking()
                                                                .FirstOrDefaultAsync(
                                                                    d => d.ProductId == cartDto.CartDetails.First().ProductId &&
                                                                            d.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if (cartDetailsFromDb == null)
                    {
                        // create cart details
                        var cartDetail = mapper.Map<CartDetail>(cartDto.CartDetails.First());
                        cartDetail.CartHeaderId = cartHeaderFromDb.CartHeaderId;

                        appDbContext.CartDetails.Add(cartDetail);
                        await appDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        // update count in cart details
                        var cartDetail = mapper.Map<CartDetail>(cartDto.CartDetails.First());
                        cartDetail.CartDetailId = cartDetailsFromDb.CartDetailId;
                        cartDetail.CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        appDbContext.CartDetails.Update(cartDetail);
                        await appDbContext.SaveChangesAsync();
                    }
                }

                return cartDto;
            }
            catch (Exception)
            {

                throw;
            }
        }
    
        public async Task<bool> RemoveCartDetail(int cartDetailId)
        {
            try
            {
                var cartDetail = await appDbContext.CartDetails.FirstOrDefaultAsync(c => c.CartDetailId == cartDetailId);

                if (cartDetail is not null)
                { 
                    // get count of detail records with that header id
                    var totalCountOfCartItem = appDbContext.CartDetails.Where(c => c.CartHeaderId == cartDetail.CartHeaderId).Count();
                
                    appDbContext.CartDetails.Remove(cartDetail);

                    if (totalCountOfCartItem == 1)
                    {
                        // last detail item associated with the header
                        var cartHeaderToRemove = await appDbContext.CartHeaders.FirstOrDefaultAsync(c => c.CartHeaderId == cartDetail.CartHeaderId);

                        if (cartHeaderToRemove is not null)
                        {
                            appDbContext.CartHeaders.Remove(cartHeaderToRemove);
                        }
                        else
                        {
                            throw new Exception("Associated Cart Header record not found.");
                        }
                    }
                    await appDbContext.SaveChangesAsync();
                    
                    return true;
                }
                else
                {
                    throw new Exception("Cart Detail record not found.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ApplyCoupon(CartDto cartDto)
        {
            var userId = cartDto.CartHeader.UserId;

            var cartFromDb = await appDbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == userId);

            if (cartFromDb != null)
            {
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;

                appDbContext.CartHeaders.Update(cartFromDb);

                await appDbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> RemoveCoupon(CartDto cartDto)
        {
            var userId = cartDto.CartHeader.UserId;

            var cartFromDb = await appDbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == userId);

            if (cartFromDb != null)
            {
                cartFromDb.CouponCode = string.Empty;

                appDbContext.CartHeaders.Update(cartFromDb);

                await appDbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
