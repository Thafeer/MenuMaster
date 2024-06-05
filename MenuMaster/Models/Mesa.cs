namespace MenuMaster.Models
{
    public class Mesa
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public bool Ocupada { get; set; }
        public bool Disponivel { get; set; }
        public int ClienteId { get; set;}
        public Cliente Cliente { get; set;}
    }
}
