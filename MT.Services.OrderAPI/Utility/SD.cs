namespace MT.Services.OrderAPI.Utility;

public class SD
{
    public enum OrderStatus
    {
        Pending,
        Approved,
        ReadyForPickup,
        Completed,
        Refunded,
        Canceled
    }

    public static string RoleAdmin = "admin".ToUpper();
    public static string RoleCustomer = "customer".ToUpper();
}