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
using SeeingSharp.Util;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if DESKTOP
using System.Windows;
using System.Windows.Media;
#endif
#if UNIVERSAL
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace SeeingSharp.View
{
    public class ViewServiceNode
    {
        private FrameworkElement m_host;
        private ViewModelBase m_lastViewModel;
#if UNIVERSAL
        private bool m_isHostLoaded;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewServiceNode"/> class.
        /// </summary>
        public ViewServiceNode(FrameworkElement host)
        {
            m_host = host;
            m_host.DataContextChanged += OnHost_DataContextChanged;
            m_host.DataContextChanged += OnHost_DataContextChanged;
            m_host.Loaded += OnHost_Loaded;
            m_host.Unloaded += OnHost_Unloaded;
        }

        private bool TryFillViewServiceImplementation(FrameworkElement element, ViewServiceRequestEventArgs e)
        {
            ViewServiceCollection viewServices = element.GetValue(
                Interaction.ViewServicesProperty) as ViewServiceCollection;
            if (viewServices == null) { return false; }

            // Try to find the view service within the host control
            foreach (object actViewService in viewServices)
            {
                if (actViewService == null) { continue; }

                Type actViewServiceType = actViewService.GetType();
                if (actViewServiceType.GetTypeInfo().IsAssignableFrom(e.RequestedType.GetTypeInfo()))
                {
                    e.Implementation = actViewService;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Called when the viewmodel request a view service.
        /// </summary>
        private void OnViewModel_ViewServiceRequest(object sender, ViewServiceRequestEventArgs e)
        {
            // Check for an implementation on the hosted control
            if(TryFillViewServiceImplementation(m_host, e))
            {
                return;
            }

            // Try to walk the visual tree up until we find a view service implementation
            DependencyObject actParent = VisualTreeHelper.GetParent(m_host);
            FrameworkElement actParentElement = actParent as FrameworkElement;
            while((actParent != null) && (actParentElement == null))
            {
                if((actParentElement != null) &&
                   (TryFillViewServiceImplementation(actParentElement, e)))
                {
                    return;
                } 
                

                actParent = VisualTreeHelper.GetParent(m_host);
                actParentElement = actParent as FrameworkElement;
            }
        }


        private void OnHost_Loaded(object sender, RoutedEventArgs e)
        {
#if UNIVERSAL
            m_isHostLoaded = true;
#endif

            if (m_lastViewModel != null)
            {
                m_lastViewModel.ViewServiceRequest += OnViewModel_ViewServiceRequest;
            }
        }

        private void OnHost_Unloaded(object sender, RoutedEventArgs e)
        {
#if UNIVERSAL
            m_isHostLoaded = false;
#endif

            if(m_lastViewModel != null)
            {
                m_lastViewModel.ViewServiceRequest -= OnViewModel_ViewServiceRequest;
            }
        }

#if UNIVERSAL
        private void OnHost_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            ViewModelBase newModel = args.NewValue as ViewModelBase;
            if (newModel == m_lastViewModel) { return; }

            if ((m_lastViewModel != null) &&
                (m_isHostLoaded))
            {
                m_lastViewModel.ViewServiceRequest -= OnViewModel_ViewServiceRequest;
            }
            m_lastViewModel = newModel;
            if ((m_lastViewModel != null) &&
                (m_isHostLoaded))
            {
                m_lastViewModel.ViewServiceRequest += OnViewModel_ViewServiceRequest;
            }
        }
#endif

#if DESKTOP
        private void OnHost_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModelBase newModel = e.NewValue as ViewModelBase;
            if(newModel == m_lastViewModel) { return; }
             
            if((m_lastViewModel != null) &&
               (m_host.IsLoaded))
            {
                m_lastViewModel.ViewServiceRequest -= OnViewModel_ViewServiceRequest;
            }
            m_lastViewModel = newModel;
            if((m_lastViewModel != null) &&
               (m_host.IsLoaded))
            {
                m_lastViewModel.ViewServiceRequest += OnViewModel_ViewServiceRequest;
            }
        }
#endif
    }
}
