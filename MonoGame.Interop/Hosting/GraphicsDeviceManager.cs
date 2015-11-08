using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame.Interop.Hosting
{
	public class GraphicsDeviceManager : IGraphicsDeviceService
	{
		public GraphicsDevice GraphicsDevice { get; private set; }

		public GraphicsDeviceManager(GraphicsDevice device)
		{
			GraphicsDevice = device;
		}
		public event EventHandler<EventArgs> DeviceCreated;
		public event EventHandler<EventArgs> DeviceDisposing;
		public event EventHandler<EventArgs> DeviceReset;
		public event EventHandler<EventArgs> DeviceResetting;
	}
}