using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private const string Admin = "ADMIN";

        private ResponseDto responseDto = new();
        private readonly ICartRepository CartRepository;
        private readonly ILogger<CartAPIController> logger;

        public CartAPIController(ICartRepository cartRepository, ILogger<CartAPIController> logger)
        {
            this.CartRepository = cartRepository;
            this.logger = logger;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                var cartDto = await CartRepository.GetCart(userId);

                responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }

            return responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var success = await CartRepository.ApplyCoupon(cartDto);

                responseDto.Result = success;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }

            return responseDto;
        }


        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDto> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var success = await CartRepository.ApplyCoupon(cartDto);

                responseDto.Result = success;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }

            return responseDto;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var updatedCartDto = await CartRepository.ShoppingCartUpsert(cartDto);

                responseDto.Result = updatedCartDto;

                if (updatedCartDto is null)
                {
                    responseDto.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }

            return responseDto;
        }

        [HttpDelete("RemoveCart")]
        public async Task<ResponseDto> RemoveCartDetail([FromBody] int cartDetailId)
        {
            try
            {
                var success = await CartRepository.RemoveCartDetail(cartDetailId);

                responseDto.Result = success;
                responseDto.Message = "Successfully removed Cart Detail item.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                responseDto.Message = ex.Message;
                responseDto.IsSuccess = false;
            }

            return responseDto;
        }
    }
}
