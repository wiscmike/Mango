using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Repositories
{
    public interface ICartRepository
    {
        Task<CartDto> GetCart(string userId);

        Task<bool> ApplyCoupon(CartDto cartDto);

        Task<bool> RemoveCoupon(CartDto cartDto);

        Task<CartDto> ShoppingCartUpsert(CartDto cartDto);

        Task<bool> RemoveCartDetail(int cartDetailId);
    }
}