using System;
using System.ComponentModel.DataAnnotations.Schema;
using SS.Db.models.auth;

/// <summary>
/// Credit to PIMS for this. 
/// </summary>
namespace db.models
{
    public abstract class BaseEntity
    {
        #region Properties

        /// <summary>
        /// get/set - The foreign key to the user who created this entity.
        /// </summary>
        /// <value></value>
        public Guid? CreatedById { get; set; }

        /// <summary>
        /// get/set - The user who created this entity.
        /// </summary>
        /// <value></value>
        public User CreatedBy { get; set; }

        /// <summary>
        /// get/set - When this entity was created.
        /// </summary>
        /// <value></value>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// get/set - Who updated this entity last.
        /// </summary>
        /// <value></value>
        public Guid? UpdatedById { get; set; }

        /// <summary>
        /// get/set - The user updated this entity last.
        /// </summary>
        /// <value></value>
        public User UpdatedBy { get; set; }

        /// <summary>
        /// get/set - When this entity was updated.
        /// </summary>
        /// <value></value>
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// get/set - The concurrency row version.
        /// </summary>
        /// <value></value>
        public byte[] RowVersion { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a BaseEntity class.
        /// Initializes the default values.
        /// </summary>
        public BaseEntity()
        {
            CreatedOn = DateTime.UtcNow;
        }
        #endregion
    }
}
