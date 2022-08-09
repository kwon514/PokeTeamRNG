using Microsoft.EntityFrameworkCore;
using poketeam_api.DbModels;

namespace poketeam_api
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
        }
        public DbSet<PokemonTeam> PokemonTeam { get; set; }

    }
}
