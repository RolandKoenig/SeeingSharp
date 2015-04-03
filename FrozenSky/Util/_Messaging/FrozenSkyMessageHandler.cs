#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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

namespace FrozenSky.Util
{
    /// <summary>
    /// Main class for application messaging. 
    /// </summary>
    public class FrozenSkyMessageHandler
    {
        /// <summary>
        /// Gets or sets a custom exception handler which is used globally.
        /// Return true to skip default exception behavior (exception is thrown to publisher by default).
        /// </summary>
        public static Func<FrozenSkyMessageHandler, Exception, bool> CustomPublishExceptionHandler;

        // Global threading synchronization (enables the possibility to publish a message over more threads)
        private static ConcurrentDictionary<string, FrozenSkyMessageHandler> s_handlersByThreadName;

        // Global information about message routing
        private static ConcurrentDictionary<Type, string[]> s_messagesAsyncTargets;
        private static ConcurrentDictionary<Type, string[]> s_messageSources;

        // Threading
        private string m_mainThreadName;
        private SynchronizationContext m_syncContext;
        private FrozenSkyMessageThreadingBehavior m_threadBehavior;

        // Message subscriptions
        private Dictionary<Type, List<MessageSubscription>> m_messageSubscriptions;
        private object m_messageSubscriptionsLock;

