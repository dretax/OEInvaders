// <copyright file="TextStorage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1401
namespace OEInvaders.Library
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// A class used to storage formatted texts.
    /// </summary>
    public class TextStorage
    {
        /// <summary>
        /// Pixels per dpi.
        /// </summary>
        public static double PixelsPerDip = 0;

        /// <summary>
        /// The font size of the text.
        /// </summary>
        public const int TextFontSize = 20;

        /// <summary>
        /// The default font.
        /// </summary>
        public const string TextFontName = "Arial";

        /// <summary>
        /// Creates a green color.
        /// </summary>
        public readonly Brush BrushGreen = Brushes.LimeGreen;

        /// <summary>
        /// Creates a white color.
        /// </summary>
        public readonly Brush BrushTextValue = Brushes.White;

        /// <summary>
        /// Creates a red color.
        /// </summary>
        public readonly Brush BrushTextGameOver = Brushes.Red;

        /// <summary>
        /// Creates a black color.
        /// </summary>
        public readonly Brush BackgroundBrush = Brushes.Black;

        /// <summary>
        /// Creates an orange color.
        /// </summary>
        public readonly Brush BoosterBrush = Brushes.Orange;

        /// <summary>
        /// Sets the title's color.
        /// </summary>
        public readonly Brush BrushTextTitle;

        /// <summary>
        /// Sets the shield's color.
        /// </summary>
        public readonly Brush BrushShield;

        /// <summary>
        /// Creates a line with a specific color based on the given thickness.
        /// </summary>
        public readonly Pen BorderPen;

        /// <summary>
        /// Creates a line with a specific color based on the given thickness.
        /// </summary>
        public readonly Pen BoundPen;

        /// <summary>
        /// Creates the specified font.
        /// </summary>
        public readonly Typeface TextTypeFace;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TitleScore;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TitleHiScore;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TitleLives;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TitleLevel;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextLevelStart;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextGameOver;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextLevelCompleted;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextRestart;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextStartNewLevel;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextInvincible;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextRespawnInvincible;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextDoubleRockets;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextSuperRockets;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextGameWon;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextInvincibleRed;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextRespawnInvincibleRed;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextDoubleRocketsRed;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText TextSuperRocketsRed;

        /// <summary>
        /// Creates a formatted text for a specific information to display.
        /// </summary>
        public readonly FormattedText Letiltva;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextStorage"/> class.
        /// </summary>
        public TextStorage()
        {
            this.BrushTextTitle = this.BrushGreen;
            this.BrushShield = this.BrushGreen;
            this.BorderPen = new Pen(this.BrushGreen, 2.0);
            this.BoundPen = new Pen(Brushes.Yellow, 1.0);
            this.TextTypeFace = new Typeface(TextFontName);
            this.TitleScore = new FormattedText("SCORE:", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextTitle, PixelsPerDip);
            this.TitleHiScore = new FormattedText("HI-SCORE:", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextTitle, PixelsPerDip);
            this.TitleLives = new FormattedText("LIVES:", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextTitle, PixelsPerDip);
            this.TitleLevel = new FormattedText("LEVEL:", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextTitle, PixelsPerDip);
            this.TextLevelStart = new FormattedText("LEVEL START", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextValue, PixelsPerDip);
            this.TextGameOver = new FormattedText("GAME OVER", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextGameOver, PixelsPerDip);
            this.TextGameWon = new FormattedText("YOU HAVE MASTERED BSC", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, Brushes.Green, PixelsPerDip);
            this.TextLevelCompleted = new FormattedText("LEVEL COMPLETED", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextTitle, PixelsPerDip);
            this.TextRestart = new FormattedText("ENTER TO START NEW GAME", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextValue, PixelsPerDip);
            this.TextStartNewLevel = new FormattedText("ENTER TO START NEXT LEVEL", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextValue, PixelsPerDip);
            this.Letiltva = new FormattedText("LETILTVA", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextGameOver, PixelsPerDip);

            this.TextInvincible = new FormattedText("INVINCIBLE", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BoosterBrush, PixelsPerDip);
            this.TextRespawnInvincible = new FormattedText("R-INVINCIBLE", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BoosterBrush, PixelsPerDip);
            this.TextDoubleRockets = new FormattedText("DOUBLE ROCKETS", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BoosterBrush, PixelsPerDip);
            this.TextSuperRockets = new FormattedText("MEGA ROCKETS", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BoosterBrush, PixelsPerDip);

            this.TextInvincibleRed = new FormattedText("INVINCIBLE", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextGameOver, PixelsPerDip);
            this.TextRespawnInvincibleRed = new FormattedText("R-INVINCIBLE", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextGameOver, PixelsPerDip);
            this.TextDoubleRocketsRed = new FormattedText("DOUBLE ROCKETS", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextGameOver, PixelsPerDip);
            this.TextSuperRocketsRed = new FormattedText("MEGA ROCKETS", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, this.TextTypeFace, TextFontSize, this.BrushTextGameOver, PixelsPerDip);

            this.BrushGreen.Freeze();
            this.BrushTextTitle.Freeze();
            this.BrushTextValue.Freeze();
            this.BrushTextGameOver.Freeze();
            this.BackgroundBrush.Freeze();
            this.BorderPen.Freeze();
            this.BoundPen.Freeze();
            this.BrushShield.Freeze();
        }
    }
}
