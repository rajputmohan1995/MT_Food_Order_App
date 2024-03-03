using System.ComponentModel;

namespace MT.Web.Utility;

public class SD
{
    public const string JsonType = "application/json";
    public static string ProductAPIBase = "";
    public static string CouponAPIBase = "";
    public static string AuthAPIBase = "";
    public static string ShoppingCartAPIBase = "";
    public static string OrderAPIBase = "";

    public static string RoleAdmin = "admin".ToUpper();
    public static string RoleCustomer = "customer".ToUpper();
    public const string TokenCookie = "JWT_Token";

    public const string InternalErrorOccured = "Internal Error Occured";
    public const string RupeeHtml = "&#8377;";
    public enum ApiType
    {
        GET, POST, PUT, DELETE
    }

    public enum Roles
    {
        ADMIN,
        CUSTOMER
    }

    public static List<string> GetRoles()
    {
        return Enum.GetValues(typeof(Roles))
                    .Cast<Roles>()
                    .Select(v => v.ToString())
                    .ToList();
    }

    public enum OrderStatus
    {
        Pending,
        Approved,
        [Description("Ready For Pickup")]
        ReadyForPickup,
        Completed,
        Refunded,
        [Description("Cancelled")]
        Canceled
    }

    public enum ContentType
    {
        Json,
        MultipartFormData
    }
}