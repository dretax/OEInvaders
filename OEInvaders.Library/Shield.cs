namespace OEInvaders.Library
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a shield pixel.
    /// </summary>
    public class Shield : IEntity
    {
        [NonSerialized]
        [JsonIgnore]
        private readonly BitmapSource image;
        private bool isAlive = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shield"/> class.
        /// </summary>
        /// <param name="pos">Position.</param>
        public Shield(Point pos)
        {
            this.image = Images.ImageList["shield"];
            this.Position = pos;
            this.Bounds = new Rect(new Point(this.Position.X - (3 / 2.0), this.Position.Y - (3 / 2.0)), new Size(4, 4));
        }

        /// <summary>
        /// Gets or sets if the shield is alive.
        /// </summary>
        public bool IsAlive
        {
            get { return this.isAlive; }
            set { this.isAlive = value; }
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
        /// Sets the position based on 2D location.
        /// </summary>
        /// <param name="x">Pos X coordinate.</param>
        /// <param name="y">Pos Y coordinate.</param>
        public void SetPosition(double x, double y)
        {
            this.Position = new Point(x, y);
            this.Bounds = new Rect(new Point(x - (this.Bounds.Width / 2.0), y - (this.Bounds.Height / 2.0)), this.Bounds.Size);
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
    }
}