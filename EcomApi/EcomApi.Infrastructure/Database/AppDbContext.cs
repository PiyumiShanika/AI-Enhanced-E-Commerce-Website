using EcomApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> users { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Address> addresses { get; set; }
        public DbSet<product> products { get; set; }
        public DbSet<OrderProduct> ordersProducts { get; set; }
        public DbSet<UserProduct> usersProducts { get; set; }
        public DbSet<ProductCategory> productsCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(a => a.User_Id);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(a => a.Order_Id);


                //1 to n with order
                entity.HasOne(a => a.User)
                .WithMany(b => b.orders)
                .HasForeignKey(a => a.User_Id)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.Adress_Id);

                entity.HasOne(a => a.User)
                .WithMany(b => b.addresses)
                .HasForeignKey(a => a.User_Id)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<product>(entity =>
            {
                entity.HasKey(a => a.Product_Id);

                entity.HasOne(p => p.productCategory)
                .WithMany(a => a.products)
                .HasForeignKey(p => p.Category_Id)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(op => new { op.Order_Id, op.Product_Id });

                entity.HasOne(op => op.order)
                .WithMany(b => b.OrderProducts)
                .HasForeignKey(bp => bp.Order_Id)
                .OnDelete(DeleteBehavior.NoAction);


                entity.HasOne(op => op.product)
                .WithMany(b => b.OrderProducts)
                .HasForeignKey(bp => bp.Product_Id)
                .OnDelete(DeleteBehavior.NoAction);

            });

            modelBuilder.Entity<UserProduct>(entity =>
            {
                entity.HasKey(up => new { up.User_ID, up.Product_ID });

                entity.HasOne(up => up.user)
                .WithMany(p => p.UserProducts)
                .HasForeignKey(bp => bp.User_ID)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(up => up.product)
                .WithMany(p => p.UserProducts)
                .HasForeignKey(bp => bp.Product_ID)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(a => a.Category_Id);

            });

        }
    }
}
