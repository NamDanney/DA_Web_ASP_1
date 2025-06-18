using DA_Web.DTOs.Common;
using DA_Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DA_Web.Services.Interfaces
{
    public interface ITourService
    {
        Task<ApiResponse<IEnumerable<Tour>>> GetAllToursAsync(int page, int pageSize);
        Task<ApiResponse<Tour>> GetTourByIdAsync(int id);
    }
}