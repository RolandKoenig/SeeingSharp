#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Threading.Tasks;
using SeeingSharp.Util;
using SeeingSharp.Multimedia.Views;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Drawing3D;
using SeeingSharp;
using RK2048.Graphics;
using SeeingSharp.Multimedia.Objects;
using RK2048.Data;

#if UNIVERSAL
using Windows.ApplicationModel;
#endif

namespace RK2048.Logic
{
    internal class GameCore : ViewModelBase
    {
        // Objects for painting
        private Scene m_scene;
        private PerspectiveCamera3D m_camera;
        private float m_lastCameraAspectRatio;

        // Objects for current data
        private ValueTile[,] m_gameTiles;
        private int m_currentScore;
        private int m_maximumReachedScore;

        // Misc
        private int m_startedAnimationsCounter;
        private Random m_randomizer;

        // Some ui specific parameters
        private bool m_menuEnabled;

        public GameCore()
        {
            // Initialize game field
            m_gameTiles = new ValueTile[4, 4];

            // Prepare scene and camera
            m_scene = new Scene();
            m_camera = new PerspectiveCamera3D();

            m_randomizer = new Random(Environment.TickCount);

            // Define commands
            this.CommandRestart = new DelegateCommand(() =>
                {
                    if (this.IsAnyTaskRunning()) { return; }
                    RestartGameAsync().FireAndForget();
                });
            this.CommandWeb = new DelegateCommand(() =>
            {
#if UNIVERSAL
                Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.rolandk.de/wp"))
                    .AsTask().FireAndForget();
#endif
            });
        }

        /// <summary>
        /// Returns true if there is any task running.
        /// </summary>
        public bool IsAnyTaskRunning()
        {
            return m_startedAnimationsCounter > 0;
        }

        /// <summary>
        /// Updates current camera location.
        /// </summary>
        public void UpdateCameraLocation()
        {
            // Update the camera's zoom level
            if (m_camera.AspectRatio != m_lastCameraAspectRatio)
            {
                m_lastCameraAspectRatio = m_camera.AspectRatio;

                if (m_lastCameraAspectRatio == 1.6f)
                {
                    m_camera.Position = new Vector3(0f, 8.5f, -8f);
                    m_camera.Target = new Vector3(0f, 0f, 0f);
                    m_camera.UpdateCamera();
                }
                else
                {
                    float changedBy = m_lastCameraAspectRatio / 1.6f;
                    m_camera.Position = new Vector3(0f, 8.5f / changedBy, -8f / changedBy);
                    m_camera.Target = new Vector3(0f, 0f, 0f);
                    m_camera.UpdateCamera();
                }
            }
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        internal async Task InitializeAsync(ISeeingSharpPainter view)
        {
            // Set scene and camera
            view.Scene = m_scene;
            view.Camera = m_camera;

#if UNIVERSAL
            App.Current.Suspending += OnApp_Suspending;
#endif

            m_startedAnimationsCounter++;
            try
            {
                UpdateCameraLocation();

                this.CurrentScore = 0;
                this.MaximumReachedScore = 0;

                // Perform all initializations on the scene
                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    manipulator.DefineTileResoures();
                    manipulator.BuildBackground();
                    manipulator.BuildGameField();
                    manipulator.BuildGameFieldLabels();
                });

                // Perform start logic
                await RestartGameAsync();

                m_scene.PerformBeforeUpdateAsync(OnGameLoopTick)
                    .FireAndForget();
            }
            finally
            {
                m_startedAnimationsCounter--;
            }
        }

