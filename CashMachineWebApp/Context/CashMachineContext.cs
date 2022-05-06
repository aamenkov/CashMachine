using CashMachineWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CashMachineWebApp.Context
{
    public class CashMachineContext : DbContext
    {
        public CashMachineContext(DbContextOptions<CashMachineContext> options) : base(options) { }
        public DbSet<Cassette> Cassettes { get; set; }
        public DbSet<ATM> ATMs { get; set; }
    }
}
