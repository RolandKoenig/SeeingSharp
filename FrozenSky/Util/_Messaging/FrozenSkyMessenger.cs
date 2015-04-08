#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
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

using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FrozenSky.Checking;

namespace FrozenSky.Util
{
    /// <summary>
    /// Main class for sending/receiving messages.
    /// This class followes the Messenger pattern but modifies it on some parts like 
    /// thread synchronization.
    /// What 'messenger' actually is, see here a short explanation: http://stackoverflow.com/questions/22747954/mvvm-light-toolkit-messenger-uses-event-aggregator-or-mediator-pattern
    /// </summary>
    public class FrozenSkyMessenger
    {
        /// <summary>
        /// Gets or sets a custom exception handler which is used globally.
        /// Return true to skip default exception behavior (exception is thrown to publisher by default).
        /// </summary>
        public static Func<FrozenSkyMessenger, Exception, bool> CustomPublishExceptionHandler;

        // Global synchronization (enables the possibility to publish a message over more threads / areas of the application)
        private static ConcurrentDictionary<string, FrozenSkyMessenger> s_messengersByName;

        // Global information about message routing
        #region
        private static ConcurrentDictionary<Type, string[]> s_messagesAsyncTargets;
        private static ConcurrentDictionary<Type, string[]> s_messageSources;
        #endregion

        // Checking and global synchronization
        #region
        private string m_messengerName;
        private SynchronizationContext m_syncContext;
        private FrozenSkyMessageThreadingBehavior m_checkBehavior;
        #endregion

        // Message subscriptions
        #region
        private Dictionary<Type, List<MessageSubscription>> m_messageSubscriptions;
        private object m_messageSubscriptionsLock;
        #endregion

