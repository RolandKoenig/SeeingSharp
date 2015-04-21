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
using FrozenSky.Infrastructure;
using FrozenSky.Multimedia.Core;
using FrozenSky.Samples.Base;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalSampleContainer
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region all collections
        private List<SampleViewModel> m_allSamples;
        private List<string> m_categories;
        private ObservableCollection<SampleViewModel> m_visibleSamples;
        #endregion

        #region current selection
        private string m_selectedCategory;
        private SampleViewModel m_selectedSample;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            // Initialize main collections
            m_allSamples = new List<SampleViewModel>(
                SampleFactory.Current.GetSampleInfos()
                    .Convert<SampleDescription, SampleViewModel>(
                        (actSampleDesc) => new SampleViewModel(actSampleDesc)));
            m_visibleSamples = new ObservableCollection<SampleViewModel>();
            m_categories = new List<string>(
                (from actSample in m_allSamples
                 select actSample.Category).Distinct());

            // Set initial selection
            m_selectedCategory = m_categories.FirstOrDefault();
            m_selectedSample = m_allSamples.FirstOrDefault();

            // Apply category
            if(!string.IsNullOrEmpty(m_selectedCategory))
            {
                Handle_SelectedCategoryChanged(m_selectedCategory);
            }

            // Initialize commands
            this.CommandShowSource = new DelegateCommand(async () =>
            {
                if(m_selectedSample != null)
                {
                    await Windows.System.Launcher.LaunchUriAsync(
                        new Uri(m_selectedSample.SampleDescription.CodeUrl));
                }
            });
        }

        /// <summary>
        /// Applies the given selected sample category.
        /// </summary>
        /// <param name="newCategory">The new category.</param>
        private void Handle_SelectedCategoryChanged(string newCategory)
        {
            m_visibleSamples.Clear();

            m_allSamples
                .Where((actSampleDesc) => actSampleDesc.Category == newCategory)
                .ForEachInEnumeration((actSampleDesc) => m_visibleSamples.Add(actSampleDesc));
        }

        public ObservableCollection<SampleViewModel> VisibleSamples
        {
            get { return m_visibleSamples; }
        }

        public SampleViewModel SelectedSample
        {
            get { return m_selectedSample; }
            set
            {
                if(m_selectedSample != value)
                {
                    m_selectedSample = value;

                    RaisePropertyChanged();
                    FrozenSkyApplication.Current.UIMessenger.Publish(
                        new MessageSampleChanged(m_selectedSample));
                }
            }
        }

        public List<string> Categories
        {
            get { return m_categories; }
        }

        public string SelectedCategory
        {
            get { return m_selectedCategory; }
            set
            {
                if(m_selectedCategory != value)
                {
                    m_selectedCategory = value;

                    RaisePropertyChanged();
                    Handle_SelectedCategoryChanged(m_selectedCategory);
                }
            }
        }

        public DelegateCommand CommandShowSource
        {
            get;
            private set;
        }
    }
}
