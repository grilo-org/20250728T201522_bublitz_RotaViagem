using Microsoft.EntityFrameworkCore;
using RotaViagemAPI.Models;

namespace RotaViagemAPI.Data
{
    public class RotaContext : DbContext
    {
        public RotaContext(DbContextOptions<RotaContext> options) : base(options) { }

        public DbSet<Rota> Rotas { get; set; }
    }
}
