using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SS.Api.Helpers;
using SS.Api.Helpers.Extensions;

namespace SS.Api.services
{
    public class ChesEmailService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _authUrl;
        private readonly string _emailUrl;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly ILogger<ChesEmailService> _logger;
        private HttpClient HttpClient { get; }

        public ChesEmailService(IConfiguration configuration, HttpClient httpClient, ILogger<ChesEmailService> logger)
        {
            _clientId = configuration.GetNonEmptyValue("ChesEmailService:ClientId");
            _clientSecret = configuration.GetNonEmptyValue("ChesEmailService:Secret");
            _authUrl = configuration.GetNonEmptyValue("ChesEmailService:AuthUrl");
            _emailUrl = configuration.GetNonEmptyValue("ChesEmailService:EmailUrl");
            _senderEmail = configuration.GetNonEmptyValue("ChesEmailService:SenderEmail");
            _senderName = configuration.GetNonEmptyValue("ChesEmailService:SenderName");
            HttpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetEmailServiceToken()
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, _authUrl);
                requestMessage.Headers.Authorization = new BasicAuthenticationHeaderValue(
                    _clientId,
                    _clientSecret);
                requestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } });

                var response = await HttpClient.SendAsync(requestMessage);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("HttpWasn'tSuccess");

                var contents = await response.Content.ReadAsStringAsync();
                return contents;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error happened while trying to get EmailServiceToken.");
            }
            return null;
        }

        public async Task SendEmail(string body, string subject, string recipientEmail)
        {
            body.ThrowIfNullOrEmpty(nameof(body));
            subject.ThrowIfNullOrEmpty(nameof(subject));
            recipientEmail.ThrowIfNullOrEmpty(nameof(recipientEmail));

            var emailServiceToken = await GetEmailServiceToken();
            emailServiceToken.ThrowIfNullOrEmpty(nameof(emailServiceToken));

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, _emailUrl);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(emailServiceToken);

            //https://ches-master-9f0fbe-prod.pathfinder.gov.bc.ca/api/v1/docs#operation/postEmail
            var email = new
            {
                Bcc = new List<string>(),
                BodyType = "text",
                Body = body,
                Cc = new List<string>(),
                DelayTs = 0,
                Encoding = "utf-8",
                From = $"\"{_senderName}\" {_senderEmail}",
                Priority = "normal",
                Subject = subject,
                To = recipientEmail.Split(",").ToList(),
                Tag = new Guid()
            };

            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");

            var response = await HttpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("Email sent successfully.");
            }
        }
    }
}

