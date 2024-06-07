using MenuMaster.Data;
using MenuMaster.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MenuMaster.Controllers
{
    public class MesaController : Controller
    {
        // Variavel privada somente leitura, obtendo meu contexto do banco de dados
        private readonly RestauranteContext _context;

        // Construir a controller, carregando minha variavel do contexto do banco de dados
        public MesaController(RestauranteContext context)
        {
            // Minha variavel privada, recebe a variavel contexto
            _context = context;
        }

        // Metodo para chamar minha página index, passando asp-action e asp-controller
        public IActionResult Index()
        {
            // Busco no meu banco de dados todas as mesas cadastradas e crio uma lista de Mesas
            var Mesas = _context.Mesas.ToList();

            // Leio esta lista, para validar algumas informações
            foreach (var mesa in Mesas)
            {
                // Verifico se esta mesa da lista está ocupada
                if (mesa.Ocupada)
                {
                    // Se ocupada, busco no meu banco o cliente que está na mesa
                    mesa.Cliente = _context.Clientes.FirstOrDefault(x => x.Id == mesa.ClienteId);  

                    // Busco os pedidos desta mesa, selecionando os que possui status diferente de pago
                    mesa.Pedidos = _context.Pedidos
                        .Where(x => x.MesaId == mesa.Id && x.Status.ToUpper() != "PAGO")
                        .ToList();

                    // Verifico se há pedidos para esta mesa
                    if (mesa.Pedidos is not null)
                    {
                        // Leio os pedidos desta mesa e pego o item do cardapio deste pedido
                        foreach (var item in mesa.Pedidos)
                        {
                            item.MenuItem = _context.MenuItens.FirstOrDefault(x => x.Id == item.MenuItemId);
                        }
                    }
                }
            }
            return View(Mesas);
        }

        // Metodo que retorna a minha página para criar uma nova Mesa
        public IActionResult Criar()
        {
            return View();
        }

        // Metodo que retorna a minha pagina de bloquear mesa, recebendo como parametro principal o ID da mesa
        public IActionResult Bloquear(int id)
        {
            // Busco no meu banco a primeira mesa que o ID recebido pela tela, seja igual ao armazenado no banco
            Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
            return View(item);
        }

        // Metodo que retorna minha pagina de abrir uma mesa, recebendo como parametro principal o ID da mesa
        public IActionResult Abrir(int id)
        {
            // Busco no meu banco a primeira mesa que o ID recebido pela tela, seja igual ao armazenado no banco
            Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
            return View(item);
        }

        // Metodo que retorna minha página para solicitar um pedido, recebendo como parametro o ID da mesa
        public IActionResult Pedido(int id)
        {
            // Busco os dados da mesa, baseado no ID recebido
            Mesa mesa = _context.Mesas.FirstOrDefault(x => x.Id == id);

            // Busco a lista de itens do cardápio, validando se há disponibilidade
            var itensMenu = _context.MenuItens.Where(x => x.Disponivel == true).ToList();

            // Crio um objeto para a view passando o retorno da mesa e a lista de Itens do cardapio
            var mesaComItens = new CardapioMesaModelView
            {
                Mesa = mesa,
                MenuItens = itensMenu.OrderBy(x => x.Tipo.ToUpper() == "BEBIDA").ToList(),
            };

            return View(mesaComItens);
        }

        // Metodo que retorna minha pagina para finalizar um pedido, passando como parametro o ID da mesa
        public IActionResult Finalizar(int id)
        {
            // Busco os dados da mesa, baseado no ID recebido
            Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
            return View(item);
        }

        // Metodo HTTP POST, com rota e validação token, para criar uma nova mesa
        // Recebendo no corpo um objeto Mesa
        [HttpPost("Mesa/CriarMesa")]
        [ValidateAntiForgeryToken]
        public IActionResult CriarMesa([FromBody] Mesa model)
        {
            try
            {
                // Verifico se o numero da mesa é igual o menor que zero, e retorno uma mensagem de alerta
                if (model.Numero <= 0)
                    throw new InvalidOperationException("Numero da mesa não pode ser menor ou igual a zero;warning");

                // Busco no meu banco se existe uma mesa cadastrada com este numero, se existir, retorno uma mensagem
                var mesaDb = _context.Mesas.FirstOrDefault(x => x.Numero == model.Numero);
                if (mesaDb is not null)
                    throw new InvalidOperationException("Mesa já cadastrada;warning");

                // Se não existir, adiciono a disponibilidade da mesa e salvo no meu banco de dados
                model.Disponivel = true;

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

        // Metodo HTTP GET, com rota, para bloquear a mesa
        // Recebendo na rota o ID
        [HttpGet("Mesa/BloquearMesa/{id}")]
        public IActionResult BloquearMesa(int id)
        {
            try
            {
                // Busco no meu banco se existe uma mesa cadastrada com este ID, se não existir, retorno uma mensagem
                Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
                if (item is null)
                    throw new InvalidOperationException("Não foi possivel encontrar a mesa, tente novamente;warning");

                // Se existir, coloco a disponibilidade da mesa como falso, e atualizo no meu banco
                item.Disponivel = false;

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

        // Metodo HTTP GET, com rota, para reativar a mesa
        // Recebendo na rota o ID
        [HttpGet("Mesa/ReativarMesa/{id}")]
        public IActionResult ReativarMesa(int id)
        {
            try
            {
                // Busco no meu banco se existe uma mesa cadastrada com este ID, se não existir, retorno uma mensagem
                Mesa item = _context.Mesas.FirstOrDefault(x => x.Id == id);
                if (item is null)
                    throw new InvalidOperationException("Não foi possivel encontrar a mesa, tente novamente;warning");

                // Se existir modifico a disponibilidade dela e atualizo no meu banco de dados
                item.Disponivel = true;

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

        // Metodo HTTP POST, com rota, para abrir a mesa
        // Recebendo na rota o ID e passando no corpo o objeto do Cliente
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

        // Metodo HTTP POST, com rota, para solicitar o pedido para mesa
        // Recebendo na rota o ID e passando no corpo o objeto do Pedido
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

                // Crio o objeto de pedido e transfiro os valores para ela
                Pedido pedido = new Pedido();

                pedido.Mesa = mesa;
                pedido.MesaId = mesa.Id;
                pedido.MenuItem = item;
                pedido.MenuItemId = model.ItemId;
                pedido.Quantidade = model.Quantidade;
                pedido.DataHora = DateTime.Now;
                pedido.Preco = item.Preco;
                pedido.Total = item.Preco * model.Quantidade;

                // Verifico se o tipo é igual a COMIDA, e faço a inserção do status
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

        // Metodo HTTP GET, com rota, para colocar o pedido em andamento
        // Recebendo na rota o ID
        [HttpGet("Mesa/PedidoAndamento/{id}")]
        public IActionResult PedidoAndamento(int id)
        {
            try
            {
                // Busco o pedido no meu banco de dados passando o ID recebido
                Pedido pedido = _context.Pedidos.FirstOrDefault(p => p.Id == id);

                // Se o pedido não existir, eu retorno uma mensagem de alerta
                if (pedido is null)
                    throw new InvalidOperationException("Não foi possivel encontrar o pedido, tente novamente;warning");

                // Busco a mesa deste pedido no meu banco, passando a mesaID do pedido acima
                Mesa mesa = _context.Mesas.FirstOrDefault(x => x.Id == pedido.MesaId);

                // Se a mesa não existir, retorno uma mensagem de alerta
                if (mesa is null)
                    throw new InvalidOperationException("Não foi possivel encontrar a mesa, tente novamente;warning");

                // Verifico o status do pedido e faço a alteração para andamento, e atualizo o pedido no meu banco
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

        // Metodo HTTP GET, com rota, para colocar o finalizar o pedido
        // Recebendo na rota o ID
        [HttpGet("Mesa/EntregarPedido/{id}")]
        public IActionResult EntregarPedido(int id)
        {
            try
            {
                // Busco o pedido no meu banco de dados passando o ID recebido
                Pedido pedido = _context.Pedidos.FirstOrDefault(p => p.Id == id);

                // Se o pedido não existir, eu retorno uma mensagem de alerta
                if (pedido is null)
                    throw new InvalidOperationException("Não foi possivel encontrar o pedido, tente novamente;warning");
                
                // Busco a mesa deste pedido no meu banco, passando a mesaID do pedido acima
                Mesa mesa = _context.Mesas.FirstOrDefault(x => x.Id == pedido.MesaId);

                // Se a mesa não existir, retorno uma mensagem de alerta
                if (mesa is null)
                    throw new InvalidOperationException("Não foi possivel encontrar a mesa, tente novamente;warning");

                // Verifico o status do pedido se contem em andamento, e mudo o status para finalizado, e atualizo no meu banco
                if (pedido.Status.ToUpper().Contains("EM ANDAMENTO"))
                {
                    pedido.Status = "Finalizado";
                }

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
    }
}


