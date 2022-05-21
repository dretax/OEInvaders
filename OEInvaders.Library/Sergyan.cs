namespace OEInvaders.Library
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Newtonsoft.Json;

    /// <summary>
    /// The master of all who we love!
    /// </summary>
    public class Sergyan : IEntity
    {
        [NonSerialized, JsonIgnore]
        private readonly BitmapSource image;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sergyan"/> class.
        /// </summary>
        /// <param name="img">The image of the teacher.</param>
        /// <param name="pos">The init position.</param>
        public Sergyan(BitmapSource img, Point pos)
        {
            this.image = img;
            this.Position = pos;
            this.Bounds = new Rect(new Point(this.Position.X - (120 / 2.0), this.Position.Y - (120 / 2.0)), new Size(150, 150));
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