using MenuMaster.Data;
using MenuMaster.Models;
using Microsoft.AspNetCore.Mvc;

namespace MenuMaster.Controllers
{
    public class MenuItensController : Controller
    {
        // Variavel privada somente leitura, obtendo meu contexto do banco de dados
        private readonly RestauranteContext _context;

        // Construir a controller, carregando minha variavel do contexto do banco de dados
        public MenuItensController(RestauranteContext context)
        {
            // Minha variavel privada, recebe a variavel contexto
            _context = context;
        }

        // Metodo para chamar minha página index, passando asp-action e asp-controller
        public IActionResult Index()
        {
            var itens = _context.MenuItens.OrderBy(x => x.Disponivel == false).ToList();
            return View(itens);
        }

        // Metodo para chamar minha página criar item do cardapio, passando asp-action e asp-controller
        public IActionResult Criar()
        {
            return View();
        }

        // Metodo para chamar minha página de editar item do cardapio
        // Recebendo o ID do item
        public IActionResult Editar(int id)
        {
            // Busco no meu banco de dados o Item com o ID passado
            MenuItem item = _context.MenuItens.FirstOrDefault(x => x.Id == id);
            return View(item);
        }

        // Metodo HTTP POS, com rota, para criar um novo item
        // Recebendo no corpo o objeto MenuItem
        [HttpPost("MenuItens/CriarItem")]
        [ValidateAntiForgeryToken]
        public IActionResult CriarItem([FromBody] MenuItem model)
        {
            try
            {
                // Faco a verificação do meu modelo de entrada, e retorno uma mensagem de alerta
                if (!ModelState.IsValid)
                    throw new InvalidOperationException("Não é possivel cadastrar o item, tente novamente;warning");

                // Realizo o cadastro do item no meu banco de dados
                _context.Add(model);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //Aqui recupero toda a mensagem de erro no sistema e divido em duas partes ou mais
                //Pegando como indicador de divisão ;
                var mensagem = ex.Message.Split(';');

                //Criei um pequeno objeto de estrutura para retornar o erro
                var estruturaRetorno = new
                {
                    DescricaoErro = mensagem[0],
                    TipoAlerta = (mensagem.Length > 1) ? mensagem[1] : "error"
                };
                return BadRequest(estruturaRetorno);
            }
            return Ok();
        }

        // Metodo HTTP POS, com rota, para editar item
        // Recebendo no corpo o objeto MenuItem
        [HttpPost("MenuItens/EditarItem")]
        [ValidateAntiForgeryToken]
        public IActionResult EditarItem([FromBody] MenuItem model)
        {
            try
            {
                // Busco no meu banco o item do menu com o ID passado
                MenuItem item = _context.MenuItens.FirstOrDefault(x => x.Id == model.Id);

                // Se o item não existir retorno uma mensagem de alerta
                if (item is null)
                    throw new InvalidOperationException("Não foi possivel encontrar o item, tente novamente;warning");

                // Faco a movimentação da disponibilidade e do preço e atualizdo no meu banco
                item.Disponivel = model.Disponivel;
                item.Preco  = model.Preco;

                _context.Update(item);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //Aqui recupero toda a mensagem de erro no sistema e divido em duas partes ou mais
                //Pegando como indicador de divisão ;
                var mensagem = ex.Message.Split(';');

                //Criei um pequeno objeto de estrutura para retornar o erro
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
