// <copyright file="OEInvadersMenu.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using OEInvaders.Library;

namespace OEInvaders
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class OEInvadersMenu : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OEInvadersMenu"/> class.
        /// </summary>
        public OEInvadersMenu()
        {
            CTimer.StartWatching();
            PreLoader.LoadRequirements();
            this.InitializeComponent();
        }
    }
}
