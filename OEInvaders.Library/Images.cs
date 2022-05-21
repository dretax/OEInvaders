// <copyright file="Images.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Library
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// An image class that stores most of the images in the memory.
    /// </summary>
    public static class Images
    {
        private static readonly Dictionary<string, BitmapImage> ImageStorage = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// Gets the loaded images.
        /// </summary>
        public static Dictionary<string, BitmapImage> ImageList
        {
            get
            {
                return ImageStorage;
            }
        }

        /// <summary>
        /// Gets if the images are loaded.
        /// </summary>
        public static bool Loaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Loads the available pngs into the memory.
        /// </summary>
        public static void LoadImages()
        {
            string[] images = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Images\\", "*.png");
            foreach (var x in images)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(x);
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(fi.FullName, UriKind.Absolute);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();

                ImageStorage.Add(Path.GetFileNameWithoutExtension(x), src);
            }

            Loaded = true;
        }
    }
}
