using System;
using System.Transactions;

namespace tests.api.helpers
{
    /// <summary>
    /// Simple wrapper, that wraps our unit tests.
    /// </summary>
    public class WrapInTransactionScope : IDisposable
    {
        private TransactionScope scope;
        public bool CommitTxn { get; set; }

        public WrapInTransactionScope()
        {
            scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        }

        public void Dispose()
        {
            if (CommitTxn) scope.Complete();
            scope.Dispose();
        }
    }
}
