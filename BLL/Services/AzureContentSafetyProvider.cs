using Azure;
using Azure.AI.ContentSafety;
using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes i content safety provider operation.
    // Validates input parameters against null or empty values.
    public class AzureContentSafetyProvider : IContentSafetyProvider
    {
        private readonly AzureContentSafetyOptions _options;
        private readonly ContentSafetyClient? _client;

        // Initializes a new instance of AzureContentSafetyProvider with dependencies: options.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public AzureContentSafetyProvider(IOptions<AzureContentSafetyOptions> options)
        {
            _options = options.Value ?? new AzureContentSafetyOptions();

            if (!string.IsNullOrWhiteSpace(_options.Endpoint) && !string.IsNullOrWhiteSpace(_options.Key))
            {
                _client = new ContentSafetyClient(
                    new Uri(_options.Endpoint),
                    new AzureKeyCredential(_options.Key));
            }
        }

        // Executes analyze text operation.
        // Validates input parameters against null or empty values.
        public async Task<ContentModerationScanResultDto> AnalyzeText(string content)
        {
            if (string.IsNullOrWhiteSpace(content))  // Mandatory string argument is blank — fail fast
            {
                return new ContentModerationScanResultDto
                {
                    IsToxic = false,
                    SeverityThreshold = GetThreshold()
                };
            }

            if (_client == null)  // Entity not found — short-circuit with appropriate error result
                throw new InvalidOperationException("Azure Content Safety is not configured. Set AzureContentSafety:Endpoint and AzureContentSafety:Key.");  // Unexpected runtime state — propagate to global error handler

            var request = new AnalyzeTextOptions(content);
            foreach (var blocklistName in _options.BlocklistNames.Where(x => !string.IsNullOrWhiteSpace(x)))  // Filter records matching the predicate
            {
                request.BlocklistNames.Add(blocklistName.Trim());
            }

            request.HaltOnBlocklistHit = _options.HaltOnBlocklistHit;

            try
            {
                Response<AnalyzeTextResult> response = await _client.AnalyzeTextAsync(request);
                var threshold = GetThreshold();
                var categories = response.Value.CategoriesAnalysis
                    .Select(x => new ContentSafetyCategoryDto
                    {
                        Category = x.Category.ToString(),
                        Severity = x.Severity ?? 0
                    })
                    .ToList();

                var matched = categories
                    .Where(x => x.Severity >= threshold)  // Filter records matching the predicate
                    .Select(x => $"{x.Category}:{x.Severity}")
                    .ToList();

                if (response.Value.BlocklistsMatch != null)
                {
                    matched.AddRange(response.Value.BlocklistsMatch
                        .Select(x => $"Blocklist:{x.BlocklistName}:{x.BlocklistItemText}")
                        .Where(x => !string.IsNullOrWhiteSpace(x)));  // Filter records matching the predicate
                }

                return new ContentModerationScanResultDto
                {
                    IsToxic = matched.Count > 0,
                    MaxSeverity = categories.Count == 0 ? 0 : categories.Max(x => x.Severity),
                    SeverityThreshold = threshold,
                    MatchedTerms = matched,
                    Categories = categories
                };
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException($"Azure Content Safety scan failed: {ex.ErrorCode} - {ex.Message}", ex);  // Unexpected runtime state — propagate to global error handler
            }
        }

        // Executes get threshold operation.
        private int GetThreshold()
        {
            // Clamp the calculated value to the minimum and maximum accepted by this domain rule.
            return Math.Clamp(_options.SeverityThreshold, 0, 7);
        }
    }
}
