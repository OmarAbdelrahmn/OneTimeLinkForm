using Microsoft.EntityFrameworkCore;

namespace OneTime;

public class AppDbcontext(DbContextOptions<AppDbcontext> options) : DbContext(options)
{
    public DbSet<FormResponse> FormResponses { get; set; }
    public DbSet<OneTimeLink> OneTimeLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OneTimeLink>(entity =>
        {
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.Token).IsRequired();
        });

        modelBuilder.Entity<FormResponse>(entity =>
        {
            entity.Property(e => e.TUserName).IsRequired();
            entity.Property(e => e.IsWorking).IsRequired();
        });
    }
}