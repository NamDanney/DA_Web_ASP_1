using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DA_Web.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string subfolder);
        void DeleteFile(string relativeFilePath);
    }
}