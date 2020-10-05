using System;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using SS.Db.models;
using tests.api.Helpers;

namespace tests.api.helpers
{
    /// <summary>
    /// Simple wrapper, that wraps our unit tests.
    /// </summary>
    public class WrapInTransactionScope : IDisposable
    {
        private readonly TransactionScope _scope;
        public bool CommitTxn { get; set; }

        protected readonly SheriffDbContext _dbContext;

        public WrapInTransactionScope(bool useMemoryDatabase)
        {
            _dbContext = new SheriffDbContext(EnvironmentBuilder.SetupDbOptions(useMemoryDatabase: useMemoryDatabase));
            _scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        }
        
        protected void Detach()
        {
            foreach (var entity in _dbContext.ChangeTracker.Entries())
                entity.State = EntityState.Detached;
        }

        public void Dispose()
        {
            if (CommitTxn) _scope.Complete();
            _scope.Dispose();
        }

    }
}
