namespace MenuMaster.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int MesaId { get; set; }
        public Mesa Mesa { get; set;}
        public DateTime DataHora { get; set; }
        public List<PedidoItem> Itens { get; set; }
        public decimal Total { get; set; }
    }

    public class PedidoItem
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set;}
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco {get; set; }

    }
}
