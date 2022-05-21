// <copyright file="Booster.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Library
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a booster.
    /// </summary>
    public class Booster : IEntity
    {
        [NonSerialized]
        [JsonIgnore]
        private readonly BitmapSource image;

        /// <summary>
        /// Initializes a new instance of the <see cref="Booster"/> class.
        /// </summary>
        /// <param name="teacher">The teacher.</param>
        /// <param name="img">The image of the teacher.</param>
        /// <param name="pos">The init position.</param>
        public Booster(Teachers teacher, BitmapSource img, Point pos)
        {
            this.BoosterTeacher = teacher;
            this.image = img;
            this.Position = pos;
            this.Bounds = new Rect(new Point(this.Position.X - (50 / 2.0), this.Position.Y - (50 / 2.0)), new Size(50, 50));
        }

        /// <summary>
        /// Gets or sets the booster teacher type.
        /// </summary>
        public Teachers BoosterTeacher
        {
            get;
            set;
        }

        /// <summary>
        /// Gets image of the teacher.
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
        /// Gets or sets the 2D position.
        /// </summary>
        public Point Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rectangle position.
        /// </summary>
        public Rect Bounds
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the position based on 2D location.
        /// </summary>
        /// <param name="x">Pos X coordinate.</param>
        /// <param name="y">Pos Y coordinate.</param>
        public void SetPosition(double x, double y)
        {
            this.Position = new Point(x, y);
            this.Bounds = new Rect(new Point(x - (this.Bounds.Width / 2.0), y - (this.Bounds.Height / 2.0)), this.Bounds.Size);
        }
    }
}
