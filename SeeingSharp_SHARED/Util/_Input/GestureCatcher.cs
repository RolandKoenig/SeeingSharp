#region License information (SeeingSharp and all based games/applications)
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
#if UNIVERSAL

using System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.Foundation;

namespace SeeingSharp.Util
{
    public class UIGestureCatcher
    {
        private const double RECOGNIZE_DISTANCE = 40.0;
        private static readonly TimeSpan RECOGNIZE_MIN_INTERVAL = TimeSpan.FromSeconds(0.5);

        private UIElement m_uiElement;

        //Some very simple gesture events
        public event EventHandler MoveLeft;
        public event EventHandler MoveTop;
        public event EventHandler MoveRight;
        public event EventHandler MoveDown;

        private Windows.Foundation.Point m_lastCapturedPoint;
        private DateTime m_lastEventTimestamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIGestureCatcher" /> class.
        /// </summary>
        public UIGestureCatcher(UIElement uiElement)
        {
            m_uiElement = uiElement;

            //Configure manipulation events an register on update event
            m_uiElement.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            m_uiElement.ManipulationStarted += OnUIElementManipulationStarted;
            m_uiElement.ManipulationDelta += OnUIElementManipulationDelta;

            MoveLeft = null;
            MoveTop = null;
            MoveRight = null;
            MoveDown = null;

            m_lastEventTimestamp = DateTime.MinValue;
        }

        /// <summary>
        /// Stops gesture capturing.
        /// </summary>
        public void Stop()
        {
            if (m_uiElement != null)
            {
                m_uiElement.ManipulationStarted -= OnUIElementManipulationStarted;
                m_uiElement.ManipulationDelta -= OnUIElementManipulationDelta;
                m_uiElement = null;
            }
        }

        private void OnUIElementManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            m_lastCapturedPoint = new Windows.Foundation.Point(0, 0);
        }

        private void OnUIElementManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs args)
        {
            double translationX = args.Cumulative.Translation.X - m_lastCapturedPoint.X;
            double translationY = args.Cumulative.Translation.Y - m_lastCapturedPoint.Y;

            if ((Math.Abs(translationX) > RECOGNIZE_DISTANCE) ||
                (Math.Abs(translationY) > RECOGNIZE_DISTANCE))
            {
                //Handle the case when one coordinat is zero
                if (translationX == 0f)
                {
                    if (translationY < 0f) { MoveTop.Raise(this, EventArgs.Empty); }
                    else if (CanRaiseEvent())
                    { 
                        MoveDown.Raise(this, EventArgs.Empty);
                        m_lastEventTimestamp = DateTime.UtcNow;
                    }

                    m_lastCapturedPoint = args.Cumulative.Translation;
                    return;
                }
                else if (translationY == 0f)
                {
                    if (translationX < 0f) { MoveLeft.Raise(this, EventArgs.Empty); }
                    else if(CanRaiseEvent())
                    { 
                        MoveRight.Raise(this, EventArgs.Empty);
                        m_lastEventTimestamp = DateTime.UtcNow;
                    }

                    m_lastCapturedPoint = args.Cumulative.Translation;
                    return;
                }

                //Handling logic for standard case
                float xToY = (float)translationX / (float)translationY;
                float yToX = (float)translationY / (float)translationX;
                if (Math.Abs(xToY) < 0.4f)
                {
                    if (translationY < 0f) { MoveTop.Raise(this, EventArgs.Empty); }
                    else if(CanRaiseEvent()) 
                    { 
                        MoveDown.Raise(this, EventArgs.Empty);
                        m_lastEventTimestamp = DateTime.UtcNow;
                    }

                    m_lastCapturedPoint = args.Cumulative.Translation;
                    return;
                }
                else if (Math.Abs(yToX) < 0.4f)
                {
                    if (translationX < 0f) { MoveLeft.Raise(this, EventArgs.Empty); }
                    else if(CanRaiseEvent()) 
                    { 
                        MoveRight.Raise(this, EventArgs.Empty);
                        m_lastEventTimestamp = DateTime.UtcNow;
                    }

                    m_lastCapturedPoint = args.Cumulative.Translation;
                    return;
                }
            }
        }

        /// <summary>
        /// Can we raise an event (=> Checks time interval since last event.
        /// </summary>
        private bool CanRaiseEvent()
        {
            return DateTime.UtcNow - m_lastEventTimestamp > RECOGNIZE_MIN_INTERVAL;
        }
    }
}

#endif