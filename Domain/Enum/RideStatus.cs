namespace Ride_Hailing_API.Domain.Enum;

public enum RideStatus
{
    Requested,
    Accepted,
    DriverArriving,
    DriverArrived,
    InProgress,
    Completed,
    Cancelled
}