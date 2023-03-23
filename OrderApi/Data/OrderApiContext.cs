using Microsoft.EntityFrameworkCore;
using SharedModels;
using Order = OrderApi.Models.Order;

namespace OrderApi.Data;

public class OrderApiContext : DbContext
{
    public OrderApiContext(DbContextOptions<OrderApiContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(order => order.OrderLines)
            .WithOne()
            .HasForeignKey(orderLine => orderLine.OrderId);
    }
}