        /// <summary>
        /// Initializes the <see cref="FrozenSkyMessenger" /> class.
        /// </summary>
        static FrozenSkyMessenger()
        {
            s_messengersByName = new ConcurrentDictionary<string, FrozenSkyMessenger>();

            s_messagesAsyncTargets = new ConcurrentDictionary<Type, string[]>();
            s_messageSources = new ConcurrentDictionary<Type, string[]>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrozenSkyMessenger"/> class.
        /// </summary>
        public FrozenSkyMessenger()
        {
            m_messengerName = string.Empty;
            m_syncContext = null;
            m_checkBehavior = FrozenSkyMessageThreadingBehavior.Ignore;

            m_messageSubscriptions = new Dictionary<Type, List<MessageSubscription>>();
            m_messageSubscriptionsLock = new object();
        }

        /// <summary>
        /// Gets the FrozenSkyMessenger by the given name.
        /// </summary>
        /// <param name="messengerName">The name of the messenger.</param>
        public static FrozenSkyMessenger GetByName(string messengerName)
        {
            messengerName.EnsureNotNullOrEmpty("messengerName");

            var result = TryGetByName(messengerName);
            if (result == null) { throw new FrozenSkyException(string.Format("Unable to find Messenger for thread {0}!", messengerName)); }
            return result;
        }

        /// <summary>
        /// Gets the FrozenSkyMessenger by the given name.
        /// </summary>
        /// <param name="messengerName">The name of the messenger.</param>
        public static FrozenSkyMessenger TryGetByName(string messengerName)
        {
            messengerName.EnsureNotNullOrEmpty("messengerName");

            FrozenSkyMessenger result = null;
            s_messengersByName.TryGetValue(messengerName, out result);
            return result;
        }

        /// <summary>
        /// Sets all required threading properties based on the given target thread.
        /// The name of the messenger is directly taken from the given ObjectThread.
        /// </summary>
        /// <param name="targetThread">The thread on which this Messanger should work on.</param>
        public void ApplyForGlobalSynchronization(ObjectThread targetThread)
        {
            targetThread.EnsureNotNull("targetThread");

            ApplyForGlobalSynchronization(
                FrozenSkyMessageThreadingBehavior.EnsureMainSyncContextOnSyncCalls,
                targetThread.Name,
                targetThread.SyncContext);
        }

        /// <summary>
        /// Sets all required synchronization properties.
        /// </summary>
        /// <param name="checkBehavior">Defines the checking behavior for Publish calls.</param>
        /// <param name="messengerName">The name by which this messenger should be registered.</param>
        /// <param name="syncContext">The synchronization context to be used.</param>
        public void ApplyForGlobalSynchronization(FrozenSkyMessageThreadingBehavior checkBehavior, string messengerName, SynchronizationContext syncContext)
        {
            messengerName.EnsureNotNullOrEmpty("messengerName");
            syncContext.EnsureNotNull("syncContext");

            m_messengerName = messengerName;
            m_checkBehavior = checkBehavior;
            m_syncContext = syncContext;

            if (!string.IsNullOrEmpty(messengerName))
            {
                s_messengersByName.TryAdd(messengerName, this);
            }
        }

        /// <summary>
        /// Clears all synchronization options.
        /// </summary>
        public void DiscardGlobalSynchronization()
        {
            if (string.IsNullOrEmpty(m_messengerName)) { return; }

            FrozenSkyMessenger dummyResult = null;
            s_messengersByName.TryRemove(m_messengerName, out dummyResult);

            m_checkBehavior = FrozenSkyMessageThreadingBehavior.Ignore;
            m_messengerName = string.Empty;
            m_syncContext = null;
        }

        /// <summary>
        /// Gets a collection containing all active subscriptions.
        /// </summary>
        public List<MessageSubscription> GetAllSubscriptions()
        {
            List<MessageSubscription> result = new List<MessageSubscription>();

            lock (m_messageSubscriptionsLock)
            {
                foreach (KeyValuePair<Type, List<MessageSubscription>> actPair in m_messageSubscriptions)
                {
                    foreach(var actSubscription in actPair.Value)
                    {
                        result.Add(actSubscription);
                    }
                }
            }

            return result;
        }

#if DESKTOP
        /// <summary>
        /// Subscribes all receiver-methods of the given target object to this Messenger.
        /// Subscribe and unsubscribe is automatically called when the control is created/destroyed.
        /// </summary>
        /// <param name="targetObject">The target object which is to subscribe..</param>
        public void SubscribeAllOnControl(System.Windows.Forms.Control target)
        {
            target.EnsureNotNull("target");

            IEnumerable<MessageSubscription> generatedSubscriptions = null;

            // Create handler delegates
            EventHandler onHandleCreated = (sender, eArgs) =>
            {
                if (generatedSubscriptions == null)
                {
                    generatedSubscriptions = SubscribeAll(target);
                }
            };
            EventHandler onHandleDestroyed = (inner, eArgs) =>
            {
                if (generatedSubscriptions != null)
                {
                    foreach(MessageSubscription actSubscription in generatedSubscriptions)
                    {
                        actSubscription.Unsubscribe();
                    }
                    generatedSubscriptions = null;
                }
            };

            // Attach to events and subscribe on message, if handle is already created
            target.HandleCreated += onHandleCreated;
            target.HandleDestroyed += onHandleDestroyed;
            if (target.IsHandleCreated)
            {
                generatedSubscriptions = SubscribeAll(target);
            }
        }
#endif

        /// <summary>
        /// Subscribes all receiver-methods of the given target object to this Messenger.
        /// </summary>
        /// <param name="targetObject">The target object which is to subscribe..</param>
        public IEnumerable<MessageSubscription> SubscribeAll(object targetObject)
        {
            targetObject.EnsureNotNull("targetObject");

            Type targetObjectType = targetObject.GetType();

            List<MessageSubscription> generatedSubscriptions = new List<MessageSubscription>(16);
            try
            {
                Type typeofMessage = typeof(FrozenSkyMessage);
#if DESKTOP
                foreach (MethodInfo actMethod in targetObjectType.GetMethods(
                    BindingFlags.NonPublic | BindingFlags.Public | 
                    BindingFlags.Instance | BindingFlags.InvokeMethod))
#else
                foreach(MethodInfo actMethod in targetObjectType.GetTypeInfo().GetDeclaredMethods(
                    FrozenSkyConstants.METHOD_NAME_MESSAGE_RECEIVED))
#endif
                {
                    if (!actMethod.Name.Equals(FrozenSkyConstants.METHOD_NAME_MESSAGE_RECEIVED)) { continue; }

                    ParameterInfo[] parameters = actMethod.GetParameters();
                    if (parameters.Length != 1) { continue; }

                    if (!typeofMessage.GetTypeInfo().IsAssignableFrom(
                        parameters[0].ParameterType.GetTypeInfo())) 
                    {
                        continue; 
                    }

                    Type genericAction = typeof(Action<>);
                    Type delegateType = genericAction.MakeGenericType(parameters[0].ParameterType);
                    generatedSubscriptions.Add(this.Subscribe(
                        actMethod.CreateDelegate(delegateType, targetObject),
                        parameters[0].ParameterType));
                }
            }
            catch(Exception)
            {
                foreach(MessageSubscription actSubscription in generatedSubscriptions)
                {
                    actSubscription.Unsubscribe();
                }
                generatedSubscriptions.Clear();
            }

            return generatedSubscriptions;
        }

        /// <summary>
        /// Subscribes to the given MessageType.
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="Messenger">Action to perform on incoming message.</param>
        public MessageSubscription Subscribe<MessageType>(Action<MessageType> Messenger)
            where MessageType : FrozenSkyMessage
        {
            Messenger.EnsureNotNull("Messenger");

            Type currentType = typeof(MessageType);
            return this.Subscribe(Messenger, currentType);
        }

        /// <summary>
        /// Subscribes the given MessageType and executes the action only when the condition is true.
        /// </summary>
        /// <typeparam name="MessageType">The type of the message type.</typeparam>
        /// <param name="condition">The condition.</param>
        /// <param name="Messenger">The messenger.</param>
        /// <returns></returns>
        public MessageSubscription SubscribeWhen<MessageType>(Func<MessageType, bool> condition, Action<MessageType> Messenger)
            where MessageType : FrozenSkyMessage
        {
            condition.EnsureNotNull("condition");
            Messenger.EnsureNotNull("Messenger");

            Action<MessageType> filterAction = (message) =>
            {
                if (condition(message))
                {
                    Messenger(message);
                }
            };

            Type currentType = typeof(MessageType);
            return this.Subscribe(filterAction, currentType);
        }

        /// <summary>
        /// Subscribes to the given message type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="Messenger">Action to perform on incoming message.</param>
        public MessageSubscription Subscribe(
            Delegate Messenger, Type messageType)
        {
            Messenger.EnsureNotNull("Messenger");
            messageType.EnsureNotNull("messageType");

            if (!messageType.GetTypeInfo().IsSubclassOf(typeof(FrozenSkyMessage))) { throw new ArgumentException("Given message type does not derive from FrozenSkyMessage!"); }

            MessageSubscription newOne = new MessageSubscription(this, messageType, Messenger);
            lock (m_messageSubscriptionsLock)
            {
                if (m_messageSubscriptions.ContainsKey(messageType))
                {
                    m_messageSubscriptions[messageType].Add(newOne);
                }
                else
                {
                    List<MessageSubscription> newList = new List<MessageSubscription>();
                    newList.Add(newOne);
                    m_messageSubscriptions[messageType] = newList;
                }
            }

            return newOne;
        }

#if DESKTOP
        /// <summary>
        /// Subscribes to the given MessageType during the livetime of the given control.
        /// Events OnHandleCreated and OnHandleDestroyed are used for subscribing / unsubscribing.
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="Messenger">Action to perform on incoming message.</param>
        /// <param name="target">The target control.</param>
        public void SubscribeOnControl<MessageType>(System.Windows.Forms.Control target, Action<MessageType> Messenger)
            where MessageType : FrozenSkyMessage
        {
            target.EnsureNotNull("target");
            Messenger.EnsureNotNull("Messenger");
            MessageSubscription subscription = null;

            //Create handler delegates
            EventHandler onHandleCreated = (sender, eArgs) =>
            {
                if (subscription == null)
                {
                    subscription = Subscribe(Messenger);
                }
            };
            EventHandler onHandleDestroyed = (inner, eArgs) =>
            {
                if (subscription != null)
                {
                    Unsubscribe(subscription);
                    subscription = null;
                }
            };

            //Attach to events and subscribe on message, if handle is already created
            target.HandleCreated += onHandleCreated;
            target.HandleDestroyed += onHandleDestroyed;
            if (target.IsHandleCreated)
            {
                subscription = Subscribe(Messenger);
            }
        }
#endif

        /// <summary>
        /// Clears the given MessageSubscription.
        /// </summary>
        /// <param name="messageSubscription">The subscription to clear.</param>
        public void Unsubscribe(MessageSubscription messageSubscription)
        {
            messageSubscription.EnsureNotNull("messageSubscription");

            if (messageSubscription != null && !messageSubscription.IsDisposed)
            {
                Type messageType = messageSubscription.MessageType;

                //Remove subscription from internal list

                lock (m_messageSubscriptionsLock)
                {
                    if (m_messageSubscriptions.ContainsKey(messageType))
                    {
                        List<MessageSubscription> listOfSubscriptions = m_messageSubscriptions[messageType];
                        listOfSubscriptions.Remove(messageSubscription);
                        if (listOfSubscriptions.Count == 0)
                        {
                            m_messageSubscriptions.Remove(messageType);
                        }
                    }
                }

                //Clear given subscription
                messageSubscription.Clear();
            }
        }

        /// <summary>
        /// Counts all message subscriptions for the given message type.
        /// </summary>
        /// <typeparam name="MessageType">The type of the message for which to count all subscriptions.</typeparam>
        public int CountSubscriptionsForMessage<MessageType>()
            where MessageType : FrozenSkyMessage
        {
            lock (m_messageSubscriptionsLock)
            {
                List<MessageSubscription> subscriptions = null;
                if (m_messageSubscriptions.TryGetValue(typeof(MessageType), out subscriptions))
                {
                    return subscriptions.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Sends the given message to all subscribers (asynchonous processing).
        /// There is no possibility here to wait for the answer.
        /// </summary>
        public void BeginPublish<MessageType>()
            where MessageType : FrozenSkyMessage, new()
        {
            BeginPublish(new MessageType());
        }

        /// <summary>
        /// Sends the given message to all subscribers (asynchonous processing).
        /// There is no possibility here to wait for the answer.
        /// </summary>
        /// <typeparam name="MessageType">The type of the essage type.</typeparam>
        /// <param name="message">The message.</param>
        public void BeginPublish<MessageType>(
            MessageType message)
            where MessageType : FrozenSkyMessage
        {
            m_syncContext.PostAlsoIfNull(() => Publish(message));
        }

        /// <summary>
        /// Sends the given message to all subscribers (asynchonous processing).
        /// The returned task waits for all synchronous subscriptions.
        /// </summary>
        /// <typeparam name="MessageType">The type of the message.</typeparam>
        public Task PublishAsync<MessageType>()
            where MessageType : FrozenSkyMessage, new()
        {
            return m_syncContext.PostAlsoIfNullAsync(
                () => Publish<MessageType>(),
                ActionIfSyncContextIsNull.InvokeUsingNewTask);
        }

        /// <summary>
        /// Sends the given message to all subscribers (asynchonous processing).
        /// The returned task waits for all synchronous subscriptions.
        /// </summary>
        /// <typeparam name="MessageType">The type of the message.</typeparam>
        /// <param name="message">The message to be sent.</param>
        public Task PublishAsync<MessageType>(
            MessageType message)
            where MessageType : FrozenSkyMessage
        {
            return m_syncContext.PostAlsoIfNullAsync(
                () => Publish(message),
                ActionIfSyncContextIsNull.InvokeUsingNewTask);
        }

        /// <summary>
        /// Sends the given message to all subscribers (synchonous processing).
        /// </summary>
        /// <param name="scriptProjectGuid">The guid of the script project which acts as a message filter.</param>
        public void Publish<MessageType>()
            where MessageType : FrozenSkyMessage, new()
        {
            Publish<MessageType>(new MessageType());
        }

        /// <summary>
        /// Sends the given message to all subscribers (synchonous processing).
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="message">The message to send.</param>
        /// <param name="scriptProjectGuid">The guid of the script project which acts as a message filter.</param>
        public void Publish<MessageType>(
            MessageType message)
            where MessageType : FrozenSkyMessage
        {
            PublishInternal<MessageType>(
                message, true);
        }

        /// <summary>
        /// Sends the given message to all subscribers (synchonous processing).
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="message">The message to send.</param>
        /// <param name="isInitialCall">Is this one the initial call to publish? (false if we are coming from async routing)</param>
        private void PublishInternal<MessageType>(
            MessageType message, bool isInitialCall)
            where MessageType : FrozenSkyMessage
        {
            message.EnsureNotNull("message");

            try
            {
                // Check whether publich is possible
                if(m_checkBehavior == FrozenSkyMessageThreadingBehavior.EnsureMainSyncContextOnSyncCalls)
                {
                    if(SynchronizationContext.Current != m_syncContext)
                    {
                        throw new FrozenSkyException(
                            "Unable to perform a synchronous publish call because current " +
                            "SynchronizationContext is set wrong. This indicates that the call " +
                            "comes from a wrong thread!");
                    }
                }

                // Notify all subscribed targets
                Type currentType = typeof(MessageType);

                // Check for correct message sources
                if (isInitialCall)
                {
                    string[] possibleSources = s_messageSources.GetOrAdd(currentType, (inputType) => message.GetPossibleSourceThreads());
                    if (possibleSources.Length > 0)
                    {
                        string mainThreadName = m_messengerName;
                        if (string.IsNullOrEmpty(mainThreadName) ||
                            (Array.IndexOf(possibleSources, mainThreadName) < 0))
                        {
                            throw new InvalidOperationException(string.Format(
                                "The message of type {0} can only be sent by the threads {1}. This Messenger belongs to the thread {2}, so no publish possible!",
                                currentType.FullName,
                                possibleSources.ToCommaSeparatedString(),
                                !string.IsNullOrEmpty(mainThreadName) ? mainThreadName as string : "(empty)"));
                        }
                    }
                }

                // Perform synchronous message handling
                List<MessageSubscription> subscriptionsToTrigger = new List<MessageSubscription>(20);
                lock (m_messageSubscriptionsLock)
                {
                    if (m_messageSubscriptions.ContainsKey(currentType))
                    {
                        // Need to copy the list to avoid problems, when the list is changed during the loop and cross thread accesses
                        subscriptionsToTrigger = new List<MessageSubscription>(m_messageSubscriptions[currentType]);
                    }
                }

                // Trigger all found subscriptions
                List<Exception> occurredExceptions = null;
                for (int loop = 0; loop < subscriptionsToTrigger.Count; loop++)
                {
                    try
                    {
                        subscriptionsToTrigger[loop].Publish(message);
                    }
                    catch (Exception ex)
                    {
                        if (occurredExceptions == null) { occurredExceptions = new List<Exception>(); }
                        occurredExceptions.Add(ex);
                    }
                }

                // Perform further message routing if enabled
                if (isInitialCall)
                {
                    // Get information about message routing
                    string[] asyncTargets = s_messagesAsyncTargets.GetOrAdd(currentType, (inputType) => message.GetAsyncRoutingTargetThreads());
                    string mainThreadName = m_messengerName;
                    for (int loop = 0; loop < asyncTargets.Length; loop++)
                    {
                        string actAsyncTargetName = asyncTargets[loop];
                        if (mainThreadName == actAsyncTargetName) { continue; }

                        FrozenSkyMessenger actAsyncTargetHandler = null;
                        if (s_messengersByName.TryGetValue(actAsyncTargetName, out actAsyncTargetHandler))
                        {
                            SynchronizationContext actSyncContext = actAsyncTargetHandler.m_syncContext;
                            if (actSyncContext == null) { continue; }

                            FrozenSkyMessenger innerHandlerForAsyncCall = actAsyncTargetHandler;
                            actSyncContext.PostAlsoIfNull(() =>
                            {
                                innerHandlerForAsyncCall.PublishInternal(message, false);
                            });
                        }
                    }
                }

                // Notify all exceptions occurred during publish progress
                if (isInitialCall)
                {
                    if ((occurredExceptions != null) &&
                        (occurredExceptions.Count > 0))
                    {
                        throw new MessagePublishException(typeof(MessageType), occurredExceptions);
                    }
                }
            }
            catch (Exception ex)
            {
                // Check whether we have to throw the exception globally
                var globalExceptionHandler = FrozenSkyMessenger.CustomPublishExceptionHandler;
                bool doRaise = true;
                if (globalExceptionHandler != null)
                {
                    try
                    {
                        doRaise = !globalExceptionHandler(this, ex);
                    }
                    catch 
                    {
                        doRaise = true;
                    }
                }

                // Raise the exception to inform caller about it
                if (doRaise) { throw; }
            }
        }

        /// <summary>
        /// Gets or sets the synchronization context on which to publish all messages.
        /// </summary>
        public SynchronizationContext SyncContext
        {
            get { return m_syncContext; }
        }

        /// <summary>
        /// Gets the current threading behavior of this Messenger.
        /// </summary>
        public FrozenSkyMessageThreadingBehavior ThreadingBehavior
        {
            get { return m_checkBehavior; }
        }

        /// <summary>
        /// Gets the name of the associated thread.
        /// </summary>
        public string MainThreadName
        {
            get
            {
                return m_messengerName;
            }
        }

        /// <summary>
        /// Counts all message subscriptions.
        /// </summary>
        public int CountSubscriptions
        {
            get
            {
                lock (m_messageSubscriptionsLock)
                {
                    int totalCount = 0;
                    foreach (var actKeyValuePair in m_messageSubscriptions)
                    {
                        totalCount += actKeyValuePair.Value.Count;
                    }
                    return totalCount;
                }
            }
        }

        /// <summary>
        /// Gets the total count of globally registered messengers.
        /// </summary>
        public static int CountGlobalMessengers
        {
            get { return s_messengersByName.Count; }
        }
    }
}
