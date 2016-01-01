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
using SeeingSharp.Infrastructure;
using SeeingSharp.Multimedia.Core;
using SeeingSharp.Samples.Base;
using SeeingSharp.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalWindowsSampleContainer
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region all collections
        private List<SampleViewModel> m_allSamples;
        private List<CategoryInfo> m_categories;
        private ObservableCollection<SampleViewModel> m_visibleSamples;
        #endregion

        #region current selection
        private CategoryInfo m_selectedCategory;
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
            m_categories = new List<CategoryInfo>(
                (from actSample in m_allSamples
                 select new CategoryInfo(actSample.Category, CategoryMetadata.GetCategoryIconSymbol(actSample.Category)))
                 .Distinct(CategoryInfo.EqualityComparer));

            // Initialize commands
            this.CommandShowSource = new DelegateCommand(async () =>
            {
                if (m_selectedSample != null)
                {
                    await Windows.System.Launcher.LaunchUriAsync(
                        new Uri(m_selectedSample.SampleDescription.CodeUrl));
                }
            });

            // Set initial selection
            CommonTools.InvokeDelayed(
                () =>
                {
                    this.SelectedCategory = this.Categories.FirstOrDefault();
                    this.SelectedSample = this.VisibleSamples.FirstOrDefault();
                },
                TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Applies the given selected sample category.
        /// </summary>
        /// <param name="newCategory">The new category.</param>
        private void Handle_SelectedCategoryChanged(CategoryInfo newCategory)
        {
            m_visibleSamples.Clear();

            m_allSamples
                .Where((actSampleDesc) => actSampleDesc.Category == newCategory.Name)
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
                    SeeingSharpApplication.Current.UIMessenger.Publish(
                        new MessageSampleChanged(m_selectedSample));
                }
            }
        }

        public List<CategoryInfo> Categories
        {
            get { return m_categories; }
        }

        public CategoryInfo SelectedCategory
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
