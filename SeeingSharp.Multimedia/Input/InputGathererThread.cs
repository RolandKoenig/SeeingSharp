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
using SeeingSharp.Util;
using SeeingSharp.Checking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeingSharp.Multimedia.Input
{
    /// <summary>
    /// This class is responsible for input gathering.
    /// </summary>
    public class InputGathererThread : ObjectThread
    {
        #region Synchronization
        private ThreadSaveQueue<Action> m_commandQueue;
        private ThreadSaveQueue<InputFrame> m_gatheredInputFrames;
        private InputFrame m_lastInputFrame;
        #endregion

        #region Thread local state
        private List<IInputHandler> m_inputHandlers;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="InputGathererThread"/> class.
        /// </summary>
        internal InputGathererThread()
            : base("Input Gatherer", 1000 / Constants.INPUT_FRAMES_PER_SECOND)
        {
            m_commandQueue = new ThreadSaveQueue<Action>();
            m_gatheredInputFrames = new ThreadSaveQueue<InputFrame>();
            m_inputHandlers = new List<IInputHandler>();
        }

        /// <summary>
        /// Gets all gathered InputFrames.
        /// </summary>
        internal List<InputFrame> GetAllFrames()
        {
            return m_gatheredInputFrames.DequeueAll();
        }

        /// <summary>
        /// Registers the given InputHandler for input gathering.
        /// </summary>
        internal void RegisterInputHandler(IInputHandler inputHandler)
        {
            inputHandler.EnsureNotNull(nameof(inputHandler));

            m_commandQueue.Enqueue(() => m_inputHandlers.Add(inputHandler));
        }

        /// <summary>
        /// Deregisters the given InputHandler.
        /// </summary>
        internal void DeregisterInputHandler(IInputHandler inputHandler)
        {
            inputHandler.EnsureNotNull(nameof(inputHandler));

            m_commandQueue.Enqueue(() =>
            {
                while (m_inputHandlers.Contains(inputHandler)) { m_inputHandlers.Remove(inputHandler); }
            });
        }

        protected override void OnTick(EventArgs eArgs)
        {
            base.OnTick(eArgs);

            // Execute all commands within the command queue
            if(m_commandQueue.Count > 0)
            {
                Action actCommand = null;
                while(m_commandQueue.Dequeue(out actCommand))
                {
                    actCommand();
                }
            }

            // Gather all input data
            int expectedStateCount = m_lastInputFrame != null ? m_lastInputFrame.CountStates : 6;
            InputFrame newInputFrame = new InputFrame(expectedStateCount);
            foreach(IInputHandler actInputHandler in m_inputHandlers)
            {
                foreach(InputStateBase actInputState in actInputHandler.GetInputStates())
                {
                    actInputState.EnsureNotNull(nameof(actInputState));

                    newInputFrame.AddState(actInputState);
                }
            }

            // Store the generated InputFrame 
            m_lastInputFrame = newInputFrame;
            m_gatheredInputFrames.Enqueue(newInputFrame);
        }
    }
}
