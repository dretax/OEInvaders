// <copyright file="Extensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extensions class.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Next element of enum.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="src">src.</param>
        /// <returns>Type ret.</returns>
        public static T Next<T>(this T src)
            where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));
            }

            T[] arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(arr, src) + 1;
            return (arr.Length == j) ? arr[0] : arr[j];
        }

        /// <summary>
        /// Element before the current element of enum.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="src">src.</param>
        /// <returns>Type ret.</returns>
        public static T Back<T>(this T src)
            where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));
            }

            T[] arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(arr, src) - 1;
            return (j == -1) ? arr[arr.Length - 1] : arr[j];
        }
    }
}
