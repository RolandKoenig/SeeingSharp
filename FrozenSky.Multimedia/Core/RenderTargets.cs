using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace FrozenSky.Multimedia.Core
{
    internal struct RenderTargets
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargets"/> struct.
        /// </summary>
        /// <param name="colorBuffer">The color buffer.</param>
        /// <param name="depthStencilBuffer">The depth stencil buffer.</param>
        public RenderTargets(D3D11.RenderTargetView colorBuffer, D3D11.DepthStencilView depthStencilBuffer)
        {
            this.ColorBuffer = colorBuffer;
            this.DepthStencilBuffer = depthStencilBuffer;
            this.ObjectIDBuffer = null;
            this.NormalDepthBuffer = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargets"/> struct.
        /// </summary>
        /// <param name="colorBuffer">The color buffer.</param>
        /// <param name="depthStencilBuffer">The depth stencil buffer.</param>
        /// <param name="objectIDBuffer">The object identifier buffer.</param>
        public RenderTargets(D3D11.RenderTargetView colorBuffer, D3D11.DepthStencilView depthStencilBuffer, D3D11.RenderTargetView objectIDBuffer)
        {
            this.ColorBuffer = colorBuffer;
            this.DepthStencilBuffer = depthStencilBuffer;
            this.ObjectIDBuffer = objectIDBuffer;
            this.NormalDepthBuffer = null;
        }

        /// <summary>
        /// The default color output.
        /// </summary>
        internal D3D11.RenderTargetView ColorBuffer;

        /// <summary>
        /// The default depth-stencil output.
        /// </summary>
        internal D3D11.DepthStencilView DepthStencilBuffer;

        /// <summary>
        /// The ObjectID output buffer.
        /// </summary>
        internal D3D11.RenderTargetView ObjectIDBuffer;

        /// <summary>
        /// The normal/depth output buffer (processes the data for input on other postprocessing effects).
        /// </summary>
        internal D3D11.RenderTargetView NormalDepthBuffer;
    }
}
