using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interop.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/*--------------------------------------------------------
 * GameModule.cs
 * 
 * Version: 1.0
 * Author: Onyx
 * Created: 07/11/2015 17:27:41
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace MonoGame.Interop
{
    public abstract class GameModule : Image
    {
        #region FIELDS

        // Render image
        private D3D11Image image;
        private RenderTarget2D renderTarget;
        private Boolean resetImageBackBuffer;

        // Game timer
        private Stopwatch gameTimer;
        private GameTime gameTime;
        private TimeSpan lastRenderingTime;
        private TimeSpan accumulatedElapsedTime;
        private long previousTicks = 0;
        private int updateFrameLag;

        private TimeSpan targetElapsedTime = TimeSpan.FromSeconds(1f / 60f); // 60fps
        private TimeSpan maxElapsedTime = TimeSpan.FromMilliseconds(500f);

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets a value indicating whether the controls runs in the context of a designer (e.g.
        /// Visual Studio Designer or Expression Blend).
        /// </summary>
        /// <value>
        /// <see langword="true" /> if controls run in design mode; otherwise, 
        /// <see langword="false" />.
        /// </value>
        public static Boolean IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                    _isInDesignMode = (Boolean)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;

                return _isInDesignMode.Value;
            }
        }
        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets the current GraphicsDevice.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Gets or sets the current ContentManager.
        /// </summary>
        public ContentManager ContentManager { get; private set; }

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
        public Boolean IsFixedTimeStep { get; private set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates a new GameModule instance.
        /// </summary>
        public GameModule()
            : base()
        {
            this.IsFixedTimeStep = true;
            this.gameTime = new GameTime();
            this.gameTimer = new Stopwatch();
            this.Loaded += this.GameModule_Loaded;
            this.Unloaded += this.GameModule_Unloaded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameModule_Loaded(Object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
                return;

            this.CreateGraphicsDevice();
            this.Services = new GameServiceContainer();
            this.Components = new GameComponentCollection();
            this.ContentManager = new ContentManager(this.Services);

            Hosting.GraphicsDeviceManager _graphicsDeviceManager = new Hosting.GraphicsDeviceManager(this.GraphicsDevice);
            this.Services.AddService(typeof(IGraphicsDeviceService), _graphicsDeviceManager);

            this.CreateImageSource();
            
            this.Initialize();
            this.LoadContent();
            this.BeginRender();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameModule_Unloaded(Object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
                return;

            this.EndRender();
            this.UnloadContent();
            this.DisposeImage();
            this.DisposeRenderTarget();
            this.DisposeGraphicsDevice();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Creates the <see cref="GraphicsDevice"/>.
        /// </summary>
        private void CreateGraphicsDevice()
        {
            // Create Direct3D 11 device.
            var presentationParameters = new PresentationParameters
            {
                // Do not associate graphics device with window.
                DeviceWindowHandle = IntPtr.Zero,
            };
            this.GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, 
                GraphicsProfile.HiDef, 
                presentationParameters);
        }

        /// <summary>
        /// Creates and initialize the image source.
        /// </summary>
        private void CreateImageSource()
        {
            this.image = new D3D11Image();

            this.image.IsFrontBufferAvailableChanged += Image_IsFrontBufferAvailableChanged;
            this.CreateImageBackBuffer();
            this.Source = this.image;
        }

        /// <summary>
        /// Creates the image back buffer.
        /// </summary>
        private void CreateImageBackBuffer()
        {
            Int32 _width = Math.Max((Int32)this.ActualWidth, 1);
            Int32 _height = Math.Max((Int32)this.ActualHeight, 1);

            // Reset buffer and render target
            this.image.SetBackBuffer(null);
            this.DisposeRenderTarget();

            // Create new render target and set it to the image back buffer
            this.renderTarget = new RenderTarget2D(this.GraphicsDevice, _width, _height, false, SurfaceFormat.Bgr32, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents, true);
            this.image.SetBackBuffer(this.renderTarget);
        }

        /// <summary>
        /// Begin rendering the game.
        /// </summary>
        private void BeginRender()
        {
            if (this.gameTimer.IsRunning)
                return;

            CompositionTarget.Rendering += this.Render;
            this.gameTimer.Start();
        }

        /// <summary>
        /// Stop rendering the game.
        /// </summary>
        private void EndRender()
        {
            if (this.gameTimer.IsRunning == false)
                return;

            CompositionTarget.Rendering -= this.Render;
            this.gameTimer.Stop();
        }

        /// <summary>
        /// Renders the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Render(Object sender, EventArgs e)
        {
            if (this.gameTimer.IsRunning == false)
                return;

            if (this.resetImageBackBuffer == true)
                this.CreateImageSource();

            RenderingEventArgs _renderingEventArgs = e as RenderingEventArgs;
            if (this.lastRenderingTime != _renderingEventArgs.RenderingTime || this.resetImageBackBuffer == true)
            {
                this.lastRenderingTime = _renderingEventArgs.RenderingTime;

                this.UpdateGameTime();
                this.GraphicsDevice.SetRenderTarget(this.renderTarget);

                this.Draw(gameTime);
                this.GraphicsDevice.Flush();
            }

            this.image.Invalidate();
            this.resetImageBackBuffer = false;
        }

        /// <summary>
        /// Update the GameTime.
        /// </summary>
        /// <remarks>Some of the code of this function is from the MonoGame project (https://github.com/mono/MonoGame/blob/develop/MonoGame.Framework/Game.cs#L426)</remarks>
        private void UpdateGameTime()
        {
            Boolean _gotoRetryTick = false;

            do
            {
                _gotoRetryTick = false;

                // Advance the accumulated elapsed time.
                Int64 _currentTicks = this.gameTimer.Elapsed.Ticks;
                this.accumulatedElapsedTime += TimeSpan.FromTicks(_currentTicks - this.previousTicks);
                this.previousTicks = _currentTicks;
                
                if (this.IsFixedTimeStep && this.accumulatedElapsedTime < this.targetElapsedTime)
                {
                    var _sleepTime = (Int32)(this.targetElapsedTime - this.accumulatedElapsedTime).TotalMilliseconds;

                    System.Threading.Thread.Sleep(_sleepTime);
                    _gotoRetryTick = true;
                }

            } while (_gotoRetryTick == true);

            if (this.accumulatedElapsedTime > this.maxElapsedTime)
                this.accumulatedElapsedTime = this.maxElapsedTime;

            if (this.IsFixedTimeStep)
            {
                this.gameTime.ElapsedGameTime = this.targetElapsedTime;
                var stepCount = 0;

                // Perform as many full fixed length time steps as we can.
                while (this.accumulatedElapsedTime >= this.targetElapsedTime)
                {
                    this.gameTime.TotalGameTime += this.targetElapsedTime;
                    this.accumulatedElapsedTime -= this.targetElapsedTime;
                    ++stepCount;

                    this.Update(this.gameTime);
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

                this.Update(this.gameTime);
            }
        }

        /// <summary>
        /// Dispose the game render target.
        /// </summary>
        private void DisposeRenderTarget()
        {
            if (this.renderTarget != null)
            {
                this.renderTarget.Dispose();
                this.renderTarget = null;
            }
        }

        /// <summary>
        /// Dispose the game image.
        /// </summary>
        private void DisposeImage()
        {
            this.Source = null;

            if (this.image != null)
            {
                this.image.IsFrontBufferAvailableChanged -= this.Image_IsFrontBufferAvailableChanged;
                this.image.Dispose();
                this.image = null;
            }
        }

        /// <summary>
        /// Dispose the game GraphicsDevice.
        /// </summary>
        private void DisposeGraphicsDevice()
        {
            if (this.GraphicsDevice != null)
            {
                this.GraphicsDevice.Dispose();
                this.GraphicsDevice = null;
            }
        }
        
        #endregion

        #region EVENTS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_IsFrontBufferAvailableChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.image.IsFrontBufferAvailable)
            {
                this.BeginRender();
                this.resetImageBackBuffer = true;
            }
            else
                this.EndRender();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            this.resetImageBackBuffer = true;
            base.OnRenderSizeChanged(sizeInfo);
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
    }
}
