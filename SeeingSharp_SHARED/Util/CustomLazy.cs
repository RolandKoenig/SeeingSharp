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
using System;
using System.Threading;

#if WINRT || UNIVERSAL
using Windows.System.Threading;
#endif

namespace SeeingSharp.Util
{
    /// <summary>
    /// Provides support for lazy initialization.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    public class CustomLazy<T>
    {
        private readonly object m_lockObject = new object();
        private readonly Func<T> m_factoryFunc;
        private volatile bool m_isValueCreated;
        private volatile bool m_loadObjectAyncDirectly;
        private EventWaitHandle m_asyncLoadWaitHandle;
        private Exception m_asyncLoadException;
        private T m_value;

        /// <summary>
        /// Initializes a new instance of the Lazy{T} class.
        /// </summary>
        /// <param name="createValue">The delegate that produces the value when it is needed.</param>
        /// <param name="loadObjectAsyncDirectly">When true, this triggers async loading using ThreadPool directly.</param>
        public CustomLazy(Func<T> createValue, bool loadObjectAsyncDirectly = false)
        {
            if (createValue == null) { throw new ArgumentNullException("createValue"); }

            this.m_factoryFunc = createValue;
            this.m_loadObjectAyncDirectly = loadObjectAsyncDirectly;

            TriggerAsyncLoading();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLazy&lt;T&gt;"/> class.
        /// </summary>
        public CustomLazy()
            : this(() => default(T))
        {
        }

        /// <summary>
        /// An implicit operator that converts a lazy object to its value.
        /// </summary>
        /// <param name="lazyObject">The lazy object.</param>
        public static implicit operator T(CustomLazy<T> lazyObject)
        {
            return lazyObject.Value;
        }

        /// <summary>
        /// Creates and returns a string representation of the Lazy{T}.Value.
        /// </summary>
        /// <returns>The string representation of the Lazy{T}.Value property.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        public void Reset()
        {
            //Wait for async loading
            if (m_asyncLoadWaitHandle != null)
            {
                m_asyncLoadWaitHandle.WaitOne();
                m_asyncLoadWaitHandle = null;
            }

            m_isValueCreated = false;
            m_value = default(T);

            TriggerAsyncLoading();
        }

        /// <summary>
        /// Triggers async loading of the object.
        /// </summary>
        private void TriggerAsyncLoading()
        {
            //Trigger async loading (if requested)
            if (m_loadObjectAyncDirectly)
            {
                m_asyncLoadWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

#if DESKTOP || ANDROID
                ThreadPool.QueueUserWorkItem((arg) =>
                {
                    try { m_value = m_factoryFunc(); }
                    catch (Exception ex)
                    {
                        m_asyncLoadException = ex;
                    }
                    m_isValueCreated = true;
                    m_asyncLoadWaitHandle.Set();
                });
#else
                ThreadPool.RunAsync((arg) =>
                {
                    try { m_value = m_factoryFunc(); }
                    catch (Exception ex)
                    {
                        m_asyncLoadException = ex;
                    }
                    m_isValueCreated = true;
                    m_asyncLoadWaitHandle.Set();
                }).FireAndForget();
#endif
            }
        }

        /// <summary>
        /// Gets the lazily initialized value of the current Lazy{T} instance.
        /// </summary>
        public T Value
        {
            get
            {
                //Wait for async loading
                if (m_asyncLoadWaitHandle != null)
                {
                    m_asyncLoadWaitHandle.WaitOne();
                    m_asyncLoadWaitHandle = null;
                }

                //Throw exception occurred during async load
                if (m_asyncLoadException != null) { throw new SeeingSharpException("Unhandled exception while loading object of type " + typeof(T).FullName + ": " + m_asyncLoadException.Message, m_asyncLoadException); }

                //Try to return or dynamically create the object
                if (!m_isValueCreated)
                {
                    lock (m_lockObject)
                    {
                        if (!m_isValueCreated)
                        {
                            m_value = m_factoryFunc();
                            m_isValueCreated = true;
                        }
                    }
                }
                return m_value;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether a value has been created for this Lazy{T} instance.
        /// </summary>
        public bool IsValueCreated
        {
            get
            {
                lock (m_lockObject)
                {
                    return m_isValueCreated;
                }
            }
        }
    }
}