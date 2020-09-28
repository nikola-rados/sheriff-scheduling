using db.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

//Thanks to PIMS for this. 
namespace SS.DB.Configuration
{
    /// <summary>
    /// BaseEntityConfiguration class, provides a way to configure base entity in the database.
    ///</summary>
    public abstract class BaseEntityConfiguration<TBase> : IEntityTypeConfiguration<TBase> where TBase : BaseEntity
    {
        #region Methods
        protected void BaseConfigure(EntityTypeBuilder<TBase> builder)
        {
            builder.HasOne(m => m.CreatedBy).WithMany().HasForeignKey(m => m.CreatedById).OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasOne(m => m.UpdatedBy).WithMany().HasForeignKey(m => m.UpdatedById).OnDelete(DeleteBehavior.ClientSetNull);
        }

        public virtual void Configure(EntityTypeBuilder<TBase> builder)
        {
            BaseConfigure(builder);
        }
        #endregion
    }
}