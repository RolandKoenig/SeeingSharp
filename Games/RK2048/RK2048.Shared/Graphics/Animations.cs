#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
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
using SeeingSharp.Multimedia.Core;
using RK2048.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RK2048.Graphics
{
    internal static class Animations
    {
        /// <summary>
        /// Builds a FadeIn animation for the given tile.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="animBuilder">The sequence builder to be manipulated.</param>
        public static IAnimationSequenceBuilder<T> DoTileFadeIn<T>(
            this IAnimationSequenceBuilder<T> animBuilder)
            where T : SceneSpacialObject
        {
            return animBuilder
                .CallAction(() => animBuilder.TargetObject.Scaling = new Vector3(0.1f, 0.1f, 0.1f))
                .Scale3DTo(Vector3.One, TimeSpan.FromMilliseconds(Constants.TILE_ANIMATION_TIME_MS));
        }

        /// <summary>
        /// Builds an animation for moving the tile to a given empty location.
        /// </summary>
        /// <param name="animBuilder">The sequence builder to be manipulated.</param>
        public static IAnimationSequenceBuilder<ValueTile> MoveTileToEmptyLocation(
            this IAnimationSequenceBuilder<ValueTile> animBuilder,
            ValueTile[,] tileMap, int targetXCoord, int targetYCoord)
        {
            Vector3 targetWorldPos = ValueTile.CalculateWorldPosition(targetXCoord, targetYCoord);
            ValueTile targetObject = animBuilder.TargetObject;
            int previousXCoord = targetObject.CoordX;
            int previousYCoord = targetObject.CoordY;

            return animBuilder
                .Move3DTo(targetWorldPos, Constants.TILE_ANIMATION_TIME)
                .WaitFinished()
                .CallAction(() =>
                {
                    // Update all coordinates on the object and the tilemap
                    targetObject.CoordX = targetXCoord;
                    targetObject.CoordY = targetYCoord;
                    tileMap[targetXCoord, targetYCoord] = targetObject;

                    // Reset previous tile (if not done so before or if the tile has not change using other animation..)
                    if (tileMap[previousXCoord, previousYCoord] == targetObject)
                    {
                        tileMap[previousXCoord, previousYCoord] = null;
                    }

                    // Update current world position finally.
                    targetObject.UpdateWorldPosition();
                });
        }

        public static IAnimationSequenceBuilder<ValueTile> PreDeleteAnimation(
            this IAnimationSequenceBuilder<ValueTile> animBuilder)
        {
            ValueTile mergeTargetObject = animBuilder.TargetObject;
            return animBuilder
                .Scale3DTo(new Vector3(1.3f, 1.5f, 1.3f), Constants.TILE_ANIMATION_REMOVE_TIME)
                .ChangeFloatBy(() => mergeTargetObject.Opacity, (actValue) => mergeTargetObject.Opacity = actValue, -1.0f, Constants.TILE_ANIMATION_REMOVE_TIME);
        }

        /// <summary>
        /// Builds an animation for moving the tile to a given other's tiles location and merges them.
        /// </summary>
        /// <param name="animBuilder">The sequence builder to be manipulated.</param>
        /// <param name="targetTile">The target tile for the merge operation.</param>
        public static IAnimationSequenceBuilder<ValueTile> MoveTileAndMergeWithOther(
            this IAnimationSequenceBuilder<ValueTile> animBuilder,
            ValueTile[,] tileMap, ValueTile targetTile, int targetXCoord, int targetYCoord)
        {
            Vector3 targetWorldPos = ValueTile.CalculateWorldPosition(targetXCoord, targetYCoord);
            ValueTile mergeOriginObject = animBuilder.TargetObject;
            ValueTile mergeTargetObject = targetTile;
            int previousXCoord = mergeOriginObject.CoordX;
            int previousYCoord = mergeOriginObject.CoordY;

            // Start remove animation for the target tile (.. and remove it finally)
            TaskCompletionSource<object> removeTaskNotifier = new TaskCompletionSource<object>();
            TaskCompletionSource<object> moveTaskNotifier = new TaskCompletionSource<object>();
            mergeTargetObject.BuildAnimationSequence()
                .CallAction(() => removeTaskNotifier.SetResult(null))
                .WaitTaskFinished(moveTaskNotifier.Task)
                .PreDeleteAnimation()
                .WaitFinished()
                .CallAction(() =>
                {
                    // Remove reference on the tile's location if it is still stet to it
                    if (tileMap[targetXCoord, targetYCoord] == mergeTargetObject)
                    {
                        tileMap[targetXCoord, targetYCoord] = null;
                    }

                    // Remove this object from the scene
                    mergeTargetObject.Scene.ManipulateSceneAsync((manipulator) =>
                    {
                        manipulator.Remove(mergeTargetObject);
                    });
                })
                .Apply();

            // Start move and change animation for the origin tile
            return animBuilder
                .Move3DTo(targetWorldPos, Constants.TILE_ANIMATION_TIME)
                .Scale3DTo(new Vector3(0.95f, 0.95f, 0.95f), Constants.TILE_ANIMATION_TIME)
                .WaitFinished()
                .WaitTaskFinished(removeTaskNotifier.Task)
                .Scale3DTo(new Vector3(1f, 1f, 1f), Constants.TILE_ANIMATION_TIME)
                .CallAction(() =>
                {
                    try
                    {
                        // Update all coordinates on the object and the tilemap
                        mergeOriginObject.CoordX = targetXCoord;
                        mergeOriginObject.CoordY = targetYCoord;
                        tileMap[targetXCoord, targetYCoord] = mergeOriginObject;

                        // Reset previous tile (if not done so before..)
                        if (tileMap[previousXCoord, previousYCoord] == mergeOriginObject)
                        {
                            tileMap[previousXCoord, previousYCoord] = null;
                        }

                        // Update current world position finally.
                        mergeOriginObject.UpdateWorldPosition();

                        // Change the value on the tile
                        mergeOriginObject.CurrentID = mergeOriginObject.CurrentID + 1;
                        mergeOriginObject.ChangeGeometry(Constants.RES_GEO_TILES_BY_ID[mergeOriginObject.CurrentID]);
                    }
                    finally
                    {
                        moveTaskNotifier.SetResult(null);
                    }
                });
        }
    }
}
