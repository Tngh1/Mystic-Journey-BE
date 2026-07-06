using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IContentSafetyProvider
    {
        Task<ContentModerationScanResultDto> AnalyzeText(string content);
    }
}