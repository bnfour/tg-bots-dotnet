using Bnfour.TgBots.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bnfour.TgBots.Contexts;

/// <summary>
/// Database context for the cat macro bot.
/// </summary>
public class CatMacroBotContext : DbContext
{
    /// <summary>
    /// The database set.
    /// </summary>
    public required DbSet<CatMacro> Images { get; set; }

    public CatMacroBotContext(DbContextOptions<CatMacroBotContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // set table name and PK
        // EF is nice enough to set up guid generation for us
        modelBuilder.Entity<CatMacro>()
            .ToTable("CatMacros")
            .HasKey(cm => cm.Id);

        // set other fields as required and unique
        modelBuilder.Entity<CatMacro>()
            .Property(cm => cm.Caption).IsRequired();
        modelBuilder.Entity<CatMacro>()
            .HasIndex(cm => cm.Caption).IsUnique();

        modelBuilder.Entity<CatMacro>()
            .Property(cm => cm.FileId).IsRequired();
        modelBuilder.Entity<CatMacro>()
            .HasIndex(cm => cm.FileId).IsUnique();

        modelBuilder.Entity<CatMacro>()
            .Property(cm => cm.FileUniqueId).IsRequired();
        modelBuilder.Entity<CatMacro>()
            .HasIndex(cm => cm.FileUniqueId).IsUnique();
    }
}
