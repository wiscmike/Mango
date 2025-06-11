using Mango.Web.Blazor.Models;

namespace Mango.Web.Blazor.Services;

public interface ICouponService
{
    Task<ResponseDto?> GetAllCouponsAsync();

    Task<ResponseDto?> GetCouponByIdAsync(int id);

    Task<ResponseDto?> GetCouponByCodeAsync(string couponCode);

    Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);

    Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);

    Task<ResponseDto> DeleteCouponAsync(int id);


}