namespace OEInvaders.Library
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an "invader" teacher.
    /// </summary>
    public class Szabo : IEntity
    {
        private bool isalive = true;
        [NonSerialized, JsonIgnore]
        private readonly BitmapSource image;
        private readonly BitmapSource zsanimage;
        private int health;
        private bool zsanie = false;
        private CTimer effecTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Szabo"/> class.
        /// </summary>
        /// <param name="img">The image of the teacher.</param>
        /// <param name="pos">The init position.</param>
        public Szabo(BitmapSource img, Point pos)
        {
            this.zsanimage = Images.ImageList["zsanie"];
            this.health = 20;
            this.InvaderStatus = EntityStatus.Fine;
            this.image = img;
            this.Position = pos;
            this.Bounds = new Rect(new Point(this.Position.X - (50 / 2.0), this.Position.Y - (50 / 2.0)), new Size(70, 70));
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
        /// Gets image of the teacher.
        /// </summary>
        [JsonIgnore]
        public BitmapSource ZsanImage
        {
            get
            {
                return this.zsanimage;
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
        /// Gets Mr.Szabo's Health.
        /// </summary>
        public int Health
        {
            get { return this.health; }
        }

        /// <summary>
        /// Gets if we should play Zsanie effect.
        /// </summary>
        public bool Zsanie
        {
            get { return this.zsanie; }
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
            this.Bounds = new Rect(new Point(x - (this.Bounds.Width / 2.0), y - (this.Bounds.Height / 2.0)), this.Bounds.Size);
        }

        /// <summary>
        /// Damages Mr.Szabo with 1 health point.
        /// </summary>
        public void Damage()
        {
            if (this.effecTimer == null)
            {
                this.zsanie = true;
                this.effecTimer = CTimer.SetTimer(
                    () =>
                {
                    this.zsanie = false;
                    this.effecTimer = null;
                }, 500);
            }

            this.health--;
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