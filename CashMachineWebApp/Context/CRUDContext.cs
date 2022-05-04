using CashMachineWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CashMachineWebApp.Context
{
    public class CRUDContext : DbContext
    {
        public CRUDContext(DbContextOptions<CRUDContext> options) : base(options)
        {

        }

        public DbSet<Cassette> Cassettes { get; set; }
        public DbSet<ATM> ATMs { get; set; }
    }
}
