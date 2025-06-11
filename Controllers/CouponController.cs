using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CouponController : Controller
{
    private readonly ICouponService couponService;

    public CouponController(ICouponService couponService)
    {
        this.couponService = couponService;
    }

    public async Task<IActionResult> CouponIndex()
    {
        List<CouponDto>? couponList = [];

        var response = await couponService.GetAllCouponsAsync();

        if (response != null && response.IsSuccess)
        {
            var result = Convert.ToString(response.Result);

            if (result is not null)
            {
                couponList = JsonConvert.DeserializeObject<List<CouponDto>>(result);
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

        return View(couponList);
    }

    public IActionResult CouponCreate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CouponCreate(CouponDto couponDto)
    {
        if (ModelState.IsValid) 
        {
            var response = await couponService.CreateCouponAsync(couponDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon created successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
        }
        
        return View(couponDto);
    }

    public async Task<IActionResult> CouponEdit(int couponId)
    {
        var response = await couponService.GetCouponByIdAsync(couponId);

        if (response != null && response.IsSuccess)
        {
            var result = Convert.ToString(response.Result);

            if (result is not null)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDto>(result);

                return View(coupon);
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
    public async Task<IActionResult> CouponEdit(CouponDto couponDto)
    {
        if (ModelState.IsValid)
        {
            var response = await couponService.UpdateCouponAsync(couponDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon updated successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
        }

        return View(couponDto);
    }


    public async Task<IActionResult> CouponDelete(int couponId)
    {
        var response = await couponService.GetCouponByIdAsync(couponId);

        if (response != null && response.IsSuccess)
        {
            var result = Convert.ToString(response.Result);

            if (result is not null)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDto>(result);

                return View(coupon);
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
    public async Task<IActionResult> CouponDelete(CouponDto couponDto)
    {
        var response = await couponService.DeleteCouponAsync(couponDto.CouponId);

        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon deleted successfully";
            return RedirectToAction(nameof(CouponIndex));
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(couponDto);
    }
}
