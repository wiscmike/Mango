using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.CouponAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers;

[Route("api/coupon")]
[ApiController]
//[Authorize]
public class CouponAPIController : ControllerBase
{
    private const string Admin = "ADMIN";

    private ResponseDto responseDto = new();
    private readonly ICouponRepository couponRepository;
    private readonly ILogger<CouponAPIController> logger;

    public CouponAPIController(ICouponRepository couponRepository, ILogger<CouponAPIController> logger)
    {
        this.couponRepository = couponRepository;
        this.logger = logger;
    }
    
    [HttpGet]
    public async Task<ResponseDto> Get()
    {
        try
        {
            logger.LogInformation("Returning all coupons");

            var result = await couponRepository.GetAllCoupons();

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
            logger.LogInformation($"Returning coupon by Id {id}");

            var result = await couponRepository.GetCouponById(id);

            responseDto.Result = result;
            if (result is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = $"Coupon Id {id} not found";
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

    [HttpGet("GetByCode/{code}")]
    public async Task<ResponseDto> GetByCode([FromRoute] string code)
    {
        try
        {
            logger.LogInformation($"Returning coupon by Code {code}");

            var result = await couponRepository.GetCouponByCode(code);

            responseDto.Result = result;
            if (result is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = $"Coupon Code {code} not found";
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
    public async Task<ResponseDto> CreateCoupon([FromBody] CouponDto couponDto)
    {
        try
        {
            logger.LogInformation("Creating new coupon");

            var result = await couponRepository.CreateCoupon(couponDto);

            responseDto.Result = result;
            if (result is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = $"Coupon not created";
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
    public Task<ResponseDto> UpdateCoupon([FromBody] CouponDto couponDto)
    {
        try
        {
            logger.LogInformation("Updating new coupon");

            var result = couponRepository.UpdateCoupon(couponDto);

            responseDto.Result = result;
            if (result is null)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = $"Coupon not updated";
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
    public async Task<ResponseDto> DeleteCoupon(int id)
    {
        try
        {
            logger.LogInformation($"Deleting coupon Id {id}");

            var result = await couponRepository.DeleteCoupon(id);

            responseDto.Result = result;
            if (result <= 0)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = $"Coupon not removed";
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