using DA_Web.DTOs.Common;
using DA_Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DA_Web.Services.Interfaces
{
    public interface ILocationService
    {
        // Chúng ta sẽ định nghĩa một phương thức để lấy danh sách địa điểm
        // Tôi sẽ tạm dùng một DTO (Data Transfer Object) đơn giản để trả về
        Task<ApiResponse<IEnumerable<Location>>> GetAllLocationsAsync(int page, int pageSize);

        // Và một phương thức để lấy chi tiết một địa điểm
        Task<ApiResponse<Location>> GetLocationByIdAsync(int id);
    }
}