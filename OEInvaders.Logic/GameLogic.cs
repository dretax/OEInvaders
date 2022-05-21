// <copyright file="GameLogic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming


namespace OEInvaders.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using OEInvaders.Library;
    
    
    /// <summary>
    /// Handles the main game logic, and rendering.
    /// </summary>
    public class GameLogic : IGameLogic
    {
        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosTitleScore;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosTitleHiScore;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosTitleLives;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosTitleLevel;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosTextLevelStart;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosTextGameOver;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosTextLevelCompleted;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosTextRestart;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosTextStartNewLevel;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosValueScore;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosValueHiScore;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosValueLives;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosValueLevel;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosGameWon;

        /// <summary>
        /// Represents a position.
        /// </summary>
        public static readonly Point PosBooster;

        /// <summary>
        /// Random generator.
        /// </summary>
        public static readonly Random RandomGenerator = new Random();

        /// <summary>
        /// Formatted text storage.
        /// </summary>
        public static TextStorage FormattedStorage;

        /// <summary>
        /// Screen's rectangle.
        /// </summary>
        public static Rect Screen;

        /// <summary>
        /// Stores the instance of the current dispatcher.
        /// </summary>
        public static Dispatcher GameDispatcher;

        /// <summary>
        /// Stores the current DPI amount.
        /// </summary>
        public static double PixelsPerDip;

        /// <summary>
        /// Initializes static members of the <see cref="GameLogic"/> class.
        /// </summary>
        static GameLogic()
        {
            FormattedStorage = new TextStorage();
            Screen = new Rect(0, 0, 640, 540);
            PixelSize = new Size(3, 3);
            PosTextLevelStart = new Point((Screen.Width - FormattedStorage.TextLevelStart.Width) / 2, (Screen.Height - FormattedStorage.TextLevelStart.Height + 80) / 2);
            PosTextGameOver = new Point((Screen.Width - FormattedStorage.TextGameOver.Width) / 2, (Screen.Height - FormattedStorage.TextGameOver.Height + 80) / 2);
            PosTextLevelCompleted = new Point((Screen.Width - FormattedStorage.TextLevelCompleted.Width) / 2, (Screen.Height - FormattedStorage.TextLevelCompleted.Height + 80) / 2);
            PosTextRestart = new Point((Screen.Width - FormattedStorage.TextRestart.Width) / 2, (Screen.Height - FormattedStorage.TextRestart.Height + 80) / 2);
            PosTextStartNewLevel = new Point((Screen.Width - FormattedStorage.TextStartNewLevel.Width) / 2, (Screen.Height - FormattedStorage.TextStartNewLevel.Height + 80) / 2);
            PosGameWon = new Point((Screen.Width - FormattedStorage.TextStartNewLevel.Width) / 2, (Screen.Height - FormattedStorage.TextStartNewLevel.Height) / 2);

            PosTitleScore = new Point(10, 5);
            PosTitleHiScore = new Point(468, 5);
            PosTitleLives = new Point(10, Screen.Height - 30 + 4);
            PosTitleLevel = new Point(540, Screen.Height - 30 + 4);

            PosValueScore = new Point(90, 5);
            PosValueHiScore = new Point(575, 5);
            PosValueLives = new Point(75, Screen.Height - 30 + 4);
            PosValueLevel = new Point(611, Screen.Height - 30 + 4);
            PosBooster = new Point((Screen.Width / 2) - 55, Screen.Height - 30 + 4);
            
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\Saves.ini"))
            {
                File.Create(Directory.GetCurrentDirectory() + "\\Saves.ini").Dispose();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogic"/> class.
        /// </summary>
        /// <param name="playername">Name.</param>
        /// <param name="chara">Character.</param>
        /// <param name="pixels">Pixels.</param>
        /// <param name="disp">Display.</param>
        public GameLogic(string playername, PlayerCharacter chara, double pixels, Dispatcher disp)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\Saves.ini"))
            {
                IniParser ini = new IniParser(Directory.GetCurrentDirectory() + "\\Saves.ini");
                List<int> values = new List<int>();
                if (ini.EnumSection("Saves").Length > 0)
                {
                    foreach (var x in ini.EnumSection("Saves"))
                    {
                        if (ini.GetSetting("Saves", x) != null)
                        {
                            int data;
                            if (int.TryParse(ini.GetSetting("Saves", x), out data))
                            {
                                values.Add(data);
                            }
                        }
                    }
                }

                values.Sort();
                if (values.Count > 0)
                {
                    HiScore = values.Last();
                }
            }

            this.Level = new Level(Screen.Size, PixelSize, playername, chara);
            this.Level.Start();
            PixelsPerDip = pixels;
            GameDispatcher = disp;
        }

        /// <summary>
        /// Gets the pixel size.
        /// </summary>
        public static Size PixelSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current level handler.
        /// </summary>
        public Level Level
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the hiscore.
        /// </summary>
        public int HiScore
        {
            get;
            private set;
        }

        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="context">Context.</param>
        public void Draw(DrawingContext context)
        {
            context.DrawRectangle(FormattedStorage.BackgroundBrush, null, Screen);

            this.Level.Draw(context);

            if (this.Level.Score > this.HiScore)
            {
                this.HiScore = this.Level.Score;
            }

            context.DrawText(FormattedStorage.TitleScore, PosTitleScore);
            context.DrawText(new FormattedText(this.Level.Score.ToString("00000"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, FormattedStorage.TextTypeFace, TextStorage.TextFontSize, FormattedStorage.BrushTextValue, PixelsPerDip), PosValueScore);

            context.DrawText(FormattedStorage.TitleHiScore, PosTitleHiScore);
            context.DrawText(new FormattedText(this.HiScore.ToString("00000"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, FormattedStorage.TextTypeFace, TextStorage.TextFontSize, FormattedStorage.BrushTextValue, PixelsPerDip), PosValueHiScore);

            context.DrawText(FormattedStorage.TitleLives, PosTitleLives);
            context.DrawText(new FormattedText(this.Level.Lives.ToString("0"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, FormattedStorage.TextTypeFace, TextStorage.TextFontSize, FormattedStorage.BrushTextValue, PixelsPerDip), PosValueLives);

            context.DrawText(FormattedStorage.TitleLevel, PosTitleLevel);
            context.DrawText(new FormattedText(this.Level.CurrentLevel.ToString("00"), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, FormattedStorage.TextTypeFace, TextStorage.TextFontSize, FormattedStorage.BrushTextValue, PixelsPerDip), PosValueLevel);

            context.DrawLine(FormattedStorage.BorderPen, new Point(0, Screen.Height - 30), new Point(Screen.Width, Screen.Height - 30));
            context.DrawRectangle(null, FormattedStorage.BorderPen, Screen);
        }
    }
}