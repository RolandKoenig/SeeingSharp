#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion

using SeeingSharp;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp.Util;

namespace SeeingSharp.Multimedia.Objects
{
    public class PalletType : ObjectType
    {
        private float m_width;
        private float m_depth;
        private float m_palletHeight;
        private float m_contentHeight;
        private float m_smallFooterWidth;
        private float m_bigFooterWidth;
        private float m_boardHeight;
        private Color4 m_contentColor;
        private Color4 m_palletColor;
        private NamedOrGenericKey m_palletMaterial;
        private NamedOrGenericKey m_contentMaterial;

        public PalletType(NamedOrGenericKey palletMaterial)
            : this(palletMaterial, NamedOrGenericKey.Empty, 0.8f, 1.2f, 0.144f, 0f, 0.10f, 0.145f, 0.022f)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PalletType" /> class.
        /// </summary>
        public PalletType()
            : this(NamedOrGenericKey.Empty, NamedOrGenericKey.Empty)
        {

        }

        /// <summary>
        /// Creates a new pallet type with default size
        /// </summary>
        public PalletType(NamedOrGenericKey palletMaterial, NamedOrGenericKey contentMaterial)
            : this(palletMaterial, contentMaterial, 0.8f, 1.2f, 0.144f, 1f, 0.10f, 0.145f, 0.022f)
        {

        }

        /// <summary>
        /// Creates a new pallet type with given size settings
        /// </summary>
        public PalletType(NamedOrGenericKey palletMaterial, NamedOrGenericKey contentMaterial, float width, float depth, float palletHeight, float contentHeight, float smallFooterWidth, float bigFooterWidth, float boardHeight)
        {
            m_width = width;
            m_depth = depth;
            m_palletHeight = palletHeight;
            m_contentHeight = contentHeight;
            m_bigFooterWidth = bigFooterWidth;
            m_smallFooterWidth = smallFooterWidth;
            m_boardHeight = boardHeight;

            m_palletMaterial = palletMaterial;
            m_contentMaterial= contentMaterial;

            m_palletColor = Color4.DarkGoldenrod;
            m_contentColor = Color4.Transparent;
        }

