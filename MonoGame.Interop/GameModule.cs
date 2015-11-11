using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Interop.Controls;
using MonoGame.Interop.Input;
using System;
using System.Windows.Controls;
using System.Windows.Input;

/*--------------------------------------------------------
 * GameModule.cs
 * 
 * Version: 1.1
 * Author: Onyx
 * Created: 07/11/2015 17:27:41
 * 
 * Credits:
 * This 'GameModule' uses some MonoGame code 
 * http://blogs.msdn.com/b/nicgrave/archive/2011/03/25/wpf-hosting-for-xna-game-studio-4-0.aspx
 * DrawingSurface by tgjones (https://github.com/tgjones/gemini/blob/master/src/Gemini.Modules.MonoGame/Controls/DrawingSurface.cs)
 * 
 * Notes:
 * 1.1: Review the game module; now using a DrawingSurface.
 *
 * -------------------------------------------------------*/

namespace MonoGame.Interop
{
    public class GameModule : ContentControl
    {
        #region FIELDS

        private DrawingSurface drawingSurface;
        private GameModuleUpdater updater;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return this.drawingSurface.GraphicsDevice; }
        }

        /// <summary>
        /// Gets or sets the current ContentManager.
        /// </summary>
        public ContentManager ContentManager { get; set; }

        /// <summary>
        /// Gets the collection of GameComponents owned by the game.
        /// </summary>
        public GameComponentCollection Components { get; private set; }

        /// <summary>
        /// Gets the GameServiceContainer holding all the service providers attached to the Game.
        /// </summary>
        public GameServiceContainer Services { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use fixed time steps.
        /// </summary>
        public Boolean IsFixedTimeStep { get; set; }

        /// <summary>
        /// Gets the mouse state.
        /// </summary>
        internal MouseState MouseState { get; private set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates a new GameModule's instance.
        /// </summary>
        public GameModule()
        {
            this.IsFixedTimeStep = true;

            // Initialize mouse and keyboard parent game module
            WPFMouse.PrimaryGameModule = this;
            WPFKeyboard.primaryGameModule = this;
            
            // Initialize drawing surface
            this.drawingSurface = new DrawingSurface();
            this.drawingSurface.Loaded += DrawingSurface_Loaded;
            this.drawingSurface.Unloaded += DrawingSurface_Unloaded;
            this.drawingSurface.LoadContent += this.DrawingSurface_LoadContent;
            this.drawingSurface.Draw += this.DrawingSurface_Draw;
            this.drawingSurface.MouseMove += this.UpdateMouse;
            this.drawingSurface.MouseDown += this.UpdateMouse;
            this.drawingSurface.MouseUp += this.UpdateMouse;

            // Initialize game updater
            this.updater = new GameModuleUpdater(this, this.Update);

            // Set the drawing surface as the GameModule's content
            this.Content = this.drawingSurface;
        }

        #endregion

        #region EVENTS METHODS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingSurface_Loaded(Object sender, System.Windows.RoutedEventArgs e)
        {
            this.BeginRun();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingSurface_Unloaded(Object sender, System.Windows.RoutedEventArgs e)
        {
            this.EndRun();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingSurface_Draw(Object sender, DrawEventArgs e)
        {
            if (this.drawingSurface != null)
            {
                this.Draw(this.updater.GameTime);
                this.drawingSurface.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingSurface_LoadContent(Object sender, GraphicsDeviceEventArgs e)
        {
            this.Services = new GameServiceContainer();
            this.Components = new GameComponentCollection();
            this.ContentManager = new ContentManager(this.Services);

            this.Services.AddService(typeof(GraphicsDevice), this.GraphicsDevice);

            this.Initialize();
            this.LoadContent();
            this.BeginRun();
        }
        
        #endregion

        #region METHODS

        /// <summary>
        /// Begin running the game module.
        /// </summary>
        private void BeginRun()
        {
            if (this.updater.IsRunning == false)
                this.updater.Start();
        }

        /// <summary>
        /// Ends running the game module.
        /// </summary>
        private void EndRun()
        {
            if (this.updater.IsRunning)
                this.updater.Stop();
        }

        #endregion

        #region VIRTUAL

        /// <summary>
        /// Called after the Game and <see cref="GraphicsDevice"/> are created, but before <see cref="LoadContent"/>.
        /// </summary>
        protected virtual void Initialize()
        {
            // TODO: Initialize components
        }

        /// <summary>
        /// Called when graphics resources need to be loaded. 
        /// Override this method to load any game-specific graphics resources.
        /// </summary>
        protected virtual void LoadContent()
        {
            // TODO: Load components content
        }

        /// <summary>
        /// Called when graphics resources need to be unloaded. 
        /// Override this method to unload any game-specific graphics resources.
        /// </summary>
        protected virtual void UnloadContent()
        {
            // TODO: unload components
        }

        /// <summary>
        /// Called when the game has determined that game logic needs to be processed.
        /// This might include the management of the game state, the processing of user input, 
        /// or the updating of simulation data. Override this method with game-specific logic.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        protected virtual void Update(GameTime gameTime)
        {
            // TODO: update components
        }

        /// <summary>
        /// Called when the game determines it is time to draw a frame. 
        /// Override this method with game-specific rendering code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        protected virtual void Draw(GameTime gameTime)
        {
            // TODO: draw components
        }

        #endregion

        #region INPUT UPDATE

        /// <summary>
        /// Update mouse state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateMouse(Object sender, MouseEventArgs e)
        {
            if (this.IsVisible == false || this.IsMouseOver == false || e.Handled)
                return;

            e.Handled = true;
            WPFMouse.Position = e.GetPosition(this);

            if (e is MouseWheelEventArgs)
                WPFMouse.MouseScrollWheelValue = (e as MouseWheelEventArgs).Delta;

            this.MouseState = new MouseState((Int32)WPFMouse.Position.X, (Int32)WPFMouse.Position.Y,
                WPFMouse.MouseScrollWheelValue,
                this.GetButtonState(e.LeftButton),
                this.GetButtonState(e.MiddleButton),
                this.GetButtonState(e.RightButton), ButtonState.Released, ButtonState.Released);
        }

        /// <summary>
        /// Converts a <see cref="MouseButtonState"/> into a <see cref="ButtonState"/>.
        /// </summary>
        /// <param name="mouseState">Window mouse button state.</param>
        /// <returns></returns>
        private ButtonState GetButtonState(MouseButtonState mouseState)
        {
            return mouseState == MouseButtonState.Pressed ? ButtonState.Pressed : ButtonState.Released;
        }

        #endregion
    }
}
