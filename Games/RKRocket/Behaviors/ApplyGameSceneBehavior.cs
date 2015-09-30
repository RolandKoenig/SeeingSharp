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
using Microsoft.Xaml.Interactivity;
using RKRocket.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SeeingSharp.Multimedia.Views;
using SeeingSharp;

namespace RKRocket.Behaviors
{
    public class ApplyGameSceneBehavior : DependencyObject, IBehavior
    {
        public static readonly DependencyProperty GameCoreProperty =
            DependencyProperty.Register("GameCore", typeof(GameCore), typeof(ApplyGameSceneBehavior), new PropertyMetadata(null));

        #region Members for UI connection
        private SwapChainPanel m_targetBGPanel;
        private SeeingSharpPanelPainter m_panelPainter;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyGameSceneBehavior"/> class.
        /// </summary>
        public ApplyGameSceneBehavior()
        {
            m_panelPainter = new SeeingSharpPanelPainter();

            // Configure view
            var viewConfig = m_panelPainter.RenderLoop.ViewConfiguration;
            viewConfig.AlphaEnabledSwapChain = true;
            viewConfig.AntialiasingEnabled = false;
        }

        public void Attach(DependencyObject associatedObject)
        {
            SwapChainPanel targetBGPanel = associatedObject as SwapChainPanel;
            if((targetBGPanel != null) &&
               (m_panelPainter.TargetPanel != targetBGPanel))
            {
                if(m_panelPainter.IsAttachedToGui)
                {
                    m_panelPainter.Detach();
                }

                m_targetBGPanel = targetBGPanel;
                
                m_panelPainter.Attach(m_targetBGPanel);
            }

            UpdateSceneGameRelation();
        }

        public void Detach()
        {
            if(m_panelPainter.IsAttachedToGui)
            {
                m_panelPainter.Detach();

                m_targetBGPanel = null;
            }

            UpdateSceneGameRelation();
        }

        private void UpdateSceneGameRelation()
        {
            GameCore gameCore = this.GameCore;
            if(m_panelPainter.IsAttachedToGui &&
               gameCore != null)
            {
                m_panelPainter.Scene = gameCore.GameScene;
                m_panelPainter.Camera = gameCore.Camera;
                m_panelPainter.RenderLoop.ClearColor = Color4.Transparent;
            }
        }

        public DependencyObject AssociatedObject
        {
            get { return m_targetBGPanel; }
        }

        public GameCore GameCore
        {
            get { return (GameCore)GetValue(GameCoreProperty); }
            set 
            { 
                SetValue(GameCoreProperty, value);
                this.UpdateSceneGameRelation();
            }
        }
    }
}
