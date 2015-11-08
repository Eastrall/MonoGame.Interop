using Microsoft.Xna.Framework;
using MonoGame.Interop.Wpf.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*--------------------------------------------------------
 * MainGame.cs
 * 
 * Version: 1.0
 * Author: Onyx
 * Created: 07/11/2015 18:59:46
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace MonoGame.Interop.Wpf
{
    public class MainGame : GameModule
    {
        CubePrimitive cube;

        public MainGame()
            : base()
        {
        }

        protected override void Initialize()
        {
            // Create the cube
            this.cube = new CubePrimitive(this.GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            // directly stolem from Nick Gravelyn's sample (http://blogs.msdn.com/b/nicgrave/archive/2011/03/25/wpf-hosting-for-xna-game-studio-4-0.aspx)

            // Compute some values for the cube rotation
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            float yaw = time * 0.4f;
            float pitch = time * 0.7f;
            float roll = time * 1.1f;

            // Create the world-view-projection matrices for the cube and camera
            Matrix world = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 2.5f), Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, GraphicsDevice.Viewport.AspectRatio, 1, 10);

            // Get the color from our sliders

            // Draw a cube
            this.cube.Draw(world, view, projection, Color.Red);

            base.Draw(gameTime);
        }
    }
}
