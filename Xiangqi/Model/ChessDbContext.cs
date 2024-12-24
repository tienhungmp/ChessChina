using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiangqi.Model
{
    public class ChessDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ChessDbContext() : base(@"Server=DESKTOP-NC78VN5;Database=Chess;User Id=sa;Password=123;")
        {
        }
    }
}
