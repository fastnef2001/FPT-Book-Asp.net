using important1.Areas.Identity.Data;
using important1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace important1.Areas.Identity.Data;

public class UserContext : IdentityDbContext<important1User>
{
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Helper> Helpers { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Book>()
            .HasOne<important1User>(b => b.IdNavigation)
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.Id);

        builder.Entity<Book>()
            .HasOne<Category>(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId);

        builder.Entity<Order>()
            .HasOne<important1User>(o => o.IdNavigation)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.Id);

        builder.Entity<OrderDetail>()
            .HasKey(od => new { od.OrderId, od.Isbn });

        builder.Entity<OrderDetail>()
            .HasOne<Order>(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.NoAction);


        builder.Entity<OrderDetail>()
            .HasOne<Book>(od => od.IsbnNavigation)
            .WithMany(b => b.OrderDetails)
            .HasForeignKey(od => od.Isbn)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Cart>()
            .HasKey(c => new { c.UId, c.BookIsbn });

        builder.Entity<Cart>()
            .HasOne<important1User>(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UId);

        builder.Entity<Cart>()
            .HasOne<Book>(c => c.Book)
            .WithMany(b => b.Carts)
            .HasForeignKey(c => c.BookIsbn)
            .OnDelete(DeleteBehavior.NoAction);


    }
}
