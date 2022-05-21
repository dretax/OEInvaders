// <copyright file="Game.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using OEInvaders.Library;
    using OEInvaders.Logic;

    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        private GameLogic Logic;
        private DispatcherTimer timer;
        private double PixelsPerDip;

        /// <summary>
        /// Tells if we are in menu.
        /// </summary>
        public static bool IsInMenu = true;

        /// <summary>
        /// Our current name.
        /// </summary>
        public static string PlayerName = "HALLGATO";

        /// <summary>
        /// The character that is selected.
        /// </summary>
        public static PlayerCharacter SelectedCharacter = PlayerCharacter.ZoliDee;

        /// <summary>
        /// The character selector's rectangle base position.
        /// </summary>
        public Rectangle CharacterSelectorRect;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            this.PixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            TextStorage.PixelsPerDip = this.PixelsPerDip;
            EndGameHandler.OnGameOver += this.ResetWholeGameToTheMenu;
            this.timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };
            this.timer.Tick += this.OnTimerGameTick;
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
            this.KeyDown += this.OnKeyDown;
            this.KeyUp += this.OnKeyUp;
            this.CharacterSelectorRect = new Rectangle(0, 0, 150, 150);
        }

        /// <summary>
        /// Called by the xaml's rendering system.
        /// </summary>
        /// <param name="context">context.</param>
        protected override void OnRender(DrawingContext context)
        {
            base.OnRender(context);

            if (this.Logic != null && !IsInMenu)
            {
                this.Logic.Draw(context);
            }
            else if (IsInMenu)
            {
                context.DrawRectangle(GameLogic.FormattedStorage.BackgroundBrush, null, GameLogic.Screen);

                context.DrawRectangle(null, GameLogic.FormattedStorage.BorderPen, new Rect(250, 200, 150, 150));

                string charimage = ((int)SelectedCharacter).ToString();
                if (Images.ImageList.ContainsKey(charimage))
                {
                    context.DrawImage(Images.ImageList[charimage], new Rect(255, 207, 145, 145));
                }

                context.DrawText(new FormattedText("Kezdj el írni ahhoz, hogy megváltoztasd a neved!", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), 20, System.Windows.Media.Brushes.White, this.PixelsPerDip), new System.Windows.Point(115, 100));
                context.DrawText(new FormattedText(PlayerName, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), 20, System.Windows.Media.Brushes.Orange, this.PixelsPerDip), new System.Windows.Point(275, 145));
                context.DrawText(new FormattedText("Nyilakkal mozogsz, és választasz karaktert!", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), 20, System.Windows.Media.Brushes.White, this.PixelsPerDip), new System.Windows.Point(140, 400));
                context.DrawText(new FormattedText("Space gomb segítségével lősz vissza.", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), 20, System.Windows.Media.Brushes.White, this.PixelsPerDip), new System.Windows.Point(160, 430));

                string charname = SelectedCharacter.ToString();
                if (SelectedCharacter == PlayerCharacter.TomiGotchaBitch)
                {
                    context.DrawText(new FormattedText(charname, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), 20, System.Windows.Media.Brushes.Orange, this.PixelsPerDip), new System.Windows.Point(250, 360));
                }
                else
                {
                    context.DrawText(new FormattedText(charname, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Arial"), 20, System.Windows.Media.Brushes.Orange, this.PixelsPerDip), new System.Windows.Point(295, 360));
                }

                context.DrawRectangle(null, GameLogic.FormattedStorage.BorderPen, GameLogic.Screen);
            }

            CTimer.OnUpdate();
        }

        /// <summary>
        /// Handles the game ending event.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="points">Points.</param>
        private void ResetWholeGameToTheMenu(string name, int points)
        {
            IsInMenu = true;
            this.Logic = null;
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\Saves.ini"))
            {
                File.Create(Directory.GetCurrentDirectory() + "\\Saves.ini").Dispose();
            }

            IniParser ini = new IniParser(Directory.GetCurrentDirectory() + "\\Saves.ini");
            if (ini.GetSetting("Saves", name) == null)
            {
                ini.AddSetting("Saves", name, points.ToString());
            }
            else
            {
                ini.SetSetting("Saves", name, points.ToString());
            }

            ini.Save();
        }

        /// <summary>
        /// Runs every drame.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">event.</param>
        private void RenderHandler(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        /// <summary>
        /// Runs when the xaml loaded.
        /// </summary>
        /// <param name="se">sender.</param>
        /// <param name="re">event.</param>
        private void OnLoaded(object se, RoutedEventArgs re)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.KeyDown += this.OnKeyDown;
                window.KeyUp += this.OnKeyUp;
            }

            this.timer.Start();
        }

        /// <summary>
        /// Runs when the xaml unloaded.
        /// </summary>
        /// <param name="se">sender.</param>
        /// <param name="re">event.</param>
        private void OnUnloaded(object se, RoutedEventArgs re)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.KeyDown -= this.OnKeyDown;
                window.KeyUp -= this.OnKeyUp;
            }
        }

        /// <summary>
        /// Runs on a specific timer.
        /// Calls the render.
        /// </summary>
        /// <param name="se">sender.</param>
        /// <param name="re">event.</param>
        private void OnTimerGameTick(object se, EventArgs re)
        {
            this.InvalidateVisual();
        }

        /// <summary>
        /// Runs when a key was pressed.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="ke">key.</param>
        private void OnKeyDown(object sender, KeyEventArgs ke)
        {
            if (!Images.Loaded)
            {
                return;
            }

            if (this.Logic != null)
            {
                this.Logic.Level.PressKey(ke.Key);
            }
        }

        /// <summary>
        /// Runs when a key was released.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="ke">key.</param>
        private void OnKeyUp(object sender, KeyEventArgs ke)
        {
            if (!Images.Loaded)
            {
                return;
            }

            if (IsInMenu)
            {
                if (ke.Key == Key.Enter)
                {
                    var GameDispatcher = this.Dispatcher;
                    this.Logic = new GameLogic(PlayerName, SelectedCharacter, this.PixelsPerDip, GameDispatcher);
                    IsInMenu = false;
                    return;
                }

                if (ke.Key == Key.Back)
                {
                    if (string.IsNullOrEmpty(PlayerName))
                    {
                        return;
                    }

                    PlayerName = PlayerName.Remove(PlayerName.Length - 1);
                    return;
                }

                if (ke.Key == Key.Left)
                {
                    SelectedCharacter = SelectedCharacter.Back();
                }
                else if (ke.Key == Key.Right)
                {
                    SelectedCharacter = SelectedCharacter.Next();
                }
                else
                {
                    if (this.CheckIfNormalCharacter(ke.Key) && PlayerName.Length < 10)
                    {
                        PlayerName += ke.Key.ToString().ToUpper();
                    }
                }

                return;
            }

            if (this.Logic != null)
            {
                this.Logic.Level.ReleaseKey(ke.Key);
            }
        }

        /// <summary>
        /// Checks if the key is eligible.
        /// </summary>
        /// <param name="ke">Key.</param>
        /// <returns>Bool.</returns>
        private bool CheckIfNormalCharacter(Key ke)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            return characters.Contains(ke.ToString().ToUpper());
        }
    }
}
