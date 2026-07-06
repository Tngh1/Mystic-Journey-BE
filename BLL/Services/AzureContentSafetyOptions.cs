using System.Collections.Generic;

namespace BLL.Services
{
    public class AzureContentSafetyOptions
    {
        public string? Endpoint { get; set; }
        public string? Key { get; set; }
        public int SeverityThreshold { get; set; } = 4;
        public List<string> BlocklistNames { get; set; } = new();
        public bool HaltOnBlocklistHit { get; set; } = true;
    }
}