using Crud.Net.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Crud.Net.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // we must define in DbContex to understad about them are tables in DB
        //and the migration show them if we not write this 2 lines the migration will be empty

        public DbSet<Genre> ?Genres { get; set; }
        public DbSet<Movie> ? Movies { get; set; }
    }

}