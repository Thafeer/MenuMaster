using MenuMaster.Data;
using MenuMaster.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            var Mesas = _context.Mesas.ToList();
            foreach (var mesa in Mesas)
            {
                if (mesa.Ocupada)
                {
                    mesa.Cliente = _context.Clientes.FirstOrDefault(x => x.Id == mesa.ClienteId);  
                    mesa.Pedidos = _context.Pedidos
                        .Where(x => x.MesaId == mesa.Id && x.Status.ToUpper() != "PAGO")
                        .ToList();

                    if (mesa.Pedidos is not null)
                    {
                        foreach (var item in mesa.Pedidos)
                        {
                            item.MenuItem = _context.MenuItens.FirstOrDefault(x => x.Id == item.MenuItemId);
                        }
                    }
                }
            }
            return View(Mesas);
        }
        public IActionResult Criar()
        {
            return View();
        }
        public IActionResult Bloquear(int id)
        {
            Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
            return View(item);
        }
        public IActionResult Abrir(int id)
        {
            Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
            return View(item);
        }
        public IActionResult Pedido(int id)
        {
            // Busco os dados da mesa, baseado no ID recebido
            Mesa mesa = _context.Mesas.FirstOrDefault(x => x.Id == id);

            // Busco a lista de itens do cardápio, validando se há disponibilidade
            var itensMenu = _context.MenuItens.Where(x => x.Disponivel == true).ToList();

            // Crio um objeto para a view passando o retorno da mesa e a lista de objeto
            var mesaComItens = new CardapioMesaModelView
            {
                Mesa = mesa,
                MenuItens = itensMenu.OrderBy(x => x.Tipo.ToUpper() == "BEBIDA").ToList(),
            };

            return View(mesaComItens);
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
                model.Disponivel = true;
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

        [HttpPost("Mesa/AbrirMesa/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult AbrirMesa(int id, [FromBody] Cliente model)
        {
            try
            {
                // Faço a tratativa inicial, se o ID for 0 ou menor, esta mesa não existe então não prossigo com a abertura
                if (id <= 0)
                    throw new InvalidOperationException("Numero da mesa não pode ser menor ou igual a zero;warning");

                //Com o ID correto, vou na tabela Clientes
                //pegando o primeiro dado em que o Nome no banco for igual ao Nome enviado pelo usuario
                var clienteDb = _context.Clientes.FirstOrDefault(x => x.Nome == model.Nome);

                // Aqui irei buscar o dado da mesa, pelo ID passado pelo usuario
                var mesaDb = _context.Mesas.FirstOrDefault(x => x.Id == id);

                //Faço a verificação, pois se não encontrar houve um erro no processo
                if (mesaDb is null)
                    throw new InvalidOperationException("Mesa não encontrada, tente novamente;warning");


                //Faço a verificação, se a variavel for nula com a pesquisa anterior, irei cadastrar este cliente no banco
                //Se a variavel não for nula, eu sigo o processo de abertura da mesa para o cliente
                if (clienteDb is null)
                {
                    _context.Clientes.Add(model);
                    _context.SaveChanges();
                }

               
                // Aqui irei fazer a movimentação do ID do cliente, para a mesa onde está, e marcando ela como ocupada
                mesaDb.ClienteId = model.Id;
                mesaDb.Ocupada = true;

                //Realizo o update no banco de dados da mesa, passando o cliente e o status de ocupada
                _context.Update(model);
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

        [HttpPost("Mesa/SolicitarPedido/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult SolicitarPedido(int id, [FromBody] PedidoMesa model)
        {
            try
            {
                // Faço a tratativa inicial, se o ID for 0 ou menor, esta mesa não existe então não prossigo com a abertura
                if (id <= 0)
                    throw new InvalidOperationException("Numero da mesa não pode ser menor ou igual a zero;warning");

                //Faço a busca da mesa passando o ID
                var mesa = _context.Mesas.FirstOrDefault(m => m.Id == id);

                //Com o ID correto, vou na tabela Itens
                //pegando o primeiro dado em que o ID no banco for igual ao ID enviado pelo usuario
                var item = _context.MenuItens.FirstOrDefault(x => x.Id == model.ItemId);

                Pedido pedido = new Pedido();

                pedido.Mesa = mesa;
                pedido.MesaId = mesa.Id;
                pedido.MenuItem = item;
                pedido.MenuItemId = model.ItemId;
                pedido.Quantidade = model.Quantidade;
                pedido.DataHora = DateTime.Now;
                pedido.Preco = item.Preco;
                pedido.Total = item.Preco * model.Quantidade;
                if (item.Tipo.ToUpper() == "COMIDA")
                {
                    pedido.Status = "Cozinha";
                }
                else
                {
                    pedido.Status = "Copa";
                }

                //Realizo o update no banco de dados do pedido, passando os dados preenchidos acima
                _context.Update(pedido);
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

        [HttpGet("Mesa/PedidoAndamento/{id}")]
        public IActionResult PedidoAndamento(int id)
        {
            try
            {
                Pedido pedido = _context.Pedidos.FirstOrDefault(p => p.Id == id);

                if (pedido == null)
                    throw new InvalidOperationException("Não foi possivel encontrar o pedido, tente novamente;warning");

                Mesa mesa = _context.Mesas.FirstOrDefault(x => x.Id == pedido.MesaId);
                if (mesa is null)
                    throw new InvalidOperationException("Não foi possivel encontrar a mesa, tente novamente;warning");

                if (pedido.Status.ToUpper() == "COZINHA")
                {
                    pedido.Status = "Em Andamento Cozinha";
                }
                else
                {
                    pedido.Status = "Em Andamento Copa";
                }

                _context.Update(pedido);
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

        [HttpGet("Mesa/EntregarPedido/{id}")]
        public IActionResult EntregarPedido(int id)
        {
            try
            {
                Pedido pedido = _context.Pedidos.FirstOrDefault(p => p.Id == id);

                if (pedido == null)
                    throw new InvalidOperationException("Não foi possivel encontrar o pedido, tente novamente;warning");

                Mesa mesa = _context.Mesas.FirstOrDefault(x => x.Id == pedido.MesaId);
                if (mesa is null)
                    throw new InvalidOperationException("Não foi possivel encontrar a mesa, tente novamente;warning");

                if (pedido.Status.ToUpper().Contains("EM ANDAMENTO"))
                {
                    pedido.Status = "Finalizado";
                }

                _context.Update(pedido);
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


