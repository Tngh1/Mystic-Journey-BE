using System.Collections.Generic;

namespace BLL.Services
{
    // Initializes a new default instance of the AzureContentSafetyOptions class.
    public class AzureContentSafetyOptions
    {
        // Executes endpoint operation.
        public string? Endpoint { get; set; }
        // Executes key operation.
        public string? Key { get; set; }
        // Executes severity threshold operation.
        public int SeverityThreshold { get; set; } = 4;
        // Executes blocklist names operation.
        public List<string> BlocklistNames { get; set; } = new();
        // Executes halt on blocklist hit operation.
        public bool HaltOnBlocklistHit { get; set; } = true;
    }
}
