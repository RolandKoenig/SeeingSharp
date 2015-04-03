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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using FrozenSky.Util;
using System.Threading.Tasks;

#if DESKTOP
// Some namespace mappings
using WinForms = System.Windows.Forms;
#endif 

namespace FrozenSky.Infrastructure
{
    public class FrozenSkyApplication
    {
        public static readonly int BOOT_ORDER_ID_GRAPHICS = 1000;
        public static readonly int BOOT_ORDER_ID_BASE_FRAMEWORK = 0;
        public static readonly int BOOT_ORDER_ID_UI = 5000;
        public static readonly int BOOT_ORDER_ID_CUSTOM_APP = 10000;

        private static FrozenSkyApplication s_current;

        private Assembly m_mainAssembly;
        private IEnumerable<Assembly> m_otherAssemblies;
        private string[] m_startupArguments;
        private Dictionary<Type, object> m_services;
        private Bootstrapper m_bootstrapper;
        private SingletonContainer m_singletons;
        private TypeQueryHandler m_assemblyQuery;
        private FrozenSkyTranslator m_translator;

        // Main ApplicationMessageHandler (responsible for EventAggregator pattern)
        private FrozenSkyMessageHandler m_uiMessageHandler;

        // Main UI SynchronizationContext
        private SynchronizationContext m_uiSynchronizationContext;

        /// <summary>
        /// Prevents a default instance of the <see cref="FrozenSkyApplication" /> class from being created.
        /// </summary>
        private FrozenSkyApplication()
        {
            m_services = new Dictionary<Type, object>();
            m_singletons = new SingletonContainer();

            m_singletons.RegisterSingleton(this);
        }

        /// <summary>
        /// Initializes the RKApplication object.
        /// </summary>
        /// <param name="mainAssembly">The main assembly of the application.</param>
        /// <param name="otherAssemblies">All other assemblies which should be search during TypeQuery.</param>
        public static async Task InitializeAsync(Assembly mainAssembly, IEnumerable<Assembly> otherAssemblies, string[] startupArguments)
        {
            if (s_current != null) { throw new FrozenSkyException("RKApplication is already initialized!"); }

            if (otherAssemblies == null) { otherAssemblies = new List<Assembly>(); }

            // Do all initializations
            FrozenSkyApplication newApplication = new FrozenSkyApplication();
            newApplication.m_mainAssembly = mainAssembly;
            newApplication.m_otherAssemblies = otherAssemblies;
            newApplication.m_startupArguments = startupArguments;
            newApplication.m_translator = new FrozenSkyTranslator();
            newApplication.m_assemblyQuery = new TypeQueryHandler();
            newApplication.m_assemblyQuery.QueryTypes(newApplication.AppAssemblies);

            // Apply created instance
            s_current = newApplication;

            // Perform bootstrapping
            newApplication.m_bootstrapper = new Bootstrapper();
            newApplication.m_bootstrapper.LoadBootstrapperItems();
            await newApplication.m_bootstrapper.Run();
        }

#if DESKTOP
        /// <summary>
        /// Creates all default services.
        /// </summary>
        /// <param name="mainWindow">The mainwindow instance.</param>
        public void CreateDefaultServices(Window mainWindow)
        {
            //TODO..
        }
#endif

        /// <summary>
        /// Initializes the UI environment.
        /// </summary>
        public void InitializeUIEnvironment()
        {
            // Get and check ui synchronization context
            m_uiSynchronizationContext = SynchronizationContext.Current;
            if ((m_uiSynchronizationContext == null) ||
                (m_uiSynchronizationContext.GetType() == typeof(SynchronizationContext)))
            {
                throw new InvalidOperationException("Unable to initialize RKApplication object: No valid UI SynchronizationContext found!");
            }

            // Set the name on the UI thread
#if DESKTOP
            Thread.CurrentThread.Name = FrozenSkyConstants.THREAD_NAME_GUI;
#endif

            // Create the UI message handler
            m_uiMessageHandler = new FrozenSkyMessageHandler();
            m_uiMessageHandler.ApplyThreadSynchronization(
                FrozenSkyMessageThreadingBehavior.EnsureMainThreadOnSyncCalls,
                FrozenSkyConstants.THREAD_NAME_GUI,
                m_uiSynchronizationContext);
        }

        public T GetSingleton<T>()
            where T : class
        {
            return m_singletons.GetSingleton<T>();
        }

        public T GetSingleton<T>(string name)
            where T : class
        {
            return m_singletons.GetSingleton<T>(name);
        }

        /// <summary>
        /// Registers a new singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton to create an object of.</typeparam>
        public void RegisterSingleton<T>()
            where T : class, new()
        {
            m_singletons.RegisterSingleton<T>();
        }

        /// <summary>
        /// Registers a new singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton to create an object of.</typeparam>
        /// <param name="singletonObject">The object to register.</param>
        public void RegisterSingleton<T>(T singletonObject)
            where T : class
        {
            m_singletons.RegisterSingleton(singletonObject);
        }

        ///// <summary>
        ///// Registers a new item for the bootstraper.
        ///// </summary>
        ///// <typeparam name="T">The type of the item to register.</typeparam>
        //public void RegisterBootstrapperItem<T>()
        //    where T : IBootstrapperItem, new()
        //{
        //    m_bootstrapper.RegisterBootstrapperItem<T>();
        //}

