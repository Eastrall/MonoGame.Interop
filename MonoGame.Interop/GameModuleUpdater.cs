using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
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

        private Action<GameTime> updateAction;

        // Game timer
        private Stopwatch gameTimer;
        private GameTime gameTime;
        private TimeSpan accumulatedElapsedTime;
        private long previousTicks = 0;
        private int updateFrameLag;

        private TimeSpan targetElapsedTime = TimeSpan.FromSeconds(1f / 60f); // 60fps
        private TimeSpan maxElapsedTime = TimeSpan.FromMilliseconds(500f);

        private GameModule gameModule;

        #endregion

        #region PROPERTIES

        public Boolean IsRunning { get; private set; }

        public GameTime GameTime { get { return this.gameTime; } }

        public Boolean IsDrawing { get; set; }

        #endregion

        #region CONSTRUCTORS

        public GameModuleUpdater(GameModule gameModule, Action<GameTime> updateAction)
        {
            this.gameModule = gameModule;
            this.updateAction = updateAction;
            this.gameTimer = new Stopwatch();
            this.gameTime = new GameTime();
        }

        #endregion

        #region METHODS

        public void Start()
        {
            this.IsRunning = true;
            this.gameTimer.Reset();
            this.gameTimer.Start();
            Task.Factory.StartNew(() =>
            {
                while (this.IsRunning)
                    this.GameTick();
            });
        }

        public void Stop()
        {
            this.IsRunning = false;
        }

        private void GameTick()
        {
            Boolean _gotoRetryTick = false;

            do
            {
                _gotoRetryTick = false;

                // Advance the accumulated elapsed time.
                Int64 _currentTicks = this.gameTimer.Elapsed.Ticks;
                this.accumulatedElapsedTime += TimeSpan.FromTicks(_currentTicks - this.previousTicks);
                this.previousTicks = _currentTicks;

                if (this.gameModule.IsFixedTimeStep && this.accumulatedElapsedTime < this.targetElapsedTime)
                {
                    var _sleepTime = (Int32)(this.targetElapsedTime - this.accumulatedElapsedTime).TotalMilliseconds;

                    System.Threading.Thread.Sleep(_sleepTime);
                    _gotoRetryTick = true;
                }

            } while (_gotoRetryTick == true);

            if (this.accumulatedElapsedTime > this.maxElapsedTime)
                this.accumulatedElapsedTime = this.maxElapsedTime;

            if (this.gameModule.IsFixedTimeStep)
            {
                this.gameTime.ElapsedGameTime = this.targetElapsedTime;
                var stepCount = 0;

                // Perform as many full fixed length time steps as we can.
                while (this.accumulatedElapsedTime >= this.targetElapsedTime)
                {
                    this.gameTime.TotalGameTime += this.targetElapsedTime;
                    this.accumulatedElapsedTime -= this.targetElapsedTime;
                    ++stepCount;

                    this.updateAction(this.gameTime);
                }

                //Every update after the first accumulates lag
                this.updateFrameLag += Math.Max(0, stepCount - 1);

                //If we think we are running slowly, wait until the lag clears before resetting it
                if (this.gameTime.IsRunningSlowly)
                {
                    if (this.updateFrameLag == 0)
                        this.gameTime.IsRunningSlowly = false;
                }
                else if (this.updateFrameLag >= 5)
                {
                    //If we lag more than 5 frames, start thinking we are running slowly
                    this.gameTime.IsRunningSlowly = true;
                }

                //Every time we just do one update and one draw, then we are not running slowly, so decrease the lag
                if (stepCount == 1 && this.updateFrameLag > 0)
                    this.updateFrameLag--;

                // Draw needs to know the total elapsed time
                // that occured for the fixed length updates.
                this.gameTime.ElapsedGameTime = TimeSpan.FromTicks(this.targetElapsedTime.Ticks * stepCount);
            }
            else
            {
                // Perform a single variable length update.
                this.gameTime.ElapsedGameTime = this.accumulatedElapsedTime;
                this.gameTime.TotalGameTime += this.accumulatedElapsedTime;
                this.accumulatedElapsedTime = TimeSpan.Zero;

                this.updateAction(this.gameTime);
            }
        }

        #endregion
    }
}
