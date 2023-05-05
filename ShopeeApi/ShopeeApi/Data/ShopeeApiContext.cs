using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopeeApi.Model;
using ShopeeApi.Models;

namespace ShopeeApi.Data
{
    public class ShopeeApiContext : DbContext
    {
        public ShopeeApiContext (DbContextOptions<ShopeeApiContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasMany(c => c.Carts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<InventoryModel>()
                .HasMany(c => c.Carts)
                .WithOne(p => p.Inventory)
                .HasForeignKey(p => p.InventoryId);

            modelBuilder.Entity<UserModel>()
                .HasMany(c => c.Forums)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<InventoryModel>()
                .HasMany(c => c.Forums)
                .WithOne(p => p.Inventory)
                .HasForeignKey(p => p.ItemId);

        }

        public DbSet<ShopeeApi.Model.UserModel> UserModel { get; set; } = default!;

        public DbSet<ShopeeApi.Model.InventoryModel>? InventoryModel { get; set; }

        public DbSet<ShopeeApi.Model.CartModel>? CartModel { get; set; }

        public DbSet<ShopeeApi.Model.ForumModel>? ForumModel { get; set; }

        public DbSet<ShopeeApi.Models.OrderModel>? OrderModel { get; set; }
    }
}
