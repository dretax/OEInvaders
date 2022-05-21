// <copyright file="IGameLogic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Logic
{
    using System.Windows.Media;
    
    /// <summary>
    /// Interface of the GameLogic.
    /// </summary>
    public interface IGameLogic
    {
        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="context">Context.</param>
        void Draw(DrawingContext context);
    }
}
