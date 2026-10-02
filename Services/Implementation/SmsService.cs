using System.Net.Http.Headers;
using Ride_Hailing_API.Services.Interfaces;

namespace Ride_Hailing_API.Services.Implementation;

public class SmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;

    public SmsService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<SmsService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendSmsAsync(
        string recipientNumber,
        string message)
    {
        var baseUrl = _configuration["TelecomAbode:BaseUrl"];
        var apiKey = _configuration["TelecomAbode:ApiKey"];
        var subject = _configuration["TelecomAbode:Subject"];

        if (string.IsNullOrWhiteSpace(recipientNumber))
        {
            _logger.LogInformation(
                "SMS sending cancelled: Recipient phone number is empty");

            return;
        }

        if (string.IsNullOrWhiteSpace(baseUrl) ||
            string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogInformation(
                "TelecomAbode SMS sending cancelled: BaseUrl or ApiKey is empty");

            return;
        }

        // Normalize recipient phone number
        var rawNumbers = recipientNumber.Split(
            new[] { ',' },
            StringSplitOptions.RemoveEmptyEntries);

        var normalizeList = new List<string>();

        foreach (var num in rawNumbers)
        {
            var cleanPhone = num.Trim();

            if (cleanPhone.StartsWith("+234"))
            {
                cleanPhone = "0" + cleanPhone.Substring(4);
            }
            else if (cleanPhone.StartsWith("234"))
            {
                cleanPhone = "0" + cleanPhone.Substring(3);
            }
            else if (cleanPhone.StartsWith("+"))
            {
                cleanPhone = cleanPhone.Substring(1);
            }

            normalizeList.Add(cleanPhone);
        }

        if (normalizeList.Count == 0)
        {
            _logger.LogInformation(
                "TelecomAbode SMS sending cancelled: Phone number is empty");

            return;
        }

        var bulkTo = string.Join(",", normalizeList);

        var reqPayload = new Dictionary<string, string>
        {
            { "subject", subject ?? "RideGo" },
            { "bulkPhones", bulkTo },
            { "message", message }
        };

        try
        {
            var content = new FormUrlEncodedContent(reqPayload);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                baseUrl)
            {
                Content = content
            };

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    apiKey);

            request.Headers.Add(
                "Accept",
                "application/json");

            _logger.LogInformation(
                $"Sending SMS alert to {bulkTo} via TelecomAbode");

            var response = await _httpClient.SendAsync(request);

            var responseContent =
                await response.Content.ReadAsStringAsync();

            _logger.LogInformation(
                $"<======== Raw Response from TelecomAbode ========>\n{responseContent}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    $"SMS sent successfully to {bulkTo} via TelecomAbode");
            }
            else
            {
                _logger.LogInformation(
                    $"Failed to send SMS to {bulkTo} via TelecomAbode. " +
                    $"Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"An error occurred while sending SMS via TelecomAbode to {bulkTo}.");
        }
    }
}