        ///// <summary>
        ///// Registers a new item for the bootstraper.
        ///// </summary>
        ///// <typeparam name="T">The type of the item to register.</typeparam>
        ///// <param name="bootstrapperItemToAdd">The item to register.</param>
        //public void RegisterBootstrapperItem<T>(T bootstrapperItemToAdd)
        //    where T : IBootstrapperItem
        //{
        //    m_bootstrapper.RegisterBootstrapperItem(bootstrapperItemToAdd);
        //}

        /// <summary>
        /// Registers the given service.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="service">The service object to register.</param>
        public void RegisterService<T>(T service)
            where T : class
        {
            Type serviceType = typeof(T);

#if DESKTOP
            if (!serviceType.IsInterface) { throw new FrozenSkyException("Service type musst be an interface!"); }
#endif
#if WINRT
            if (!serviceType.GetTypeInfo().IsInterface) { throw new FrozenSkyException("Service type musst be an interface!"); }
#endif
            if (service == null) { throw new ArgumentNullException("service"); }

            m_services[serviceType] = service;
        }

        /// <summary>
        /// Tries to get the service of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service to get.</typeparam>
        public void TryGetService<T>(ref T serviceReference)
            where T : class
        {
            serviceReference = TryGetService<T>();
        }

        /// <summary>
        /// Tries to get the service of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service to get.</typeparam>
        public T TryGetService<T>()
            where T : class
        {
            Type serviceType = typeof(T);

#if DESKTOP
            if (!serviceType.IsInterface) { throw new FrozenSkyException("Service type musst be an interface!"); }
#elif WINRT
            if (!serviceType.GetTypeInfo().IsInterface) { throw new FrozenSkyException("Service type musst be an interface!"); }
#endif

            //Tries to return the service of the given service type
            if (m_services.ContainsKey(serviceType)) { return m_services[serviceType] as T; }

            return null;
        }

        /// <summary>
        /// Gets the service of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        public void GetService<T>(ref T serviceReference)
            where T : class
        {
            serviceReference = GetService<T>();
        }

        /// <summary>
        /// Gets the service of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        public T GetService<T>()
            where T : class
        {
            T result = TryGetService<T>();
            if (result == null) { throw new FrozenSkyException("Service " + typeof(T).FullName + " not found!"); }
            return result;
        }

        /// <summary>
        /// Helper method.
        /// </summary>
        private static IEnumerable<Assembly> GetAssemblyEnumerable(Assembly mainAssembly, IEnumerable<Assembly> otherAssemblies)
        {
            yield return mainAssembly;
            if (otherAssemblies != null)
            {
                foreach (var actOtherAssembly in otherAssemblies)
                {
                    yield return actOtherAssembly;
                }
            }
        }

        /// <summary>
        /// Gets the current instance of this application.
        /// </summary>
        public static FrozenSkyApplication Current
        {
            get
            {
                if (s_current == null)
                {
                    throw new FrozenSkyException("RKApplication object not initialized!");
                }
                return s_current;
            }
        }

        /// <summary>
        /// Is the application initialized?
        /// </summary>
        public static bool IsInitialized
        {
            get { return s_current != null; }
        }

        public static bool IsUIEnvironmentInitialized
        {
            get
            {
                if (s_current == null) { return false; }
                return s_current.m_uiMessageHandler != null;
            }
        }

        /// <summary>
        /// Gets the name of this product.
        /// </summary>
        public string ProductName
        {
            get
            {
                var productAttribute = m_mainAssembly.GetCustomAttribute<AssemblyProductAttribute>();

                if (productAttribute != null) { return productAttribute.Product; }
                else { return string.Empty; }
            }
        }

        /// <summary>
        /// Gets the version of this product.
        /// </summary>
        public string ProductVersion
        {
            get
            {
                var versionAttribute = m_mainAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                if (versionAttribute != null) { return versionAttribute.InformationalVersion; }
                else { return string.Empty; }
            }
        }

        /// <summary>
        /// Gets a collection containing all startup arguments passed by the caller of this program.
        /// </summary>
        public IEnumerable<string> StartupArguments
        {
            get { return m_startupArguments; }
        }

        /// <summary>
        /// A list containing all assemblies given during initialization.
        /// </summary>
        public IEnumerable<Assembly> AppAssemblies
        {
            get { return GetAssemblyEnumerable(m_mainAssembly, m_otherAssemblies); }
        }

        /// <summary>
        /// Gets the message handler of the ui.
        /// </summary>
        public FrozenSkyMessageHandler UIMessageHandler
        {
            get { return m_uiMessageHandler; }
        }

        /// <summary>
        /// Gets the object which is responsible for type query.
        /// </summary>
        public TypeQueryHandler TypeQuery
        {
            get { return m_assemblyQuery; }
        }

        /// <summary>
        /// Gets the current translator object.
        /// </summary>
        public FrozenSkyTranslator Translator
        {
            get { return m_translator; }
        }

        /// <summary>
        /// Gets the application's bootstrapper.
        /// </summary>
        public Bootstrapper Bootstrapper
        {
            get { return m_bootstrapper; }
        }

        /// <summary>
        /// Gets a container holding all registered singletons.
        /// </summary>
        public SingletonContainer Singletons
        {
            get { return m_singletons; }
        }

        public SynchronizationContext UISyncContext
        {
            get { return m_uiSynchronizationContext; }
        }
    }
}