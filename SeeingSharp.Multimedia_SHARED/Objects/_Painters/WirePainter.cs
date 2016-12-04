using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;

// Namespace mappings
using D3D11 = SharpDX.Direct3D11;

namespace SeeingSharp.Multimedia.Objects
{
    public class WirePainter
    {
        #region All members required for painting
        private bool m_isValid;
        private LineRenderResources m_renderResources;
        private RenderState m_renderState;
        private Lazy<Matrix4x4> m_worldViewPojCreator;
        #endregion

        internal WirePainter(RenderState renderState, LineRenderResources renderResources)
        {
            m_isValid = true;
            m_renderResources = renderResources;
            m_renderState = renderState;

            m_worldViewPojCreator = new Lazy<Matrix4x4>(() => Matrix4x4.Transpose(renderState.ViewProj));
        }

        public void DrawLine(Vector3 start, Vector3 destination)
        {
            this.DrawLine(start, destination, Color4.Black);
        }

        public void DrawLine(Vector3 start, Vector3 destination, Color4 lineColor)
        {
            if (!m_isValid) { throw new SeeingSharpGraphicsException($"This {nameof(WirePainter)} is only valid in the rendering pass that created it!"); }

            // Load and render the given line
            using (D3D11.Buffer lineBuffer = GraphicsHelper.CreateImmutableVertexBuffer(m_renderState.Device, new Vector3[] { start, destination }))
            {
                m_renderResources.RenderLines(
                    m_renderState, m_worldViewPojCreator.Value, lineColor, lineBuffer, 2);
            }
        }

        public void DrawTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            this.DrawTriangle(point1, point2, point3, Color4.Black);
        }

        public void DrawTriangle(Vector3 point1, Vector3 point2, Vector3 point3, Color4 lineColor)
        {
            if (!m_isValid) { throw new SeeingSharpGraphicsException($"This {nameof(WirePainter)} is only valid in the rendering pass that created it!"); }

            Line[] lineData = new Line[]
            {
                new Line(point1, point2),
                new Line(point2, point3),
                new Line(point3, point1)
            };

            // Load and render the given lines
            using (D3D11.Buffer lineBuffer = GraphicsHelper.CreateImmutableVertexBuffer(m_renderState.Device, lineData))
            {
                m_renderResources.RenderLines(
                    m_renderState, m_worldViewPojCreator.Value, lineColor, lineBuffer, lineData.Length * 2);
            }
        }

        internal void SetInvalid()
        {
            m_isValid = false;
        }
    }
}
