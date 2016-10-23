using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Infrastructure;
using Xunit;

namespace SeeingSharp.Tests
{
    public class InfrastructureTests
    {
        public const string TEST_CATEGORY = "SeeingSharp Infrastructure";

        public InfrastructureTests()
        {
            SeeingSharpApplication.InitializeForUnitTests();
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Exceptions_ExtractInformation()
        {
            Exception catchedEx = null;
            try
            {
                throw new FileNotFoundException("File not found", "Dummy.txt");
            }
            catch(Exception ex) { catchedEx = ex; }

            // Query for exception info
            ExceptionInfo exInfo = new ExceptionInfo(catchedEx);

            // Check information
            Assert.True(exInfo.ChildNodes.Count == 1);
            Assert.True(exInfo.ChildNodes[0].ChildNodes.Count > 5);
            Assert.True(exInfo.ChildNodes[0].ChildNodes.Any(
                (actNode) => actNode.PropertyName == nameof(catchedEx.Message)));
        }
    }
}
