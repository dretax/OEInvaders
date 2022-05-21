// <copyright file="ExplosionEffect.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Library
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    /// <summary>
    /// Static class representing an ExplosionEffect image.
    /// </summary>
    public static class ExplosionEffect
    {
        /// <summary>
        /// Initializes static members of the <see cref="ExplosionEffect"/> class.
        /// </summary>
        static ExplosionEffect()
        {
            Uri myUri = new Uri(Directory.GetCurrentDirectory() + "\\Images\\fire.gif", UriKind.RelativeOrAbsolute);
            GifBitmapDecoder decoder2 = new GifBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            int frameCount = decoder2.Frames.Count;

            GifDecoder = decoder2;
            Frames = frameCount;
        }

        /// <summary>
        /// Gets or Sets the target image's gif bitmap.
        /// </summary>
        public static GifBitmapDecoder GifDecoder
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or Sets the available frames in the target gif image.
        /// </summary>
        public static int Frames
        {
            get;
            private set;
        }
    }
}