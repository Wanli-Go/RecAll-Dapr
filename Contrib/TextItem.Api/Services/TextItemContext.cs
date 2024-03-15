using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecAll.Contrib.TextItem.Api.Services;

public class TextItemContext : DbContext
{
    public DbSet<Models.TextItem> TextItems { get; set; }

    public TextItemContext(DbContextOptions<TextItemContext> options) : base(options)
    { }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new TextItemConfiguration());
    }
}

public class TextItemConfiguration : IEntityTypeConfiguration<Models.TextItem>
{
    public void Configure(EntityTypeBuilder<Models.TextItem> builder)
    {
        builder.ToTable("textitems"); //  Specifies that the TextItem class should be mapped to a database table named "textitems".
        
        builder.HasKey(p => p.Id); // Defines the "Id" property as the primary key.
        builder.Property(p => p.Id).UseHiLo("textitem_hilo"); 
        // Configures the primary key to use a HiLo algorithm for generating new key values. This is an efficient way to generate IDs, especially in distributed environments.

        builder.Property(p => p.ItemId).IsRequired(false);
        builder.HasIndex(p => p.ItemId).IsUnique();
        // Creates a unique index on the "ItemId" column. This ensures no two TextItems can have the same ItemId (enforces uniqueness if an ItemId is provided).

        builder.Property(p => p.Content).IsRequired();

        builder.Property(p => p.UserIdentityGuid).IsRequired();
        builder.HasIndex(p => p.UserIdentityGuid).IsUnique(false);

        builder.Property(p => p.IsDeleted).IsRequired();
    }
}

public class TextListContextDesignFactory : IDesignTimeDbContextFactory<TextItemContext>
{
    public TextItemContext CreateDbContext(string[] args)
    {
        return new(new DbContextOptionsBuilder<TextItemContext>()
            .UseSqlServer(
                "Server=.;Initial Catalog=RecAll.TextListDb;Integrated Security=true")
            .Options);
    }
}

