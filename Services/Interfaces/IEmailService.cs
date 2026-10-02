namespace Ride_Hailing_API.Services.Interfaces;

public interface IEmailService
{
    Task SendRegistrationOtpEmailAsync(string toEmail, string firstName, string otpCode);

    Task SendResendOtpEmailAsync(string toEmail, string firstName, string otpCode);

    Task SendAccountVerifiedEmailAsync(string toEmail, string firstName);

    Task SendPasswordResetOtpEmailAsync(string toEmail, string firstName, string otpCode);

    Task SendPasswordChangedEmailAsync(string toEmail, string firstName);

    Task SendDriverApprovedEmailAsync(string toEmail, string firstName, int driverId);

    Task SendDriverRejectedEmailAsync(string toEmail, string firstName, int driverId, string reason);

    Task SendRideAcceptedEmailAsync(string toEmail, string passengerName, string driverName, string rideReference);

    Task SendDriverArrivedEmailAsync(string toEmail, string passengerName, string driverName, string rideReference);

    Task SendRideCancelledEmailAsync(string toEmail, string passengerName, string rideReference,string cancelledBy, string reason);

    Task SendRideCompletedEmailAsync(string toEmail, string passengerName, string rideReference, string driverName);
    Task SendAccountDeactivatedEmailAsync(string toEmail, string firstName, string reason);
}