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
    public class AzureContentSafetyProvider : IContentSafetyProvider
    {
        private readonly AzureContentSafetyOptions _options;
        private readonly ContentSafetyClient? _client;

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

        public async Task<ContentModerationScanResultDto> AnalyzeText(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return new ContentModerationScanResultDto
                {
                    IsToxic = false,
                    SeverityThreshold = GetThreshold()
                };
            }

            if (_client == null)
                throw new InvalidOperationException("Azure Content Safety is not configured. Set AzureContentSafety:Endpoint and AzureContentSafety:Key.");

            var request = new AnalyzeTextOptions(content);
            foreach (var blocklistName in _options.BlocklistNames.Where(x => !string.IsNullOrWhiteSpace(x)))
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
                    .Where(x => x.Severity >= threshold)
                    .Select(x => $"{x.Category}:{x.Severity}")
                    .ToList();

                if (response.Value.BlocklistsMatch != null)
                {
                    matched.AddRange(response.Value.BlocklistsMatch
                        .Select(x => $"Blocklist:{x.BlocklistName}:{x.BlocklistItemText}")
                        .Where(x => !string.IsNullOrWhiteSpace(x)));
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
                throw new InvalidOperationException($"Azure Content Safety scan failed: {ex.ErrorCode} - {ex.Message}", ex);
            }
        }

        private int GetThreshold()
        {
            return Math.Clamp(_options.SeverityThreshold, 0, 7);
        }
    }
}