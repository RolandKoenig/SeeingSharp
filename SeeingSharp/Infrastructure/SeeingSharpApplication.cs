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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using SeeingSharp.Util;
using SeeingSharp.Checking;
using SeeingSharp.View;

#if DESKTOP
// Some namespace mappings
using WinForms = System.Windows.Forms;
#endif 

namespace SeeingSharp.Infrastructure
{
    public class SeeingSharpApplication
    {
        public static readonly int BOOT_ORDER_ID_GRAPHICS = 1000;
        public static readonly int BOOT_ORDER_ID_BASE_FRAMEWORK = 0;
        public static readonly int BOOT_ORDER_ID_UI = 5000;
        public static readonly int BOOT_ORDER_ID_CUSTOM_APP = 10000;

        private static SeeingSharpApplication s_current;

        private Assembly m_mainAssembly;
        private IEnumerable<Assembly> m_otherAssemblies;
        private string[] m_startupArguments;
        private Dictionary<Type, object> m_services;
        private Bootstrapper m_bootstrapper;
        private SingletonContainer m_singletons;
        private TypeQueryHandler m_assemblyQuery;
        private SeeingSharpTranslator m_translator;

        // Main ApplicationMessenger (responsible for EventAggregator pattern)
        private SeeingSharpMessenger m_uiMessenger;

        // Main UI SynchronizationContext
        private SynchronizationContext m_uiSynchronizationContext;

        /// <summary>
        /// Prevents a default instance of the <see cref="SeeingSharpApplication" /> class from being created.
        /// </summary>
        private SeeingSharpApplication()
        {
            m_services = new Dictionary<Type, object>();
            m_singletons = new SingletonContainer();

            m_singletons.RegisterSingleton(this);
        }

        /// <summary>
        /// Initializes the SeeingSharpApplication object for UnitTests.
        /// </summary>
        public static void InitializeForUnitTests()
        {
            // Do not throw any exception if Initialize was called before
            if (s_current != null) { return; }

            InitializeAsync(
                typeof(SeeingSharpApplication).GetTypeInfo().Assembly,
                new Assembly[0],
                new string[0]).Wait();

        }

        /// <summary>
        /// Initializes the SeeingSharpApplication object.
        /// </summary>
        /// <param name="mainAssembly">The main assembly of the application.</param>
        /// <param name="otherAssemblies">All other assemblies which should be search during TypeQuery.</param>
        /// <param name="startupArguments">All arguments passed to this application.</param>
        public static async Task InitializeAsync(Assembly mainAssembly, IEnumerable<Assembly> otherAssemblies, string[] startupArguments)
        {
            if (s_current != null) { throw new SeeingSharpException("RKApplication is already initialized!"); }

            if (otherAssemblies == null) { otherAssemblies = new List<Assembly>(); }

            // Ensure that the SeeingSharp assembly is contained in otherAssemblies list
            Assembly thisAssembly = typeof(SeeingSharpApplication).GetTypeInfo().Assembly;
            List<Assembly> otherAssembliesList = new List<Assembly>(otherAssemblies);
            if (!otherAssembliesList.Contains(thisAssembly)) { otherAssembliesList.Add(thisAssembly); }

            // Do all initializations
            SeeingSharpApplication newApplication = new SeeingSharpApplication();
            newApplication.m_mainAssembly = mainAssembly;
            newApplication.m_otherAssemblies = otherAssembliesList;
            newApplication.m_startupArguments = startupArguments;
            newApplication.m_translator = new SeeingSharpTranslator();
            newApplication.m_assemblyQuery = new TypeQueryHandler();
            newApplication.m_assemblyQuery.QueryTypes(newApplication.AppAssemblies);

            // Apply created instance
            s_current = newApplication;

            // Perform bootstrapping
            newApplication.m_bootstrapper = new Bootstrapper();
            newApplication.m_bootstrapper.LoadBootstrapperItems();
            await newApplication.m_bootstrapper.RunAsync(newApplication)
                .ConfigureAwait(false);
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

#if DESKTOP        
        /// <summary>
        /// Initializes the automatic error reporting for Wpf applications.
        /// </summary>
        /// <param name="app">The main application object.</param>
        /// <param name="mainWindow">The window that acts as the host of the error dialog.</param>
        public void InitializeAutoErrorReporting_Wpf(System.Windows.Application app, System.Windows.Window mainWindow)
        {
            app.EnsureNotNull(nameof(app));

            app.DispatcherUnhandledException += (sender, eArgs) =>
            {
                ExceptionInfo exInfo = new ExceptionInfo(eArgs.Exception);
                eArgs.Handled = true;

                SeeingSharpWpfErrorDialog.ShowDialog(mainWindow, exInfo);
            };
        }

        /// <summary>
        /// Initializes the automatic error reporting win forms.
        /// </summary>
        /// <param name="mainForm">The form that acts as the host of the error dialog.</param>
        public void InitializeAutoErrorReporting_WinForms(System.Windows.Forms.Form mainForm)
        {
            System.Windows.Forms.Application.ThreadException += (sender, eArgs) =>
            {
                if(eArgs.Exception != null)
                {
                    ExceptionInfo exInfo = new ExceptionInfo(eArgs.Exception);

                }
                else
                {

                }
            };
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

            // Create the UI messenger
            m_uiMessenger = new SeeingSharpMessenger();
            m_uiMessenger.ApplyForGlobalSynchronization(
                SeeingSharpMessageThreadingBehavior.EnsureMainSyncContextOnSyncCalls,
                SeeingSharpConstants.THREAD_NAME_GUI,
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
            if (!serviceType.IsInterface) { throw new SeeingSharpException("Service type musst be an interface!"); }
#endif
#if WINRT
            if (!serviceType.GetTypeInfo().IsInterface) { throw new SeeingSharpException("Service type musst be an interface!"); }
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
            if (!serviceType.IsInterface) { throw new SeeingSharpException("Service type musst be an interface!"); }
#elif WINRT
            if (!serviceType.GetTypeInfo().IsInterface) { throw new SeeingSharpException("Service type musst be an interface!"); }
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
            if (result == null) { throw new SeeingSharpException("Service " + typeof(T).FullName + " not found!"); }
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
        public static SeeingSharpApplication Current
        {
            get
            {
                if (s_current == null)
                {
                    throw new SeeingSharpException("RKApplication object not initialized!");
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
                return s_current.m_uiMessenger != null;
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
        /// Gets the messenger of the ui.
        /// </summary>
        public SeeingSharpMessenger UIMessenger
        {
            get { return m_uiMessenger; }
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
        public SeeingSharpTranslator Translator
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