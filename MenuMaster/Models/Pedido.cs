namespace MenuMaster.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int? MesaId { get; set; }
        public Mesa Mesa { get; set;}
        public DateTime DataHora { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }

    public class PedidoMesa
    {
        public int ItemId { get; set; }
        public int Quantidade { get; set; }
    }
}
