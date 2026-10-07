using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearlerPlatform.Domain.UserInfo;
using Microsoft.EntityFrameworkCore;

namespace DearlerPlatform.Core.Core
{
    public class DearlerPlatformDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DearlerPlatformDbContext()
        {

        }
        public DearlerPlatformDbContext(DbContextOptions options) : base(options)
        {

        }

        //新写法
        // public DearlerPlatformDbContext(DbContextOptions<DearlerPlatformDbContext> options) : base(options)
        // {

        // }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=127.0.0.1;Database=TestDb;uid=sa;pwd=308213;TrustServerCertificate=true");
            }
        }

        //用于配置实体模型
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // entity.ToTable();
                entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(20)
                .HasComment("这是用户名");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}