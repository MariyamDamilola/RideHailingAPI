using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Ride_Hailing_API.Services.Interfaces;
using RideHailingAPI.Helpers;

namespace Ride_Hailing_API.Services.Implementation;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IConfiguration configuration,
        ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private async Task SendMimeMessageAsync(MimeMessage message)
    {
        var server = _configuration["Smtp:Server"];
        var port = int.Parse(_configuration["Smtp:Port"]!);
        var username = _configuration["Smtp:Username"];
        var password = _configuration["Smtp:Password"];

        _logger.LogInformation(
            $"Sending email to {message.To} (Subject: {message.Subject})");

        using var client = new SmtpClient();

        try
        {
            _logger.LogInformation(
                $"Connecting to SMTP server {server}:{port}");

            await client.ConnectAsync(
                server,
                port,
                SecureSocketOptions.StartTls);

            _logger.LogInformation("Connected to SMTP server");

            await client.AuthenticateAsync(
                username,
                password);

            _logger.LogInformation("Authenticated to SMTP server");

            await client.SendAsync(message);

            _logger.LogInformation(
                $"Email sent to {message.To} successfully");

            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while sending email to {message.To}");
        }
    }

    private MimeMessage CreateBaseMessage(
        string toEmail,
        string subject)
    {
        var senderName = _configuration["Smtp:SenderName"];
        var senderEmail = _configuration["Smtp:SenderEmail"];

        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                senderName,
                senderEmail));

        message.To.Add(
            new MailboxAddress(
                toEmail,
                toEmail));

        message.Subject = subject;

        return message;
    }

    public async Task SendRegistrationOtpEmailAsync(
        string toEmail,
        string firstName,
        string otpCode)
    {
        var subject = "Verify Your RideGo Account";

        var htmlBody =
            MailUtils.GetRegistrationOtpEmailHtml(
                firstName,
                toEmail,
                otpCode);

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }

    public async Task SendResendOtpEmailAsync(
        string toEmail,
        string firstName,
        string otpCode)
    {
        var subject = "Your New RideGo Verification Code";

        var htmlBody =
            MailUtils.GetResendOtpEmailHtml(
                firstName,
                toEmail);

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }

    public async Task SendAccountVerifiedEmailAsync(
        string toEmail,
        string firstName)
    {
        var subject = "Your RideGo Account Has Been Verified";

        var htmlBody =
            MailUtils.GetAccountVerifiedEmailHtml(
                firstName,
                toEmail);

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }

    public async Task SendPasswordResetOtpEmailAsync(
        string toEmail,
        string firstName,
        string otpCode)
    {
        var subject = "RideGo Password Reset Code";

        var htmlBody =
            MailUtils.GetPasswordResetOtpEmailHtml(
                firstName,
                toEmail);

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }

    public async Task SendPasswordChangedEmailAsync(
        string toEmail,
        string firstName)
    {
        var subject = "Your RideGo Password Has Been Changed";

        var htmlBody =
            MailUtils.GetPasswordChangedEmailHtml(
                firstName,
                toEmail);

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }

    public async Task SendDriverApprovedEmailAsync(
        string toEmail,
        string firstName,
        int driverId)
    {
        var subject = "Your RideGo Driver Account Has Been Approved";

        var htmlBody =
            MailUtils.GetDriverApprovedEmailHtml(
                firstName,
                driverId.ToString());

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }

    public async Task SendDriverRejectedEmailAsync(
        string toEmail,
        string firstName,
        int driverId,
        string reason)
    {
        var subject = "RideGo Driver Application Update";

        var htmlBody =
            MailUtils.GetDriverRejectedEmailHtml(
                firstName,
                driverId.ToString(),
                reason);

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }

    public async Task SendRideAcceptedEmailAsync(
        string toEmail,
        string passengerName,
        string driverName,
        string rideReference)
    {
        var subject = "Your RideGo Ride Has Been Accepted";

        var htmlBody =
            MailUtils.GetRideAcceptedEmailHtml(
                passengerName,
                driverName,
                rideReference);

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }

    public async Task SendDriverArrivedEmailAsync(
        string toEmail,
        string passengerName,
        string driverName,
        string rideReference)
    {
        var subject = "Your RideGo Driver Has Arrived";

        var htmlBody =
            MailUtils.GetDriverArrivedEmailHtml(
                passengerName,
                driverName,
                rideReference);

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }

    public async Task SendRideCancelledEmailAsync(
        string toEmail,
        string passengerName,
        string rideReference,  
        string cancelledBy,
        string reason)
    {
        var subject = "Your RideGo Ride Has Been Cancelled";

        var htmlBody =
            MailUtils.GetRideCancelledEmailHtml(
                passengerName,
                rideReference, 
                cancelledBy, 
                reason);

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }

    public async Task SendRideCompletedEmailAsync(
        string toEmail,
        string passengerName,
        string rideReference,
        string driverName)
    {
        var subject = "Your RideGo Ride Has Been Completed";

        var htmlBody =
            MailUtils.GetRideCompletedEmailHtml(
                passengerName,
                rideReference,
                driverName);

        var message = CreateBaseMessage(
            toEmail,
            subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }
    
    public async Task SendAccountDeactivatedEmailAsync(
        string toEmail,
        string firstName,
        string reason)
    {
        var subject = "Your RideGo Account Has Been Deactivated";

        var htmlBody = MailUtils.GetAccountDeactivatedEmailHtml(
            firstName,
            reason);

        var message = CreateBaseMessage(toEmail, subject);

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = htmlBody
        };

        await SendMimeMessageAsync(message);
    }
}