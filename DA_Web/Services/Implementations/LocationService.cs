using DA_Web.Data;
using DA_Web.DTOs.Common;
using DA_Web.Models;
using DA_Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DA_Web.Services.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _context;

        public LocationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<Location>>> GetAllLocationsAsync(int page, int pageSize)
        {
            try
            {
                // THÊM .Include(l => l.LocationImages) vào câu truy vấn
                var locations = await _context.Locations
                                              .Include(l => l.LocationImages) // Tải danh sách ảnh liên quan
                                              .OrderBy(l => l.Name)
                                              .Skip((page - 1) * pageSize)
                                              .Take(pageSize)
                                              .ToListAsync();

                return ApiResponse<IEnumerable<Location>>.SuccessResult(locations, "Locations retrieved successfully.");
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi (log the error) ở đây nếu cần
                return ApiResponse<IEnumerable<Location>>.ErrorResult($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Location>> GetLocationByIdAsync(int id)
        {
            try
            {
                var location = await _context.Locations.FindAsync(id);

                if (location == null)
                {
                    return ApiResponse<Location>.ErrorResult("Location not found.");
                }

                return ApiResponse<Location>.SuccessResult(location, "Location retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Location>.ErrorResult($"An error occurred: {ex.Message}");
            }
        }
    }
}