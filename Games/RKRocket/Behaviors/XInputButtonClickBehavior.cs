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
using Microsoft.Xaml.Interactivity;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Multimedia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SeeingSharp.Util;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace RKRocket.Behaviors
{
    public class XInputButtonClickBehavior : DependencyObject, IBehavior
    {
        private DependencyObject m_associatedObject;
        private Button m_associatedButton;

        public void Attach(DependencyObject associatedObject)
        {
            m_associatedObject = associatedObject;
            m_associatedButton = m_associatedObject as Button;

            // Register generic input event
            if(m_associatedObject != null)
            {
                GraphicsCore.Current.MainLoop.GenericInput += OnGraphicsCore_GenericInput;
            }
        }

        public void Detach()
        {
            m_associatedObject = null;
            m_associatedButton = null;

            // Deregister generic input event
            if(m_associatedObject == null)
            {
                GraphicsCore.Current.MainLoop.GenericInput -= OnGraphicsCore_GenericInput;
            }
        }

        private void OnGraphicsCore_GenericInput(object sender, GenericInputEventArgs e)
        {
            if (!e.AnyRelevantState) { return; }

            // Get the button which we are listening for
            GamepadButton listeningButton = GamepadButton.A;
            switch(ControllerButton)
            {
                case XInputControllerButton.A:
                    listeningButton = GamepadButton.A;
                    break;

                case XInputControllerButton.B:
                    listeningButton = GamepadButton.B;
                    break;

                case XInputControllerButton.X:
                    listeningButton = GamepadButton.X;
                    break;

                case XInputControllerButton.Y:
                    listeningButton = GamepadButton.Y;
                    break;

                case XInputControllerButton.Start:
                    listeningButton = GamepadButton.Start;
                    break;

                default:
                    return;
            }

            // Check button state
            bool buttonPressed = e.DefaultGamepad.IsButtonHit(listeningButton);
            if (!buttonPressed) { return; }

            // Raise button click
            Button currentButton = m_associatedButton;
            if (currentButton != null)
            {
                currentButton.Dispatcher.RunAsync(
                    CoreDispatcherPriority.High,
                    new DispatchedHandler(() =>
                    {
                        ButtonAutomationPeer peer = new ButtonAutomationPeer(currentButton);
                        IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                        invokeProv.Invoke();
                    }))
                    .FireAndForget();
            }
        }

        public DependencyObject AssociatedObject
        {
            get { return m_associatedObject; }
        }

        public XInputControllerButton ControllerButton
        {
            get;
            set;
        }
    }
}
