using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interop.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using DTexture = SharpDX.Direct3D9.Texture;

/*--------------------------------------------------------
 * DrawingSurface.cs
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 10/11/2015 19:42:31
 * 
 * Notes:
 * Thank you to tgjones for the drawing surface.
 * https://github.com/tgjones/gemini/blob/master/src/Gemini.Modules.MonoGame/Controls/DrawingSurface.cs
 * -------------------------------------------------------*/

namespace MonoGame.Interop.Controls
{
    public class DrawingSurface : ContentControl, IDisposable
    {
        #region FIELDS

        private GraphicsDeviceService graphicsDeviceService;
        private RenderTarget2D renderTarget;
        private DTexture renderTargetD3D9;

        // Image
        private D3DImage d3dimage;
        private Image image;

        // States
        private Boolean needsRefresh;

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
        private static Boolean? _isInDesignMode;

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return this.graphicsDeviceService.GraphicsDevice; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control will redraw every time the CompositionTarget.Rendering event is fired.
        /// Defaults to false.
        /// </summary>
        public bool AlwaysRefresh { get; set; }

        #endregion

        #region EVENTS

        public event EventHandler<GraphicsDeviceEventArgs> LoadContent;

        public event EventHandler<DrawEventArgs> Draw;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates a new DrawingSurface object.
        /// </summary>
        public DrawingSurface()
        {
            // Create the d3dimage
            this.d3dimage = new D3DImage();
            this.d3dimage.IsFrontBufferAvailableChanged += D3dimage_IsFrontBufferAvailableChanged;

            // Create the image and set the source
            this.image = new Image()
            {
                Stretch = Stretch.None,
                Source = this.d3dimage
            };

            this.AddChild(this.image);

            // Initalize events
            this.Loaded += this.DrawingSurface_Loaded;
            this.Unloaded += this.DrawingSurface_Unloaded;
        }

        /// <summary>
        /// Load the DrawingSurface's content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingSurface_Loaded(Object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsInDesignMode)
                return;

            if (this.graphicsDeviceService == null)
            {
                DeviceService.StartD3D(Window.GetWindow(this));

                this.graphicsDeviceService = GraphicsDeviceService.AddRef(new WindowInteropHelper(Application.Current.MainWindow).Handle, 1, 1);
                this.graphicsDeviceService.DeviceResetting += GraphicsDeviceService_DeviceResetting;

                if (this.LoadContent != null)
                    this.LoadContent(this, new GraphicsDeviceEventArgs(this.GraphicsDevice));

                this.CreateRenderTarget();

                CompositionTarget.Rendering += this.RenderSurface;

                this.needsRefresh = true;
            }
        }

        /// <summary>
        /// Unload the DrawingSurface's content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingSurface_Unloaded(Object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsInDesignMode)
                return;

