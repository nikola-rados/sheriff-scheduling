using System;
using System.Transactions;

namespace tests.api.helpers
{
    /// <summary>
    /// Simple wrapper, that wraps our unit tests.
    /// </summary>
    public class WrapInTransactionScope : IDisposable
    {
        private readonly TransactionScope _scope;
        public bool CommitTxn { get; set; }

        public WrapInTransactionScope()
        {
            _scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        }

        public void Dispose()
        {
            if (CommitTxn) _scope.Complete();
            _scope.Dispose();
        }
    }
}
