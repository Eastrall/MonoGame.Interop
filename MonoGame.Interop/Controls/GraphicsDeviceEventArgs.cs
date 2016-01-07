using Microsoft.Xna.Framework.Graphics;
using System;

/*--------------------------------------------------------
 * GraphicsDeviceEventArgs.cs
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 11/11/2015 11:21:15
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace MonoGame.Interop.Controls
{
    /// <summary>
    /// Arguments used for Device related events.
    /// </summary>
    public class GraphicsDeviceEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the GraphicsDevice.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
		/// Initializes a new GraphicsDeviceEventArgs.
        /// </summary>
		/// <param name="graphicsDevice">The GraphicsDevice associated with the event.</param>
		public GraphicsDeviceEventArgs(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }
    }
}
