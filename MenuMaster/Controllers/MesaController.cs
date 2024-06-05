using MenuMaster.Data;
using MenuMaster.Models;
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
        public IActionResult Criar()
        {
            return View();
        }
        public IActionResult Bloquear(int id)
        {
            MenuItem item = _context.MenuItens.FirstOrDefault(x => x.Id == id);
            return View(item);
        }
        public IActionResult Abrir(int id)
        {
            Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
            return View(item);
        }
        public IActionResult Pedido(int id)
        {
            Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
            return View(item);
        }
        public IActionResult Finalizar(int id)
        {
            Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
            return View(item);
        }

        [HttpPost("Mesa/CriarMesa")]
        [ValidateAntiForgeryToken]
        public IActionResult CriarMesa([FromBody] Mesa model)
        {
            try
            {
                if (model.Numero <= 0)
                    throw new InvalidOperationException("Numero da mesa não pode ser menor ou igual a zero;warning");

                var mesaDb = _context.Mesas.FirstOrDefault(x => x.Numero == model.Numero);
                if (mesaDb is not null)
                    throw new InvalidOperationException("Mesa já cadastrada;warning");

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

        [HttpGet("Mesa/BloquearMesa/{id}")]
        public IActionResult BloquearMesa(int id)
        {
            try
            {
                Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
                if (item is null)
                    throw new InvalidOperationException("Não foi possivel encontrar a mesa, tente novamente;warning");

                item.Disponivel = false;
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

        [HttpGet("Mesa/ReativarMesa/{id}")]
        public IActionResult ReativarMesa(int id)
        {
            try
            {
                Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
                if (item is null)
                    throw new InvalidOperationException("Não foi possivel encontrar a mesa, tente novamente;warning");

                item.Disponivel = true;
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


