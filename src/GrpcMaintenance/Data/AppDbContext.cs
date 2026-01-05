using Microsoft.EntityFrameworkCore;
using GrpcMaintenance.Models;

namespace GrpcMaintenance.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<MaintenanceOrder> MaintenanceOrders { get; set; }
        public DbSet<DetailMaintenance> DetailMaintenances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaintenanceOrder>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<MaintenanceOrder>()
                .HasOne(o => o.Equipment)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.EquipmentId);

            modelBuilder.Entity<DetailMaintenance>()
                .HasOne(d => d.MaintenanceOrder)
                .WithMany(o => o.Details)
                .HasForeignKey(d => d.MaintenanceOrderId);
        }
    }
}
