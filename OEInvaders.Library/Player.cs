// <copyright file="Player.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Library
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a player type.
    /// </summary>
    public enum PlayerCharacter
    {
        /// <summary>
        /// A player character.
        /// </summary>
        ZoliDee = 11,

        /// <summary>
        /// A player character.
        /// </summary>
        RicsiBoi = 12,

        /// <summary>
        /// A player character.
        /// </summary>
        RajosBoi = 13,

        /// <summary>
        /// A player character.
        /// </summary>
        TomiGotchaBitch = 16
    }

    /// <summary>
    /// Represents a player type.
    /// </summary>
    public class Player : IEntity
    {
        private readonly string name;

        [NonSerialized]
        [JsonIgnore]
        private readonly BitmapSource image;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="chara">The player type.</param>
        /// <param name="img">The image of the player.</param>
        /// <param name="pos">Init position.</param>
        /// <param name="isholder">Specifies if this image is a player health bar.</param>
        public Player(string name, PlayerCharacter chara, Point pos, bool isholder = false)
        {
            this.PlayerStatus = EntityStatus.Fine;
            this.name = name;
            this.image = Images.ImageList[((int)chara).ToString()];
            this.Position = pos;
            int size = 50;
            if (isholder)
            {
                size = 30;
            }

            this.Bounds = new Rect(new Point(this.Position.X - 50 / 2.0, this.Position.Y - size / 2.0), new Size(size, size));
        }

        /// <summary>
        /// Gets or sets the stored explosion effect.
        /// </summary>
        public BitmapSource Explosion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the player status.
        /// </summary>
        public EntityStatus PlayerStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the image of the player.
        /// </summary>
        [JsonIgnore]
        public BitmapSource Image
        {
            get
            {
                return image;
            }
        }

        /// <summary>
        /// Sets the 2D position of the player.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public void SetPosition(double x, double y)
        {
            this.Position = new Point(x, y);
            this.Bounds = new Rect(new Point(x - this.Bounds.Width / 2.0, y - this.Bounds.Height / 2.0), this.Bounds.Size);
        }

        public int Rotation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the position.
        /// </summary>
        public Point Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the Rectangle position.
        /// </summary>
        public Rect Bounds
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the player.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
