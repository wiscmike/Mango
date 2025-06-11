using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service;

public class CouponService : ICouponService
{
    private readonly string couponUrl = StaticDetail.CouponAPIBase + "/api/Coupon";
    private readonly IBaseService baseService;

    public CouponService(IBaseService baseService)
    {
        this.baseService = baseService;
    }

    public async Task<ResponseDto?> GetAllCouponsAsync()
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.GET,
            Url = couponUrl
        });
    }

    public async Task<ResponseDto?> GetCouponByCodeAsync(string couponCode)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.GET,
            Url = couponUrl + "/GetByCode/" + couponCode
        });
    }

    public async Task<ResponseDto?> GetCouponByIdAsync(int id)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.GET,
            Url = couponUrl + $"/{id}"
        });
    }

    public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.POST,
            Data = couponDto,
            Url = couponUrl
        });
    }

    public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.PUT,
            Data = couponDto,
            Url = couponUrl
        });
    }

    public async Task<ResponseDto?> DeleteCouponAsync(int id)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.DELETE,
            Url = couponUrl + $"/{id}"
        });
    }
}