        /// <summary>
        /// Starts or resets the game.
        /// </summary>
        internal async Task RestartGameAsync()
        {
            await UpdateScoreDataAsync();

            m_startedAnimationsCounter++;
            try
            {
                // Trigger delete animations
                List<Task> preDeleteTasks = new List<Task>();
                List<ValueTile> toRemoveTiles = new List<ValueTile>();
                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    for (int loopX = 0; loopX < 4; loopX++)
                    {
                        for (int loopY = 0; loopY < 4; loopY++)
                        {
                            if (m_gameTiles[loopX, loopY] == null) { continue; }

                            preDeleteTasks.Add(m_gameTiles[loopX, loopY].BuildAnimationSequence()
                                .PreDeleteAnimation()
                                .ApplyAsync());
                            toRemoveTiles.Add(m_gameTiles[loopX, loopY]);

                            m_gameTiles[loopX, loopY] = null;
                        }
                    }
                });

                // Wait for all delete animations
                if(preDeleteTasks.Count > 0)
                {
                    await Task.WhenAll(preDeleteTasks);
                }

                // Delete previous tiles and create new ones here
                Task animationTask = null;
                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    foreach(ValueTile actValueTile in toRemoveTiles)
                    {
                        manipulator.Remove(actValueTile);
                    }

