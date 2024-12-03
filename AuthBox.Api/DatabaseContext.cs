using AuthBox.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthBox.Api;

public class DatabaseContext : DbContext
{
    #region DbSet
    public DbSet<User> Users { get; set; }
    #endregion

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
    {
        EntityEntry<TEntity> entityAdded = base.Add(entity);

        if (base.Database.CurrentTransaction is null)
        {
            base.SaveChanges();
        }

        return entityAdded;
    }
}
