
namespace Mango.Web.Blazor.Utilities
{
    public class StaticUtility
    {
        public static string CouponAPIName { get; } = "CouponAPI";

        public static string ProductAPIName { get; } = "ProductAPI";

        public static string AuthAPIName { get; set; } = "AuthAPI";

        public static string CartAPIName { get; set; } = "CartAPI";

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JWTToken";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
