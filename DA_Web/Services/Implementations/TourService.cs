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
    public class TourService : ITourService
    {
        private readonly ApplicationDbContext _context;

        public TourService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<Tour>>> GetAllToursAsync(int page, int pageSize)
        {
            try
            {
                var tours = await _context.Tours
                                          .OrderBy(t => t.Destination) // ĐÃ SỬA
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync();

                return ApiResponse<IEnumerable<Tour>>.SuccessResult(tours, "Tours retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Tour>>.ErrorResult($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Tour>> GetTourByIdAsync(int id)
        {
            try
            {
                var tour = await _context.Tours
                                         .Include(t => t.TourLocations)       // Lấy các bản ghi trong bảng nối
                                             .ThenInclude(tl => tl.Location) // Từ bảng nối, lấy thông tin Location
                                         .FirstOrDefaultAsync(t => t.Id == id);

                if (tour == null)
                {
                    return ApiResponse<Tour>.ErrorResult("Tour not found.");
                }

                return ApiResponse<Tour>.SuccessResult(tour, "Tour retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<Tour>.ErrorResult($"An error occurred: {ex.Message}");
            }
        }
    }
}