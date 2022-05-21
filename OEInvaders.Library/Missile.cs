// <copyright file="Missile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Library
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the type of a missile.
    /// </summary>
    public enum MissileType
    {
        /// <summary>
        /// Test missile. Every rocket is used.
        /// </summary>
        Test,

        /// <summary>
        /// Rockets that can only be shot by players.
        /// </summary>
        PlayerType,

        /// <summary>
        /// Rockets that can only be shot by teachers.
        /// </summary>
        TeacherType
    }

    /// <summary>
    /// Represents the direction of the missile.
    /// </summary>
    public enum MissileDirection
    {
        /// <summary>
        /// Launches the missile at the player's direction.
        /// </summary>
        TowardsPlayer,

        /// <summary>
        /// Launches the missile at the teachers direction.
        /// </summary>
        TowardsInvaderTeachers
    }

    /// <summary>
    /// Represents an actual missile.
    /// </summary>
    public class Missile : IEntity
    {
        [NonSerialized]
        [JsonIgnore]
        private readonly BitmapSource image;
        private readonly MissileDirection direction;
        private readonly MissileType type;
        public static readonly Random RandomGenerator = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="Missile"/> class.
        /// </summary>
        /// <param name="type">The type of the missile.</param>
        /// <param name="point">The starting position.</param>
        /// <param name="dir">The direction of the missile.</param>
        public Missile(MissileType type, Point point, MissileDirection dir)
        {
            if (type == MissileType.Test)
            {
                string[] names = Enum.GetNames(typeof(MissileType));
                int random = RandomGenerator.Next(0, names.Length);
                this.image = Images.ImageList[names[random].ToLower()];
            }
            else if (type == MissileType.PlayerType)
            {
                int random = RandomGenerator.Next(1, 4);
                this.image = Images.ImageList["ediak" + random];
            }
            else if (type == MissileType.TeacherType)
            {
                int random = RandomGenerator.Next(1, 4);
                this.image = Images.ImageList["etanar" + random];
            }

            this.type = type;
            this.Position = point;
            this.Bounds = new Rect(new Point(this.Position.X - (50 / 2.0), this.Position.Y - (50 / 2.0)), new Size(50, 50));
            this.direction = dir;
        }

        /// <summary>
        /// Gets the image of the missile.
        /// </summary>
        [JsonIgnore]
        public BitmapSource Image
        {
            get
            {
                return this.image;
            }
        }

        /// <summary>
        /// Sets the position of the missile by 2D values.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public void SetPosition(double x, double y)
        {
            this.Position = new Point(x, y);
            this.Bounds = new Rect(new Point(x - this.Bounds.Width / 2.0, y - this.Bounds.Height / 2.0), this.Bounds.Size);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the position of the missile.
        /// </summary>
        public Point Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the rectangle position of the missile.
        /// </summary>
        public Rect Bounds
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether if the missile exploded.
        /// </summary>
        public bool Exploded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the direction of the missile.
        /// </summary>
        public MissileDirection MissileDirection
        {
            get { return this.direction; }
        }

        /// <summary>
        /// Gets the type of the missile.
        /// </summary>
        public MissileType MissileType
        {
            get { return this.type; }
        }
    }
}
