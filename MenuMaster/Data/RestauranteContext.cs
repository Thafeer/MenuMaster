using MenuMaster.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuMaster.Data
{
    public class RestauranteContext : DbContext
    {
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set;}
        public DbSet<Cliente> Clientes { get; set;}

        public RestauranteContext(DbContextOptions<RestauranteContext> options) : base(options) { }
    }
}
