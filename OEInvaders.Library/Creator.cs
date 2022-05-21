// <copyright file="Creator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Library
{
    using System.Windows;

    /// <summary>
    /// This class is used to Create specific type of entities.
    /// </summary>
    public static class Creator
    {
        /// <summary>
        /// Creates a Teacher type of "invader".
        /// </summary>
        /// <param name="pos">Position.</param>
        /// <param name="type">Type of teacher.</param>
        /// <returns>Teacher</returns>
        public static Teacher CreateInvader(Point pos, Teachers type)
        {
            int val = (int)type;
            Teacher t = new Teacher(Images.ImageList[val.ToString()], pos);
            return t;
        }

        /// <summary>
        /// Creates a Player type.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="chara">Character.</param>
        /// <returns>Player.</returns>
        public static Player CreatePlayer(string name, PlayerCharacter chara)
        {
            Player p = new Player(name, chara, new Point(320, 485));
            return p;
        }

        /// <summary>
        /// Creates a Missile.
        /// </summary>
        /// <param name="dir">The direction of rocket.</param>
        /// <param name="type">The type of missile.</param>
        /// <param name="p">The starting position.</param>
        /// <returns>Missile.</returns>
        public static Missile CreateMissile(MissileDirection dir, MissileType type, Point p)
        {
            Missile m = new Missile(type, p, dir);
            return m;
        }

        /// <summary>
        /// Creates a shield.
        /// </summary>
        /// <param name="p">The position.</param>
        /// <returns>Shield.</returns>
        public static Shield CreateShield(Point p)
        {
            Shield s = new Shield(p);
            return s;
        }
    }
}
