using FrozenSky.Infrastructure;
using FrozenSky.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrozenSky.RKKinectLounge.Base
{
    public class MainFolderViewModel : FolderViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainFolderViewModel"/> class.
        /// </summary>
        public MainFolderViewModel(string mainPath)
            : base(null, mainPath)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFolderViewModel"/> class.
        /// </summary>
        public MainFolderViewModel()
            : this(string.Empty)
        {

        }

        /// <summary>
        /// Creates the new ViewModel with the same target (.. same constructor arguments).
        /// </summary>
        public override NavigateableViewModelBase CreateNewWithSameTarget()
        {
           return new MainFolderViewModel(base.BasePath);
        }
    }
}
