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

            Assert.True(exInfo.Properties.Any(
                (actProperty) => actProperty.Name == nameof(catchedEx.Message)));
            Assert.True(exInfo.Properties.Count > 5);
        }
    }
}
