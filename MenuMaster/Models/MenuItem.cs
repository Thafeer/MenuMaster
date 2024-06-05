namespace MenuMaster.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public string Tipo { get; set; }
        public bool Disponivel { get; set; }
    }
}