        /// <summary>
        /// Builds the structure needed for the pallet
        /// </summary>
        /// <param name="buildOptions">Some generic options for structure building</param>
        public override VertexStructure[] BuildStructure(StructureBuildOptions buildOptions)
        {
            bool createContent = m_contentHeight > 0f;
            
            // Prepare result array
            VertexStructure[] result = null;
            if (createContent) { result = new VertexStructure[2]; }
            else { result = new VertexStructure[1]; }

            //Build pallet
            #region -----------------------------------------------------------
            if (buildOptions.IsHighDetail)
            {
                float middleFront = m_width / 2f;
                float middleSide = m_depth / 2f;
                float middleFrontBegin = middleFront - m_bigFooterWidth / 2f;
                float middleSideBegin = middleSide - m_bigFooterWidth / 2f;
                float lastBeginSmall = m_width - m_smallFooterWidth;
                float lastBeginBig = m_depth - m_bigFooterWidth;
                float footerHeight = m_palletHeight - m_boardHeight * 3f;
                float quarterFrontBegin = ((m_bigFooterWidth / 2f) + ((middleFront - (m_bigFooterWidth / 2f)) / 2f)) - (m_smallFooterWidth / 2f);// +(middleFront / 2f - m_smallFooterWidth / 2f);
                float threeQuarterFrontBegin = middleFront + (middleFront - quarterFrontBegin - m_smallFooterWidth);//(middleFront / 2f) * 3f - m_smallFooterWidth / 2f;

                result[0] = new VertexStructure(408, 612);
                result[0].Material = m_palletMaterial;

                //Build 3 board on bottom
                result[0].BuildCube24V(new Vector3(0f, 0f, 0f), new Vector3(m_smallFooterWidth, m_boardHeight, m_depth), m_palletColor);
                result[0].BuildCube24V(new Vector3(middleFrontBegin, 0f, 0f), new Vector3(m_bigFooterWidth, m_boardHeight, m_depth), m_palletColor);
                result[0].BuildCube24V(new Vector3(lastBeginSmall, 0f, 0f), new Vector3(m_smallFooterWidth, m_boardHeight, m_depth), m_palletColor);

                //Build 9 footers
                result[0].BuildCubeSides16V(new Vector3(0f, m_boardHeight, 0f), new Vector3(m_smallFooterWidth, footerHeight, m_bigFooterWidth), m_palletColor);
                result[0].BuildCubeSides16V(new Vector3(0f, m_boardHeight, middleSideBegin), new Vector3(m_smallFooterWidth, footerHeight, m_bigFooterWidth), m_palletColor);
                result[0].BuildCubeSides16V(new Vector3(0f, m_boardHeight, lastBeginBig), new Vector3(m_smallFooterWidth, footerHeight, m_bigFooterWidth), m_palletColor);
                result[0].BuildCubeSides16V(new Vector3(middleFrontBegin, m_boardHeight, 0f), new Vector3(m_bigFooterWidth, footerHeight, m_bigFooterWidth), m_palletColor);
                result[0].BuildCubeSides16V(new Vector3(middleFrontBegin, m_boardHeight, middleSideBegin), new Vector3(m_bigFooterWidth, footerHeight, m_bigFooterWidth), m_palletColor);
                result[0].BuildCubeSides16V(new Vector3(middleFrontBegin, m_boardHeight, lastBeginBig), new Vector3(m_bigFooterWidth, footerHeight, m_bigFooterWidth), m_palletColor);
                result[0].BuildCubeSides16V(new Vector3(lastBeginSmall, m_boardHeight, 0f), new Vector3(m_smallFooterWidth, footerHeight, m_bigFooterWidth), m_palletColor);
                result[0].BuildCubeSides16V(new Vector3(lastBeginSmall, m_boardHeight, middleSideBegin), new Vector3(m_smallFooterWidth, footerHeight, m_bigFooterWidth), m_palletColor);
                result[0].BuildCubeSides16V(new Vector3(lastBeginSmall, m_boardHeight, lastBeginBig), new Vector3(m_smallFooterWidth, footerHeight, m_bigFooterWidth), m_palletColor);

                //Build boards above footers
                result[0].BuildCube24V(new Vector3(0f, m_boardHeight + footerHeight, 0f), new Vector3(m_width, m_boardHeight, m_bigFooterWidth), m_palletColor);
                result[0].BuildCube24V(new Vector3(0f, m_boardHeight + footerHeight, middleSideBegin), new Vector3(m_width, m_boardHeight, m_bigFooterWidth), m_palletColor);
                result[0].BuildCube24V(new Vector3(0f, m_boardHeight + footerHeight, lastBeginBig), new Vector3(m_width, m_boardHeight, m_bigFooterWidth), m_palletColor);

                //Build top boards
                float localYPos = m_palletHeight - m_boardHeight;
                result[0].BuildCube24V(new Vector3(0f, localYPos, 0f), new Vector3(m_bigFooterWidth, m_boardHeight, m_depth), m_palletColor);
                result[0].BuildCube24V(new Vector3(middleFrontBegin, localYPos, 0f), new Vector3(m_bigFooterWidth, m_boardHeight, m_depth), m_palletColor);
                result[0].BuildCube24V(new Vector3(m_width - m_bigFooterWidth, localYPos, 0f), new Vector3(m_bigFooterWidth, m_boardHeight, m_depth), m_palletColor);
                result[0].BuildCube24V(new Vector3(quarterFrontBegin, localYPos, 0f), new Vector3(m_smallFooterWidth, m_boardHeight, m_depth), m_palletColor);
                result[0].BuildCube24V(new Vector3(threeQuarterFrontBegin, localYPos, 0f), new Vector3(m_smallFooterWidth, m_boardHeight, m_depth), m_palletColor);
            }
            else
            {
                result[0] = new VertexStructure(24, 100);
                result[0].BuildCube24V(
                    new Vector3(0f, 0f, 0f), 
                    new Vector3(m_width, m_palletHeight, m_depth), 
                    m_palletColor);
            }
            #endregion -----------------------------------------------------------

            //Build content
            #region -----------------------------------------------------------
            if (createContent)
            {
                result[1] = new VertexStructure();
                result[1].Material = m_contentMaterial;
                result[1].BuildCubeSides16V(new Vector3(0f, m_palletHeight, 0f), new Vector3(m_width, m_contentHeight, m_depth), m_contentColor);
                result[1].BuildCubeTop4V(new Vector3(0f, m_palletHeight, 0f), new Vector3(m_width, m_contentHeight, m_depth), m_contentColor);
                result[1].BuildCubeBottom4V(new Vector3(0f, m_palletHeight, 0f), new Vector3(m_width, m_contentHeight, m_depth), m_contentColor);
            }
            #endregion -----------------------------------------------------------

            Matrix rotMatrix = Matrix.RotationY(EngineMath.RAD_90DEG);

            result[0].UpdateVerticesUsingRelocationBy(new Vector3(-m_width / 2f, 0f, -m_depth / 2f));
            result[0].CalculateTangentsAndBinormals();
            result[0].TransformVertices(rotMatrix);

            if (createContent)
            {
                result[1].UpdateVerticesUsingRelocationBy(new Vector3(-m_width / 2f, 0f, -m_depth / 2f));
                result[1].CalculateTangentsAndBinormals();
                result[1].TransformVertices(rotMatrix);
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the width of the object
        /// </summary>
        public float Width
        {
            get { return m_width; }
            set
            {
                if (m_width != value)
                {
                    m_width = value;

                    //base.RefreshStructure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the depth of the object
        /// </summary>
        public float Depth
        {
            get { return m_depth; }
            set
            {
                if (m_depth != value)
                {
                    m_depth = value;

                    //base.RefreshStructure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of a pallet
        /// </summary>
        public float PalletHeight
        {
            get { return m_palletHeight; }
            set
            {
                if (m_palletHeight != value)
                {
                    m_palletHeight = value;

                    //base.RefreshStructure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the content
        /// </summary>
        public float ContentHeight
        {
            get { return m_contentHeight; }
            set
            {
                if (m_contentHeight != value)
                {
                    m_contentHeight = value;

                    //base.RefreshStructure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the small footer
        /// </summary>
        public float SmallFooterWidth
        {
            get { return m_smallFooterWidth; }
            set
            {
                if (m_smallFooterWidth != value)
                {
                    m_smallFooterWidth = value;

                    //base.RefreshStructure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the big footer
        /// </summary>
        public float BigFooterWidth
        {
            get { return m_bigFooterWidth; }
            set
            {
                if (m_bigFooterWidth != value)
                {
                    m_bigFooterWidth = value;

                    //base.RefreshStructure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of a board
        /// </summary>
        public float BoardHeight
        {
            get { return m_boardHeight; }
            set
            {
                if (m_boardHeight != value)
                {
                    m_boardHeight = value;

                    //base.RefreshStructure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the material of the pallet.
        /// </summary>
        public NamedOrGenericKey PalletMaterial
        {
            get { return m_palletMaterial; }
            set { m_palletMaterial = value; }
        }

        /// <summary>
        /// Gets or sets the material of the content.
        /// </summary>
        public NamedOrGenericKey ContentMaterial
        {
            get { return m_contentMaterial; }
            set { m_contentMaterial= value; }
        }

        public Color4 ContentColor
        {
            get { return m_contentColor; }
            set { m_contentColor = value; }
        }

        public Color4 PalletColor
        {
            get { return m_palletColor; }
            set { m_palletColor = value; }
        }
    }
}