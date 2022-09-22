using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StokTakip.Models;

namespace StokTakip.Data
{
    public class StokTakipContext : DbContext
    {
        public StokTakipContext (DbContextOptions<StokTakipContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>().OwnsOne(x => x.ImageData);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Stock> Stock { get; set; } = default!;
    }
}
