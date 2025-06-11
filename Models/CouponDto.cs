using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Blazor.Models;

public class CouponDto
{
    public int CouponId { get; set; }

    [Required]
    public string CouponCode { get; set; } = string.Empty;

    [Required]
    [Range(1, double.MaxValue, ErrorMessage = "Discount Amount must be greater than 0")]
    public double DiscountAmount { get; set; }

    [Required]
    [Range(1, double.MaxValue, ErrorMessage = "Minimum Amount must be greater than 0")]
    public int MinAmount { get; set; }
}