            if (this.graphicsDeviceService != null)
            {
                // Remove back buffer references by disposing it
                this.DisposeBackBuffer();
                CompositionTarget.Rendering -= this.RenderSurface;

                // Remove events from graphics device service
                this.graphicsDeviceService.DeviceResetting -= this.GraphicsDeviceService_DeviceResetting;
                this.graphicsDeviceService = null;

                DeviceService.EndD3D();
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Create the RenderTarget.
        /// </summary>
        private void CreateRenderTarget()
        {
            if (this.renderTarget == null)
            {
                try
                {
                    this.renderTarget = new RenderTarget2D(this.GraphicsDevice, (int)ActualWidth, (int)ActualHeight,
                        false, SurfaceFormat.Bgra32, DepthFormat.Depth24Stencil8, 1,
                        RenderTargetUsage.PlatformContents, true);

                    var handle = this.renderTarget.GetSharedHandle();
                    if (handle == IntPtr.Zero)
                        throw new ArgumentException("Handle could not be retrieved");

                    this.renderTargetD3D9 = new DTexture(DeviceService.D3DDevice,
                        this.renderTarget.Width, this.renderTarget.Height,
                        1, SharpDX.Direct3D9.Usage.RenderTarget, SharpDX.Direct3D9.Format.A8R8G8B8,
                        SharpDX.Direct3D9.Pool.Default, ref handle);

                    using (SharpDX.Direct3D9.Surface surface = this.renderTargetD3D9.GetSurfaceLevel(0))
                    {
                        this.d3dimage.Lock();
                        this.d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
                        this.d3dimage.Unlock();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("An error occured while creating the render target");
                    this.renderTarget = null;   
                }
            }
        }

        /// <summary>
        /// Render on the DrawingSurface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenderSurface(Object sender, EventArgs e)
        {
            if ((this.needsRefresh || this.AlwaysRefresh) && this.CanBeginDraw())
            {
                this.needsRefresh = false;

                this.d3dimage.Lock();
                this.CreateRenderTarget();
                this.GraphicsDevice.SetRenderTarget(this.renderTarget);
                this.SetViewport();

                if (this.Draw != null)
                    this.Draw(this, new DrawEventArgs(this, this.GraphicsDevice));

                this.GraphicsDevice.Flush();

                this.d3dimage.AddDirtyRect(new Int32Rect(0, 0, (Int32)this.ActualWidth, (Int32)this.ActualHeight));

                this.d3dimage.Unlock();

                this.GraphicsDevice.SetRenderTarget(null);
            }
        }

        /// <summary>
        /// Set the viewport for the drawing surface.
        /// </summary>
        /// <remarks>
        /// Many GraphicsDeviceControl instances can be sharing the same GraphicsDevice. 
        /// The device backbuffer will be resized to fit the largest of these controls. 
        /// But what if we are currently drawing a smaller control?
        /// To avoid unwanted stretching, we set the viewport to only use 
        /// the top left portion of the full backbuffer.
        /// </remarks>
        private void SetViewport()
        {
            this.GraphicsDevice.Viewport = new Viewport(
                0, 0, Math.Max(1, (Int32)ActualWidth), Math.Max(1, (Int32)ActualHeight));
        }

        /// <summary>
        /// Check if we can begin to draw on the DrawingSurface.
        /// </summary>
        /// <returns></returns>
        private Boolean CanBeginDraw()
        {
            return this.graphicsDeviceService != null && 
                this.d3dimage.IsFrontBufferAvailable &&
                this.HandleDeviceReset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Boolean HandleDeviceReset()
        {
            Boolean _deviceNeedsReset = false;

            switch (this.GraphicsDevice.GraphicsDeviceStatus)
            {
                // If the graphics device is lost, we cannot use it at all.
                case GraphicsDeviceStatus.Lost: return false;

                // If device is in the not-reset state, we should try to reset it.
                case GraphicsDeviceStatus.NotReset: _deviceNeedsReset = true; break;
            }

            if (_deviceNeedsReset)
            {
                this.graphicsDeviceService.ResetDevice((Int32)ActualWidth, (Int32)ActualHeight);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Invalidate the DrawingSurface.
        /// </summary>
        public void Invalidate()
        {
            this.needsRefresh = true;
        }

        /// <summary>
        /// Event fired when we resize the DrawingSurface.
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            this.DisposeBackBuffer();
            this.needsRefresh = true;

            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// Event fired when we reset the Device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphicsDeviceService_DeviceResetting(Object sender, EventArgs e)
        {
            this.DisposeBackBuffer();
            this.needsRefresh = true;
        }

        /// <summary>
        /// Event fired when the new data is availiable on the front buffer of
        /// the DrawingSurface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void D3dimage_IsFrontBufferAvailableChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.d3dimage.IsFrontBufferAvailable)
                this.needsRefresh = true;
        }

        /// <summary>
        /// Dispose the image back buffer resources.
        /// </summary>
        private void DisposeBackBuffer()
        {
            this.DisposeRenderTarget();
            this.DisposeRenderTargetD3D9();

            this.d3dimage.Lock();
            this.d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            this.d3dimage.Unlock();   
        }

        /// <summary>
        /// Dispose the render target.
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
        /// Dispose the D3D9's render target.
        /// </summary>
        private void DisposeRenderTargetD3D9()
        {
            if (this.renderTargetD3D9 != null)
            {
                this.renderTargetD3D9.Dispose();
                this.renderTargetD3D9 = null;
            }
        }

        #endregion

        #region IDisposable Support

        private Boolean disposedValue = false;

        /// <summary>
        /// Dispose the DrawingSurface.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(Boolean disposing)
        {
            if (this.disposedValue == false)
            {
                this.DisposeRenderTarget();
                this.DisposeRenderTargetD3D9();

                if (this.graphicsDeviceService != null)
                    this.graphicsDeviceService.Release(disposing);

                this.disposedValue = true;
            }
        }

        /// <summary>
        /// Destructs the drawing surface.
        /// </summary>
        ~DrawingSurface()
        {
            this.Dispose(false);
        }
        
        /// <summary>
        /// Dipose the drawing surface.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