                    // Generate two new ones
                    Task task1 = TryCreateNewTile(manipulator);
                    Task task2 = TryCreateNewTile(manipulator);
                    animationTask = Task.WhenAll(task1, task2);
                });

                // Wait unti animation has finished
                await animationTask;

                // Reset current score
                this.CurrentScore = 0;
            }
            finally
            {
                m_startedAnimationsCounter--;
            }
        }

        /// <summary>
        /// Tries to move all tiles up.
        /// </summary>
        /// <returns>True if command was successful.</returns>
        internal async Task<bool> TryMoveUpAsync()
        {
            this.IsMenuOpened = false;

            m_startedAnimationsCounter++;
            try
            {
                List<Task<int>> generatedTasks = new List<Task<int>>(16);

                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Search for mergable pairs and trigger movements
                    for (int loopX = 0; loopX < 4; loopX++)
                    {
                        TileMoveAndMerger moveAndMerger = new TileMoveAndMerger(m_gameTiles, Tuple.Create(0, -1));
                        for (int loopY = 3; loopY >= 0; loopY--)
                        {
                            Task<int> actTask = moveAndMerger.CalculateForNextRow(loopX, loopY);
                            if (actTask != null) { generatedTasks.Add(actTask); }
                        }
                    }
                });

                // Wait for all animation tasks
                await WaitAnimationsAndApplyScores(generatedTasks);

                // Finish this game operation.
                if( generatedTasks.Count > 0)
                {
                    return await TryCreateNewTile();
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                m_startedAnimationsCounter--;
            }
        }

        /// <summary>
        /// Tries to move all tiles down.
        /// </summary>
        /// <returns>True if command was successful.</returns>
        internal async Task<bool> TryMoveDownAsync()
        {
            this.IsMenuOpened = false;

            m_startedAnimationsCounter++;
            try
            {
                List<Task<int>> generatedTasks = new List<Task<int>>(16);

                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Search for mergable pairs and trigger movements
                    for (int loopX = 0; loopX < 4; loopX++)
                    {
                        TileMoveAndMerger moveAndMerger = new TileMoveAndMerger(m_gameTiles, Tuple.Create(0, 1));
                        for (int loopY = 0; loopY < 4; loopY++)
                        {
                            Task<int> actTask = moveAndMerger.CalculateForNextRow(loopX, loopY);
                            if (actTask != null) { generatedTasks.Add(actTask); }
                        }
                    }
                });

                // Wait for all animation tasks
                await WaitAnimationsAndApplyScores(generatedTasks);

                // Finish this game operation.
                if (generatedTasks.Count > 0)
                {
                    return await TryCreateNewTile();
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                m_startedAnimationsCounter--;
            }
        }

        /// <summary>
        /// Tries to move all tiles to the right.
        /// </summary>
        /// <returns>True if command was successful.</returns>
        internal async Task<bool> TryMoveRightAsync()
        {
            this.IsMenuOpened = false;

            m_startedAnimationsCounter++;
            try
            {
                List<Task<int>> generatedTasks = new List<Task<int>>(16);

                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Search for mergable pairs and trigger movements
                    for (int loopY = 0; loopY < 4; loopY++)
                    {
                        TileMoveAndMerger moveAndMerger = new TileMoveAndMerger(m_gameTiles, Tuple.Create(-1, 0));
                        for (int loopX = 3; loopX >= 0; loopX--)
                        {
                            Task<int> actTask = moveAndMerger.CalculateForNextRow(loopX, loopY);
                            if (actTask != null) { generatedTasks.Add(actTask); }
                        }
                    }
                });

                // Wait for all animation tasks
                await WaitAnimationsAndApplyScores(generatedTasks);

                // Finish this game operation.
                if (generatedTasks.Count > 0)
                {
                    return await TryCreateNewTile();
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                m_startedAnimationsCounter--;
            }
        }

        /// <summary>
        /// Tries to move all tiles to the left.
        /// </summary>
        /// <returns>True if command was successful.</returns>
        internal async Task<bool> TryMoveLeftAsync()
        {
            this.IsMenuOpened = false;

            m_startedAnimationsCounter++;
            try
            {
                List<Task<int>> generatedTasks = new List<Task<int>>(16);

                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    // Search for mergable pairs and trigger movements
                    for (int loopY = 0; loopY < 4; loopY++)
                    {
                        TileMoveAndMerger moveAndMerger = new TileMoveAndMerger(m_gameTiles, Tuple.Create(1, 0));
                        for (int loopX = 0; loopX < 4; loopX++)
                        {
                            Task<int> actTask = moveAndMerger.CalculateForNextRow(loopX, loopY);
                            if (actTask != null) { generatedTasks.Add(actTask); }
                        }
                    }
                });

                // Wait for all animation tasks
                await WaitAnimationsAndApplyScores(generatedTasks);

                // Finish this game operation.
                if (generatedTasks.Count > 0)
                {
                    return await TryCreateNewTile();
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                m_startedAnimationsCounter--;
            }
        }

        /// <summary>
        /// Tries to create a new tile.
        /// </summary>
        /// <returns>False if there was no space left.</returns>
        internal async Task<bool> TryCreateNewTile()
        {
            m_startedAnimationsCounter++;
            try
            {
                Task<bool> createTask = null;

                await m_scene.ManipulateSceneAsync((manipulator) =>
                {
                    createTask = TryCreateNewTile(manipulator);
                });

                return await createTask;
            }
            finally
            {
                m_startedAnimationsCounter--;
            }
        }

        /// <summary>
        /// Helper method that waits for all given animations and applies gained scores.
        /// </summary>
        /// <param name="animationTasks">The animation tasks to wait for.</param>
        private async Task WaitAnimationsAndApplyScores(IEnumerable<Task<int>> animationTasks)
        {
            // Wait for finished animation tasks
            await Task.WhenAll(animationTasks);

            // Update score values
            this.CurrentScore = this.CurrentScore + animationTasks
                .Select((actTask) => actTask.Result)
                .Sum();
            if(this.CurrentScore > this.MaximumReachedScore)
            {
                this.MaximumReachedScore = this.CurrentScore;
            }
        }

        /// <summary>
        /// Tries to create a new tile of the given type id.
        /// </summary>
        /// <param name="manipulator">The manipulator of the scene to be modified.</param>
        /// <param name="typeID">The id of the type of the tile.</param>
        private async Task<bool> TryCreateNewTile(SceneManipulator manipulator)
        {
            // Try to find out which locations are free
            List<Tuple<int, int>> freeTiles = new List<Tuple<int, int>>(4 * 4);
            for (int loopX = 0; loopX < 4; loopX++)
            {
                for (int loopY = 0; loopY < 4; loopY++)
                {
                    if (m_gameTiles[loopX, loopY] == null) { freeTiles.Add(Tuple.Create(loopX, loopY)); }
                }
            }

            // Cancel here if there are not free locations left (that means, game is finished..)
            if (freeTiles.Count == 0) { return false; }

            // Generate a new tile on a random location
            Tuple<int, int> newTilePos = freeTiles[m_randomizer.Next(0, freeTiles.Count)];
            ValueTile newValueTile = new ValueTile(
                newTilePos.Item1, newTilePos.Item2,
                m_randomizer.Next(0, 5) / 3);
            newValueTile.EnableShaderGeneratedBorder();
            m_gameTiles[newTilePos.Item1, newTilePos.Item2] = newValueTile;

            newValueTile.Scaling = new Vector3(0.1f, 0.1f, 0.1f);
            manipulator.Add(newValueTile);

            // Start fade-in animation
            await newValueTile.BuildAnimationSequence()
                .DoTileFadeIn()
                .ApplyAsync();

            return true;
        }

        /// <summary>
        /// Called when the game loop ticks.
        /// </summary>
        private void OnGameLoopTick()
        {
            UpdateCameraLocation();

            // Trigger next tick
            m_scene.PerformBeforeUpdateAsync(OnGameLoopTick)
                .FireAndForget();
        }

        /// <summary>
        /// Updates current score data.
        /// </summary>
        public async Task UpdateScoreDataAsync()
        {
            try
            {
                // Load current datacontainer
                GameDataContainer dataContainer = await GameDataContainer.LoadFromRoamingFolderAsync(Constants.GAME_NAME);

                // Perform changes (if needed
                bool dataContainerChanged = false;
                if (dataContainer.GameScores.Count == 0)
                {
                    dataContainer.GameScores.Add(new GameScore("Dummy", this.MaximumReachedScore));
                    dataContainerChanged = true;
                }
                else
                {
                    if (dataContainer.GameScores[0].TotalScore > this.MaximumReachedScore) 
                    { 
                        this.MaximumReachedScore = dataContainer.GameScores[0].TotalScore; 
                    }
                    else 
                    {
                        dataContainer.GameScores[0].TotalScore = this.MaximumReachedScore;
                        dataContainerChanged = true;
                    }
                }

                // Save data if there where changes made on it
                if (dataContainerChanged)
                {
                    await dataContainer.SaveToRoamingFolder();
                }
            }
            catch
            {
                // Something did go wrong.. do not rais an error here, 
            }
        }

