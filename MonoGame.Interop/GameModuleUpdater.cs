using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*--------------------------------------------------------
 * GameUpdater.cs
 * 
 * Version: 1.0
 * Author: Onyx
 * Created: 10/11/2015 12:39:01
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace MonoGame.Interop
{
    internal class GameModuleUpdater
    {
        #region FIELDS



        #endregion

        #region PROPERTIES



        #endregion

        #region CONSTRUCTORS

        public GameModuleUpdater(Action<GameTime> updateAction, Action<GameTime> drawAction)
        {

        }

        #endregion

        #region METHODS

        public void Run()
        {
            // Run
        }

        private void GameTick()
        {
        }

        #endregion
    }
}
