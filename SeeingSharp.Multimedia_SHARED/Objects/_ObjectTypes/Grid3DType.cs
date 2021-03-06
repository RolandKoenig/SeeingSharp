﻿#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
	Exception are projects where it is noted otherwhise.
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using System.Collections.Generic;
using System.Numerics;
using SeeingSharp.Multimedia.Objects;
using SeeingSharp;
using SeeingSharp.Util;
using System;

namespace SeeingSharp.Multimedia.Objects
{
    public class Grid3DType : ObjectType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Grid3DType" /> class.
        /// </summary>
        public Grid3DType()
        {
            this.GenerateGround = true;
            this.LineSmallDevider = 25f;
            this.LineBigDevider = 100f;
            this.TileWidth = 1f;
            this.TilesX = 10;
            this.TilesZ = 10;
            this.GroupTileCount = 5;
            this.BuildBackFaces = true;

            this.HighlightXZLines = false;
            this.ZLineHighlightColor = Color4.BlueColor;
            this.XLineHighlightColor = Color4.GreenColor;

            this.GroundColor = Color4.LightSteelBlue;
            this.LineColor = Color4.LightGray; 
        }

        /// <summary>
        /// Builds the structures.
        /// </summary>
        public override VertexStructure BuildStructure(StructureBuildOptions buildOptions)
        {
            VertexStructure result = new VertexStructure();

            // Calculate parameters
            Vector3 firstCoordinate = new Vector3(
                -((TilesX * TileWidth) / 2f),
                0f,
                -((TilesZ * TileWidth) / 2f));
            float tileWidthX = this.TileWidth;
            float tileWidthZ = this.TileWidth;
            float fieldWidth = tileWidthX * TilesX;
            float fieldDepth = tileWidthZ * TilesZ;
            float fieldWidthHalf = fieldWidth / 2f;
            float fieldDepthHalf = fieldDepth / 2f;

            int tileMiddleX = (TilesX % 2 == 0) && (this.HighlightXZLines) ? this.TilesX / 2 : 1;
            int tileMiddleZ = (TilesZ % 2 == 0) && (this.HighlightXZLines) ? this.TilesZ / 2 : 1;

            // Define lower ground structure
            if (this.GenerateGround)
            {
                VertexStructureSurface lowerGround = result.CreateSurface();
                lowerGround.EnableTextureTileMode(new Vector2(TileWidth, TileWidth));
                lowerGround.BuildRect4V(
                    new Vector3(-fieldWidthHalf, -0.01f, -fieldDepthHalf),
                    new Vector3(fieldWidthHalf, -0.01f, -fieldDepthHalf),
                    new Vector3(fieldWidthHalf, -0.01f, fieldDepthHalf),
                    new Vector3(-fieldWidthHalf, -0.01f, fieldDepthHalf),
                    new Vector3(0f, 1f, 0f),
                    this.GroundColor);
                lowerGround.Material = this.GroundMaterial;
            }

            // Define line structures
            VertexStructureSurface genStructureDefaultLine = result.CreateSurface();
            VertexStructureSurface genStructureGroupLine = result.CreateSurface();
            for (int actTileX = 0; actTileX < TilesX + 1; actTileX++)
            {
                Vector3 localStart = firstCoordinate + new Vector3(actTileX * tileWidthX, 0f, 0f);
                Vector3 localEnd = localStart + new Vector3(0f, 0f, tileWidthZ * TilesZ);

                Color4 actLineColor = this.LineColor;
                float devider = actTileX % this.GroupTileCount == 0 ? this.LineSmallDevider : this.LineBigDevider;
                if (this.HighlightXZLines && (actTileX == tileMiddleX))
                {
                    actLineColor = this.ZLineHighlightColor;
                    devider = this.LineSmallDevider;
                }

                VertexStructureSurface targetStruture = actTileX % this.GroupTileCount == 0 ? genStructureGroupLine : genStructureDefaultLine;
                targetStruture.BuildRect4V(
                    localStart - new Vector3(tileWidthX / devider, 0f, 0f),
                    localStart + new Vector3(tileWidthX / devider, 0f, 0f),
                    localEnd + new Vector3(tileWidthX / devider, 0f, 0f),
                    localEnd - new Vector3(tileWidthX / devider, 0f, 0f),
                    actLineColor);
                if(this.BuildBackFaces)
                {
                    targetStruture.BuildRect4V(
                        localEnd - new Vector3(tileWidthX / devider, 0f, 0f),
                        localEnd + new Vector3(tileWidthX / devider, 0f, 0f),
                        localStart + new Vector3(tileWidthX / devider, 0f, 0f),
                        localStart - new Vector3(tileWidthX / devider, 0f, 0f),
                        actLineColor);
                }
            }
            for (int actTileZ = 0; actTileZ < TilesZ + 1; actTileZ++)
            {
                Vector3 localStart = firstCoordinate + new Vector3(0f, 0f, actTileZ * tileWidthZ);
                Vector3 localEnd = localStart + new Vector3(tileWidthX * TilesX, 0f, 0f);

                Color4 actLineColor = this.LineColor;
                float devider = actTileZ % this.GroupTileCount == 0 ? this.LineSmallDevider : this.LineBigDevider;
                if (this.HighlightXZLines && (actTileZ == tileMiddleZ))
                {
                    actLineColor = this.XLineHighlightColor;
                    devider = this.LineSmallDevider;
                }

                VertexStructureSurface targetStruture = actTileZ % this.GroupTileCount == 0 ? genStructureGroupLine : genStructureDefaultLine;
                targetStruture.BuildRect4V(
                    localStart + new Vector3(0f, 0f, tileWidthZ / devider),
                    localStart - new Vector3(0f, 0f, tileWidthZ / devider),
                    localEnd - new Vector3(0f, 0f, tileWidthZ / devider),
                    localEnd + new Vector3(0f, 0f, tileWidthZ / devider),
                    actLineColor);
                if(this.BuildBackFaces)
                {
                    targetStruture.BuildRect4V(
                        localEnd + new Vector3(0f, 0f, tileWidthZ / devider),
                        localEnd - new Vector3(0f, 0f, tileWidthZ / devider),
                        localStart - new Vector3(0f, 0f, tileWidthZ / devider),
                        localStart + new Vector3(0f, 0f, tileWidthZ / devider),
                        actLineColor);
                }
            }
            genStructureDefaultLine.Material = this.LineMaterial;
            genStructureGroupLine.Material = this.LineMaterial;
            if (genStructureDefaultLine.CountTriangles == 0) { result.RemoveSurface(genStructureDefaultLine); }
            if (genStructureGroupLine.CountTriangles == 0) { result.RemoveSurface(genStructureGroupLine); }

            // Return all generated structures
            return result;
        }

        public NamedOrGenericKey LineMaterial
        {
            get;
            set;
        }

        public NamedOrGenericKey GroupLineMaterial
        {
            get;
            set;
        }

        public NamedOrGenericKey GroundMaterial
        {
            get;
            set;
        }

        public int GroupTileCount
        {
            get;
            set;
        }

        public Color4 LineColor
        {
            get;
            set;
        }

        public Color4 GroundColor
        {
            get;
            set;
        }

        public bool HighlightXZLines
        {
            get;
            set;
        }

        public Color4 XLineHighlightColor
        {
            get;
            set;
        }

        public Color4 ZLineHighlightColor
        {
            get;
            set;
        }

        public bool BuildBackFaces
        {
            get;
            set;
        }

        public float TileWidth
        {
            get;
            set;
        }

        public int TilesX
        {
            get;
            set;
        }

        public int TilesZ
        {
            get;
            set;
        }

        public bool GenerateGround
        {
            get;
            set;
        }

        public float LineSmallDevider
        {
            get;
            set;
        }

        public float LineBigDevider
        {
            get;
            set;
        }
    }
}