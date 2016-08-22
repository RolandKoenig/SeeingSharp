using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SeeingSharpModelViewer.Tests
{
    public class ViewModelTests : IDisposable
    {
        public const string TEST_CATEGORY = "SeeingSharp ModelViewer ViewModel";

        private MemoryRenderTarget m_memRenderTarget;


        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelTests"/> class.
        /// </summary>
        public ViewModelTests()
        {
            
        }

        public void Dispose()
        {
          
        }

        [Fact]
        [Trait("Category", TEST_CATEGORY)]
        public void Check_Something()
        {

        }


    }
}
