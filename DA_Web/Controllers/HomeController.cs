using DA_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DA_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILocationService _locationService;
        private readonly ITourService _tourService;

        // Chúng ta s? inject các service c?n thi?t ?? l?y d? li?u
        public HomeController(ILocationService locationService, ITourService tourService)
        {
            _locationService = locationService;
            _tourService = tourService;
        }

        public async Task<IActionResult> Index()
        {
            // L?y m?t vài ?i?m ??n n?i b?t ?? hi?n th?
            // Gi? s? service có ph??ng th?c GetFeaturedLocationsAsync
            var featuredLocations = await _locationService.GetAllLocationsAsync(1, 6); // L?y 6 ??a ?i?m ??u tiên

            // Chúng ta s? truy?n d? li?u này t?i View
            ViewBag.FeaturedLocations = featuredLocations.Data;

            return View();
        }

        // Action cho trang Gi?i thi?u (About)
        public IActionResult About()
        {
            return View();
        }

        // Action cho trang Liên h? (Contact)
        public IActionResult Contact()
        {
            return View();
        }
    }
}