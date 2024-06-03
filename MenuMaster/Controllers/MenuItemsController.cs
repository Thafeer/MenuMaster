using MenuMaster.Data;
using MenuMaster.Models;
using Microsoft.AspNetCore.Mvc;

namespace MenuMaster.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly RestauranteContext _context;
        public MenuItemsController(RestauranteContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.MenuItems.OrderBy(x => x.Disponivel == false).ToList());
        }
        public IActionResult Criar()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
            MenuItem item = _context.MenuItems.FirstOrDefault(x => x.Id == id);
            return View(item);
        }

        [HttpPost("MenuItems/CriarItem")]
        [ValidateAntiForgeryToken]
        public IActionResult CriarItem([FromBody] MenuItem model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new InvalidOperationException("Não é possivel cadastrar o item, tente novamente;warning");

                _context.Add(model);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                var mensagem = ex.Message.Split(';');
                var estruturaRetorno = new
                {
                    DescricaoErro = mensagem[0],
                    TipoAlerta = (mensagem.Length > 1) ? mensagem[1] : "error"
                };
                return BadRequest(estruturaRetorno);
            }
            return Ok();
        }

        [HttpPost("MenuItems/EditarItem")]
        [ValidateAntiForgeryToken]
        public IActionResult EditarItem([FromBody] MenuItem model)
        {
            try
            {

                MenuItem item = _context.MenuItems.FirstOrDefault(x => x.Id == model.Id);
                if (item is null)
                    throw new InvalidOperationException("Não foi possivel encontrar o item, tente novamente;warning");

                item.Disponivel = model.Disponivel;
                item.Preco  = model.Preco;
                _context.Update(item);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                var mensagem = ex.Message.Split(';');
                var estruturaRetorno = new
                {
                    DescricaoErro = mensagem[0],
                    TipoAlerta = (mensagem.Length > 1) ? mensagem[1] : "error"
                };
                return BadRequest(estruturaRetorno);
            }
            return Ok();
        }
    }
}
