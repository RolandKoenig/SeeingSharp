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

using System;
using System.Collections.Generic;

namespace SeeingSharp.Infrastructure
{
    public class SingletonContainer
    {
        private Dictionary<Type, object> m_singletons;
        private Dictionary<string, object> m_singletonsByName;
        private object m_singletonsLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonContainer" /> class.
        /// </summary>
        public SingletonContainer()
        {
            m_singletons = new Dictionary<Type, object>();
            m_singletonsByName = new Dictionary<string, object>();
            m_singletonsLock = new object();
        }

        /// <summary>
        /// Registers a new singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton to create an object of.</typeparam>
        public void RegisterSingleton<T>()
            where T : class, new()
        {
            this.RegisterSingleton(new T());
        }

        /// <summary>
        /// Registers a new singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton to create an object of.</typeparam>
        /// <param name="singletonObject">The object to register.</param>
        public void RegisterSingleton<T>(T singletonObject)
            where T : class
        {
            if (singletonObject == null) { throw new ArgumentNullException("singletonObject"); }

            lock (m_singletonsLock)
            {
                if (m_singletons.ContainsKey(typeof(T))) { throw new InvalidOperationException("There is already a singleton registered for type " + typeof(T).FullName + "!"); }

                m_singletons[typeof(T)] = singletonObject;
                m_singletonsByName[typeof(T).Name] = singletonObject;
            }
        }

        /// <summary>
        /// Registers a singleton on the given name.
        /// </summary>
        /// <typeparam name="T">The type of the singleton.</typeparam>
        /// <param name="singletonObject">The object to register.</param>
        /// <param name="name">The name for the singleton</param>
        public void RegisterSingleton<T>(T singletonObject, string name)
        {
            lock (m_singletonsLock)
            {
                if (m_singletonsByName.ContainsKey(name)) { throw new InvalidOperationException("There is already a singleton registered for name " + name + "!"); }

                m_singletonsByName[name] = singletonObject;
            }
        }

        /// <summary>
        /// Is there any singleton with the given name?
        /// </summary>
        /// <param name="singletonName">The name of the singleton.</param>
        public bool ContainsSingleton(string singletonName)
        {
            lock (m_singletonsLock)
            {
                return m_singletonsByName.ContainsKey(singletonName);
            }
        }

        /// <summary>
        /// Gets the singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton.</typeparam>
        public T GetSingleton<T>()
            where T : class
        {
            lock (m_singletonsLock)
            {
                return m_singletons[typeof(T)] as T;
            }
        }

        /// <summary>
        /// Gets the singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton.</typeparam>
        public T GetSingleton<T>(string name)
            where T : class
        {
            lock (m_singletonsLock)
            {
                T result = m_singletonsByName[name] as T;
                if (result == null) { throw new ArgumentException("Singleton with name " + name + " and type " + typeof(T).Name + " not found!"); }
                return result;
            }
        }

        /// <summary>
        /// Tries to get the singleton of the given type.
        /// Null is returned when the singleton is not available.
        /// </summary>
        /// <typeparam name="T">The type of the singleton.</typeparam>
        public T TryGetSingleton<T>()
            where T : class
        {
            lock (m_singletonsLock)
            {
                Type type = typeof(T);
                if (!m_singletons.ContainsKey(type)) { return null; }

                return m_singletons[typeof(T)] as T;
            }
        }

        /// <summary>
        /// Is there any singleton with the given type?
        /// </summary>
        /// <param name="singletonName">The type of the singleton.</param>
        internal bool ContainsSingleton(Type type)
        {
            lock (m_singletonsLock)
            {
                return m_singletons.ContainsKey(type);
            }
        }

        /// <summary>
        /// Gets the singleton object of the given type.
        /// </summary>
        /// <param name="typeOfSingleton">The type of the singleton.</param>
        public object this[Type typeOfSingleton]
        {
            get
            {
                lock (m_singletonsLock)
                {
                    return m_singletons[typeOfSingleton];
                }
            }
        }

        /// <summary>
        /// Gets the singleton with the given name.
        /// </summary>
        /// <param name="name">The name of the singleton.</param>
        public object this[string name]
        {
            get
            {
                lock (m_singletonsLock)
                {
                    return m_singletonsByName[name];
                }
            }
        }
    }
}