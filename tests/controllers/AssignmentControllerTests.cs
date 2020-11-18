using System;
using System.Collections.Generic;
using System.Text;
using SS.Api.controllers.scheduling;
using SS.Api.services.scheduling;
using tests.api.helpers;
using tests.api.Helpers;
using Xunit;

namespace tests.controllers
{
    //Sequential, because there are issues with Adding Location (with a unique index) within a TransactionScope.
    [Collection("Sequential")]
    public class AssignmentControllerTests : WrapInTransactionScope
    {
        #region Fields
        private readonly AssignmentController _controller;
        #endregion Fields

        public AssignmentControllerTests() : base(false)
        {
            _controller = new AssignmentController(new AssignmentService(Db), Db)
            {
                ControllerContext = HttpResponseTest.SetupMockControllerContext()
            };
        }

    }
}
