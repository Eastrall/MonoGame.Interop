using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

/*--------------------------------------------------------
 * WPFMouse.cs
 * 
 * Version: 1.0
 * Author: Onyx
 * Created: 08/11/2015 15:06:35
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace MonoGame.Interop.Input
{
    public static class WPFMouse
    {
        #region FIELDS

        internal static GameModule PrimaryGameModule = null;
        internal static UIElement UIElement = null;
        internal static System.Windows.Point Position;
        internal static Int32 MouseScrollWheelValue = 0;

        #endregion

        #region PROPERTIES



        #endregion

        #region CONSTRUCTORS



        #endregion

        #region METHODS

        public static void SetUIElement(UIElement element)
        {
            UIElement = element;
        }

        public static MouseState GetState()
        {
            return PrimaryGameModule != null ? PrimaryGameModule.MouseState : default(MouseState);
        }

        #endregion
    }
}
