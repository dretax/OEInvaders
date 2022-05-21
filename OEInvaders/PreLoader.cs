// <copyright file="PreLoader.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders
{
    using Library;

    /// <summary>
    /// Preloader class used to load the images before the initialization.
    /// </summary>
    internal static class PreLoader
    {
        /// <summary>
        /// Called to load the library, and load the images into the memory.
        /// </summary>
        internal static void LoadRequirements()
        {
            Images.LoadImages();
        }
    }
}
