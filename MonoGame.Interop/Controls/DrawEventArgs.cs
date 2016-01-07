using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*--------------------------------------------------------
 * DrawEventArgs.cs
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 11/11/2015 11:23:21
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace MonoGame.Interop.Controls
{
    /// <summary>
    /// Provides data for the Draw event.
    /// </summary>
    public sealed class DrawEventArgs : GraphicsDeviceEventArgs
    {
        private readonly DrawingSurface _drawingSurface;

        public DrawEventArgs(DrawingSurface drawingSurface, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            _drawingSurface = drawingSurface;
        }

        public void InvalidateSurface()
        {
            _drawingSurface.Invalidate();
        }
    }
}