#if UNIVERSAL
        /// <summary>
        /// Called when the app is suspending (e. g. called before program exits).
        /// </summary>
        private async void OnApp_Suspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            try
            {
                await UpdateScoreDataAsync();
            }
            finally
            {
                deferral.Complete();
            }
        }
#endif

        public DelegateCommand CommandRestart
        {
            get;
            private set;
        }

        public DelegateCommand CommandWeb
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the sets the visibility of the main menu.
        /// </summary>
        public bool IsMenuOpened
        {
            get { return m_menuEnabled; }
            set
            {
                if (m_menuEnabled != value)
                {
                    m_menuEnabled = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public int CurrentScore
        {
            get { return m_currentScore; }
            set
            {
                if(m_currentScore != value)
                {
                    m_currentScore = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int MaximumReachedScore
        {
            get { return m_maximumReachedScore; }
            set
            {
                if(m_maximumReachedScore != value)
                {
                    m_maximumReachedScore = value;
                    RaisePropertyChanged();
                }
            }
        }

        // All translatable properties
        public string TextScoreCurrent { get { return Translatables.SCORE_CURRENT; } }
        public string TextScoreMaximum { get { return Translatables.SCORE_MAXIMUM; } }
        public string TextTitle { get { return Translatables.MAIN_PAGE_TITLE; } }
        public string TextSubtitle { get { return Translatables.MAIN_PAGE_SUBTITLE; } }
        public string TextRestart { get { return Translatables.GAME_RESTART; } }
        public string TextWeb { get { return Translatables.MENU_WEB; } }
        public string TextHelp { get { return Translatables.MENU_HELP; } }
    }
}
