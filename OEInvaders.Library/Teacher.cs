// <copyright file="Teacher.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Library
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the available teachers.
    /// </summary>
    public enum Teachers
    {
        /// <summary>
        /// The semi boss.
        /// </summary>
        SzaboZS = 5,

        /// <summary>
        /// A teacher.
        /// </summary>
        SiposM = 4,

        /// <summary>
        /// A teacher.
        /// </summary>
        KissD = 1,

        /// <summary>
        /// A teacher.
        /// </summary>
        SzenaS = 10,

        /// <summary>
        /// The one and only BOSS.
        /// </summary>
        SergyanSz = 9,

        /// <summary>
        /// A teacher.
        /// </summary>
        KovacsA = 7,

        /// <summary>
        /// A teacher.
        /// </summary>
        SimonG = 8,

        /// <summary>
        /// A teacher.
        /// </summary>
        RaczE = 6,

        /// <summary>
        /// A teacher.
        /// </summary>
        GyenesS = 3,

        /// <summary>
        /// A teacher.
        /// </summary>
        VernyihelZ = 2
    }

    /// <summary>
    /// Represents an "invader" teacher.
    /// </summary>
    public class Teacher : IEntity
    {
        private bool isalive = true;
        [NonSerialized]
        [JsonIgnore]
        private readonly BitmapSource image;

        /// <summary>
        /// Initializes a new instance of the <see cref="Teacher"/> class.
        /// </summary>
        /// <param name="img">The image of the teacher.</param>
        /// <param name="pos">The init position.</param>
        public Teacher(BitmapSource img, Point pos)
        {
            this.InvaderStatus = EntityStatus.Fine;
            this.image = img;
            this.Position = pos;
            this.Bounds = new Rect(new Point(this.Position.X - 50 / 2.0, this.Position.Y - 50 / 2.0), new Size(50, 50));
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
        /// Gets or sets the stored explosion effect.
        /// </summary>
        public BitmapSource Explosion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the invader status.
        /// </summary>
        public EntityStatus InvaderStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the Alive variable.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return this.isalive;
            }

            set
            {
                this.isalive = value;
            }
        }

        /// <summary>
        /// Sets the position based on 2D location.
        /// </summary>
        /// <param name="x">Pos X coordinate.</param>
        /// <param name="y">Pos Y coordinate.</param>
        public void SetPosition(double x, double y)
        {
            this.Position = new Point(x, y);
            this.Bounds = new Rect(new Point(x - this.Bounds.Width / 2.0, y - this.Bounds.Height / 2.0), this.Bounds.Size);
        }

        /// <summary>
        /// Gets or Sets the 2D position.
        /// </summary>
        public Point Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the rectangle position.
        /// </summary>
        public Rect Bounds
        {
            get;
            set;
        }
    }
}
