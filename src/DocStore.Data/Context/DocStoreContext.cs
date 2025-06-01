using DocStore.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace DocStore.Data.Context;

public class DocStoreContext(DbContextOptions<DocStoreContext> options) : DbContext(options)
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentPage> DocumentPages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocStoreContext).Assembly);
    }
}