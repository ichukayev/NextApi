using System.Threading;
using System.Threading.Tasks;
using Abitech.NextApi.Server.EfCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Abitech.NextApi.Server.EfCore.DAL
{
    /// <summary>
    /// Base interface for NextApiDbContext
    /// </summary>
    public interface INextApiDbContext
    {
        /// <summary>
        /// Get specific entity set
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <returns>DbSet for Entity Type</returns>
        DbSet<T> Set<T>() where T : class;

        /// <summary>
        ///     <para>
        ///         Gets an <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry" /> for the given entity. The entry provides
        ///         access to change tracking information and operations for the entity.
        ///     </para>
        ///     <para>
        ///         This method may be called on an entity that is not tracked. You can then
        ///         set the <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State" /> property on the returned entry
        ///         to have the context begin tracking the entity in the specified state.
        ///     </para>
        /// </summary>
        /// <param name="entity"> The entity to get the entry for. </param>
        /// <returns> The entry for the given entity. </returns>
        EntityEntry Entry(object entity);

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        ///     <para>
        ///         Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///         that any asynchronous operations have completed before calling another method on this context.
        ///     </para>
        /// </remarks>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the
        ///     number of state entries written to the database.
        /// </returns>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Basic implementation for NextApiDbContext
    /// </summary>
    public abstract class NextApiDbContext : DbContext, INextApiDbContext
    {
        // system for sync
        /// <summary>
        /// Accessor to ColumnChangesLogs
        /// </summary>
        public DbSet<ColumnChangesLog> ColumnChangesLogs { get; set; }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = new CancellationToken())
        {
            // TODO: implement logic for ILoggedEntity and IColumnLoggedEntity
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <inheritdoc />
        protected NextApiDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ColumnChangesLog>(config =>
            {
                config.HasIndex("RowGuid", "TableName", "ColumnName")
                    .IsUnique();
                config.Property(p => p.RowGuid).IsRequired();
                config.Property(p => p.TableName).IsRequired();
                config.Property(p => p.ColumnName).IsRequired();
                config.Property(p => p.LastChangedOn);
            });
        }
    }
}
