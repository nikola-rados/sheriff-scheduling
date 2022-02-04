using SS.Api.infrastructure.exceptions;
using SS.Db.models;
using SS.Db.models.sheriff;
using System;
using System.Linq;
using System.Threading.Tasks;
using SS.Api.helpers.extensions;

namespace SS.Api.services.usermanagement
{
    public class SheriffRankService
    {
        #region Properties
        private SheriffDbContext Db { get; }
        #endregion

        #region Constructor
        public SheriffRankService(SheriffDbContext db) { Db = db; }
        #endregion

        #region Helpers
        private void CheckForOverlap(SheriffRank rank)
        {
            if (Db.SheriffRank.Any(x => x.Id != rank.Id && x.SheriffId == rank.SheriffId && x.EffectiveDate <= rank.EffectiveDate &&
                                 (x.ExpiryDate >= rank.ExpiryDate || x.ExpiryDate == null)))
                throw new BusinessLayerException("Overlap detected for Rank.");
        }
        #endregion

        #region Methods
        public async Task<SheriffRank> AssignSheriffRank(SheriffRank rank)
        {
            CheckForOverlap(rank);

            await Db.SheriffRank.AddAsync(rank);
            await Db.SaveChangesAsync();
            return rank;
        }

        public async Task<SheriffRank> UpdateSheriffRank(SheriffRank rank)
        {
            CheckForOverlap(rank);

            var savedSheriffRank = await Db.SheriffRank.FindAsync(rank.Id);
            savedSheriffRank.ThrowBusinessExceptionIfNull($"{nameof(SheriffRank)} with the id: {rank.Id} could not be found. ");

            Db.Entry(savedSheriffRank).CurrentValues.SetValues(rank);
            Db.Entry(savedSheriffRank).Property(x => x.Id).IsModified = false;
            Db.Entry(savedSheriffRank).Property(x => x.SheriffId).IsModified = false;

            await Db.SaveChangesAsync();
            return savedSheriffRank;
        }
        #endregion
    }
}
