using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IContentSafetyProvider class.
    public interface IContentSafetyProvider
    {
        Task<ContentModerationScanResultDto> AnalyzeText(string content);
    }
}
