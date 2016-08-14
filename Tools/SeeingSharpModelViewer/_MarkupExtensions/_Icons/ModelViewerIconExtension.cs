﻿#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# Tools. More info at 
     - https://github.com/RolandKoenig/SeeingSharp/tree/master/Tools (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
	This program is distributed under the terms of the Microsoft Public License (Ms-PL)-
	More info at https://msdn.microsoft.com/en-us/library/ff647676.aspx
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace SeeingSharpModelViewer
{
    public class ModelViewerIconExtension : MarkupExtension
    {
        private static Dictionary<string, BitmapImage> s_images;

        /// <summary>
        /// Initializes the <see cref="ModelViewerIconExtension"/> class.
        /// </summary>
        static ModelViewerIconExtension()
        {
            s_images = new Dictionary<string, BitmapImage>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelViewerIconExtension"/> class.
        /// </summary>
        public ModelViewerIconExtension()
        {
            this.Icon = ModelViewerIcon.Open;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            string uriPath = $"pack://application:,,,/SeeingSharpModelViewer;component/Assets/Icons/{this.Icon}_16x16.png";

            // Load the ImageSource (me may have cached it before
            BitmapImage result = null;
            if(!s_images.TryGetValue(uriPath, out result))
            {
                result = new BitmapImage(new Uri(uriPath, UriKind.Absolute));
                result.Freeze();
                s_images.Add(uriPath, result);
            }

            // Create the result object
            switch(this.ResultType)
            {
                case IconResultType.Image:
                    Image imgControl = new Image();
                    imgControl.Source = result;
                    return imgControl;

                case IconResultType.BitmapImage:
                    return result;

                default:
                    return null;
            } 
        }

        public ModelViewerIcon Icon
        {
            get;
            set;
        }

        public IconResultType ResultType
        {
            get;
            set;
        }
    }
}