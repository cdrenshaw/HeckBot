using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;

namespace HeckBot.Models
{
    public class HeckBotContext : DbContext
    {
        public DbSet<ShieldDetails> ShieldTimers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var temp = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "heckbot.db");
            optionsBuilder.UseSqlite("Data Source=heckbot.db");
        }
    }    
}
