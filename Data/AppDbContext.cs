using Microsoft.EntityFrameworkCore;
using NotesApi.Models;

namespace NotesApi.Data;

public class AppDbContext : DbContext
{
    public DbSet<Note> Notes { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Note>()
            .HasOne(note => note.User)
            .WithMany(user => user.Notes)
            .HasForeignKey(note => note.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
