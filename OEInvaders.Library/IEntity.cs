// <copyright file="IEntity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// The status of the entity.
    /// </summary>
    public enum EntityStatus
    {
        /// <summary>
        /// Fine means that the entity is still alive.
        /// </summary>
        Fine,

        /// <summary>
        /// Exploding means that the entity is dead, and is exploding.
        /// </summary>
        Exploding,

        /// <summary>
        /// The entity is now fully dead, and this value helps the MoveInvaders method.
        /// </summary>
        FinishedExploding
    }

    /// <summary>
    /// This interface is used to extend similar object types.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets or Sets the entity image.
        /// </summary>
        BitmapSource Image
        {
            get;
        }

        /// <summary>
        /// Gets or sets the 2D position.
        /// </summary>
        Point Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rectangle position.
        /// </summary>
        Rect Bounds
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the position by 2D values.
        /// </summary>
        /// <param name="x">X Coords.</param>
        /// <param name="y">Y Coords.</param>
        void SetPosition(double x, double y);
    }
}