        /// <summary>
        /// Initializes the <see cref="FrozenSkyMessageHandler" /> class.
        /// </summary>
        static FrozenSkyMessageHandler()
        {
            s_handlersByThreadName = new ConcurrentDictionary<string, FrozenSkyMessageHandler>();

            s_messagesAsyncTargets = new ConcurrentDictionary<Type, string[]>();
            s_messageSources = new ConcurrentDictionary<Type, string[]>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrozenSkyMessageHandler"/> class.
        /// </summary>
        public FrozenSkyMessageHandler()
        {
            m_mainThreadName = string.Empty;
            m_syncContext = null;
            m_threadBehavior = FrozenSkyMessageThreadingBehavior.EnsureMainThreadOnSyncCalls;

            m_messageSubscriptions = new Dictionary<Type, List<MessageSubscription>>();
            m_messageSubscriptionsLock = new object();
        }

        /// <summary>
        /// Gets the FrozenSkyMessageHandler for the thread with the given name.
        /// </summary>
        /// <param name="threadName">The name of the thread.</param>
        public static FrozenSkyMessageHandler GetForThread(string threadName)
        {
            var result = TryGetForThread(threadName);
            if (result == null) { throw new FrozenSkyException("Unable to find MessageHandler for thread " + threadName + "!"); }
            return result;
        }

        /// <summary>
        /// Gets the FrozenSkyMessageHandler for the thread with the given name.
        /// </summary>
        /// <param name="threadName">The name of the thread.</param>
        public static FrozenSkyMessageHandler TryGetForThread(string threadName)
        {
            FrozenSkyMessageHandler result = null;
            s_handlersByThreadName.TryGetValue(threadName, out result);
            return result;
        }

        /// <summary>
        /// Sets all required threading properties based on the given target thread.
        /// </summary>
        /// <param name="targetThread">The thread on which this MessageHandler should work on.</param>
        public void ApplyThreadSynchronization(ObjectThread targetThread)
        {
            ApplyThreadSynchronization(
                FrozenSkyMessageThreadingBehavior.EnsureMainThreadOnSyncCalls,
                targetThread.Name,
                targetThread.SyncContext);
        }

        /// <summary>
        /// Sets all required threading properties.
        /// </summary>
        /// <param name="threadBehavior">Defines the threading behavior.</param>
        /// <param name="threadName">The name of the thread to be registered.</param>
        /// <param name="syncContext">The synchronization context to be used.</param>
        public void ApplyThreadSynchronization(FrozenSkyMessageThreadingBehavior threadBehavior, string threadName, SynchronizationContext syncContext)
        {
            m_mainThreadName = threadName;
            m_threadBehavior = threadBehavior;
            m_syncContext = syncContext;

            if (!string.IsNullOrEmpty(threadName))
            {
                s_handlersByThreadName.TryAdd(threadName, this);
            }
        }

        /// <summary>
        /// Clears all thread synchronization options.
        /// </summary>
        public void DiscardThreadSynchronization()
        {
            if (string.IsNullOrEmpty(m_mainThreadName)) { return; }

            FrozenSkyMessageHandler dummyResult = null;
            s_handlersByThreadName.TryRemove(m_mainThreadName, out dummyResult);

            m_threadBehavior = FrozenSkyMessageThreadingBehavior.EnsureMainThreadOnSyncCalls;
            m_mainThreadName = string.Empty;
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

        /// <summary>
        /// Subscribes to the given MessageType.
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="messageHandler">Action to perform on incoming message.</param>
        public MessageSubscription Subscribe<MessageType>(Action<MessageType> messageHandler)
            where MessageType : FrozenSkyMessage
        {
            Type currentType = typeof(MessageType);
            return this.Subscribe(messageHandler, currentType);
        }

        /// <summary>
        /// Subscribes the given MessageType and executes the action only when the condition is true.
        /// </summary>
        /// <typeparam name="MessageType">The type of the message type.</typeparam>
        /// <param name="condition">The condition.</param>
        /// <param name="messageHandler">The message handler.</param>
        /// <returns></returns>
        public MessageSubscription SubscribeWhen<MessageType>(Func<MessageType, bool> condition, Action<MessageType> messageHandler)
            where MessageType : FrozenSkyMessage
        {
            Action<MessageType> filterAction = (message) =>
            {
                if (condition(message))
                {
                    messageHandler(message);
                }
            };

            Type currentType = typeof(MessageType);
            return this.Subscribe(filterAction, currentType);
        }

        /// <summary>
        /// Subscribes to the given message type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="messageHandler">Action to perform on incoming message.</param>
        public MessageSubscription Subscribe(
            Delegate messageHandler, Type messageType)
        {
            if (!messageType.GetTypeInfo().IsSubclassOf(typeof(FrozenSkyMessage))) { throw new ArgumentException("Given message type does not derive from FrozenSkyMessage!"); }

            MessageSubscription newOne = new MessageSubscription(this, messageType, messageHandler);
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
        /// <param name="messageHandler">Action to perform on incoming message.</param>
        /// <param name="target">The target control.</param>
        public void SubscribeOnControl<MessageType>(System.Windows.Forms.Control target, Action<MessageType> messageHandler)
            where MessageType : FrozenSkyMessage
        {
            MessageSubscription subscription = null;

            //Create handler delegates
            EventHandler onHandleCreated = (sender, eArgs) =>
            {
                if (subscription == null)
                {
                    subscription = Subscribe(messageHandler);
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
                subscription = Subscribe(messageHandler);
            }
        }
#endif

        /// <summary>
        /// Clears the given MessageSubscription.
        /// </summary>
        /// <param name="messageSubscription">The subscription to clear.</param>
        public void Unsubscribe(MessageSubscription messageSubscription)
        {
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
            try
            {
                // Check whether publich is possible
                if ((m_threadBehavior == FrozenSkyMessageThreadingBehavior.EnsureMainThreadOnSyncCalls) &&
                    (!string.IsNullOrEmpty(m_mainThreadName)))
                {
#if DESKTOP
                    if (Thread.CurrentThread.Name != m_mainThreadName) { throw new MessagePublishException(typeof(MessageType), "Synchronous publish is not possible because caller comes from another thread!"); }
#else
                    throw new FrozenSkyException(
                        "Unable to check by thread during publish on platforms other than DESKTOP. " +
                        "Please don't use " + m_threadBehavior + "!");
#endif
                }
                else if(m_threadBehavior == FrozenSkyMessageThreadingBehavior.EnsureMainSyncContextOnSyncCalls)
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
                        string mainThreadName = m_mainThreadName;
                        if (string.IsNullOrEmpty(mainThreadName) ||
                            (Array.IndexOf(possibleSources, mainThreadName) < 0))
                        {
                            throw new InvalidOperationException(string.Format(
                                "The message of type {0} can only be sent by the threads {1}. This MessageHandler belongs to the thread {2}, so no publish possible!",
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
                    string mainThreadName = m_mainThreadName;
                    for (int loop = 0; loop < asyncTargets.Length; loop++)
                    {
                        string actAsyncTargetName = asyncTargets[loop];
                        if (mainThreadName == actAsyncTargetName) { continue; }

                        FrozenSkyMessageHandler actAsyncTargetHandler = null;
                        if (s_handlersByThreadName.TryGetValue(actAsyncTargetName, out actAsyncTargetHandler))
                        {
                            SynchronizationContext actSyncContext = actAsyncTargetHandler.m_syncContext;
                            if (actSyncContext == null) { continue; }

                            FrozenSkyMessageHandler innerHandlerForAsyncCall = actAsyncTargetHandler;
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
                var globalExceptionHandler = FrozenSkyMessageHandler.CustomPublishExceptionHandler;
                bool doRaise = true;
                if (globalExceptionHandler != null)
                {
                    try
                    {
                        doRaise = !globalExceptionHandler(this, ex);
                    }
                    catch { }
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
        /// Gets the current threading behavior of this MessageHandler.
        /// </summary>
        public FrozenSkyMessageThreadingBehavior ThreadingBehavior
        {
            get { return m_threadBehavior; }
        }

        /// <summary>
        /// Gets the name of the associated thread.
        /// </summary>
        public string MainThreadName
        {
            get
            {
                return m_mainThreadName;
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
        /// Gets the total count of globally registered message handlers.
        /// </summary>
        public static int CountGlobalMessageHandlers
        {
            get { return s_handlersByThreadName.Count; }
        }
    }
}
