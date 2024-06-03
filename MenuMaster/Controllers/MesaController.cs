using MenuMaster.Data;
using Microsoft.AspNetCore.Mvc;

namespace MenuMaster.Controllers
{
    public class MesaController : Controller
    {
        private readonly RestauranteContext _context;
        public MesaController(RestauranteContext context)
        {
            _context = context;  
        }
        public IActionResult Index()
        {
            return View(_context.Mesas.ToList());
        }
    }
}
