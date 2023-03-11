using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShinyAspNet.Data;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions opts) : base(opts) { }


    public DbSet<User> Users => this.Set<User>();
    public DbSet<RefreshToken> RefreshTokens => this.Set<RefreshToken>();


    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder
            .Properties<string>()
            .AreUnicode(true)
            .HaveMaxLength(50);
    }


    static void Map(
        EntityTypeBuilder defaultType,
        string tableName,
        string primaryKey,
        string pkPropertyName = "Id"
    )
    {
        defaultType.ToTable(tableName);
        defaultType.HasKey(pkPropertyName);
        defaultType.Property(pkPropertyName).HasColumnName(primaryKey);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        Map(modelBuilder.Entity<CodeVerification>());
        Map(modelBuilder.Entity<User>());
        Map(modelBuilder.Entity<DispatchRequest>());
        Map(modelBuilder.Entity<RefreshToken>());
    }


    static void Map(EntityTypeBuilder<RefreshToken> type)
    {
        Map(type, "RefreshTokens", "RefreshTokenIdentifier");

        type.Property(x => x.Id).HasMaxLength(512);
        type.Property(x => x.DateCreated);

        type.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }


    static void Map(EntityTypeBuilder<User> type)
    {
        Map(type, "Users", "UserId");

        type.HasIndex(x => x.Email).IsUnique();
        type.Property(x => x.Email);
        type.Property(x => x.FirstName);
        type.Property(x => x.LastName);
    }
}