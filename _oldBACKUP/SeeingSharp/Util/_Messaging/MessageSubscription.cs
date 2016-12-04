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
using System.ComponentModel;

namespace SeeingSharp.Util
{
    /// <summary>
    /// This class holds all information about message subscriptions. It implements IDisposable for unsubscribing
    /// from the message.
    /// </summary>
    public class MessageSubscription : IDisposable
    {
        // Main members for publishing
        private SeeingSharpMessenger m_Messenger;
        private Type m_messageType;
        private Delegate m_targetHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSubscription"/> class.
        /// </summary>
        /// <param name="Messenger">The messenger.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="targetHandler">The target handler.</param>
        internal MessageSubscription(SeeingSharpMessenger Messenger, Type messageType, Delegate targetHandler)
        {
            m_Messenger = Messenger;
            m_messageType = messageType;
            m_targetHandler = targetHandler;
        }

        /// <summary>
        /// Unsubscribes this subscription.
        /// </summary>
        public void Unsubscribe()
        {
            this.Dispose();
        }

        /// <summary>
        /// Sends the given message to the target.
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="message">The message to be published.</param>
        internal void Publish<MessageType>(MessageType message)
            where MessageType : SeeingSharpMessage
        {
            // Call this subscription
            if (!this.IsDisposed)
            {
                Action<MessageType> targetDelegate = m_targetHandler as Action<MessageType>;
                if (targetDelegate != null)
                {
                    targetDelegate(message);
                }
            }
        }

        /// <summary>
        /// Clears this message.
        /// </summary>
        internal void Clear()
        {
            m_Messenger = null;
            m_messageType = null;
            m_targetHandler = null;
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                m_Messenger.Unsubscribe(this);
            }
        }

        /// <summary>
        /// Is this subscription valid?
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return
                    (m_Messenger == null) ||
                    (m_messageType == null) ||
                    (m_targetHandler == null);
            }
        }

        /// <summary>
        /// Gets the corresponding Messenger object.
        /// </summary>
        public SeeingSharpMessenger Messenger
        {
            get { return m_Messenger; }
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public Type MessageType
        {
            get { return m_messageType; }
        }

        /// <summary>
        /// Gets the name of the message type.
        /// </summary>
        public string MessageTypeName
        {
            get { return m_messageType.Name; }
        }

        /// <summary>
        /// Gets the name of the target object.
        /// </summary>
        public string TargetObjectName
        {
            get
            {
                if (m_targetHandler == null) { return string.Empty; }
                if (m_targetHandler.Target == null) { return string.Empty; }
                return m_targetHandler.Target.ToString();
            }
        }

#if DESKTOP
        /// <summary>
        /// Gets the name of the target method.
        /// </summary>
        public string TargetMethodName
        {
            get
            {
                if (m_targetHandler == null) { return string.Empty; }
                if (m_targetHandler.Target == null) { return string.Empty; }
                if (m_targetHandler.Method == null) { return string.Empty; }
                return m_targetHandler.Method.Name;
            }
        }
#endif
    }
}
