using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Interop.Wpf.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*--------------------------------------------------------
 * OtherMainGame.cs
 * 
 * Version: 1.0
 * Author: Onyx
 * Created: 07/11/2015 18:59:46
 * 
 * Notes:
 * -------------------------------------------------------*/

namespace MonoGame.Interop.Wpf
{
    public class OtherMainGame : GameModule
    {
        private CubePrimitive cube;
        private SpriteBatch spriteBatch;
        private Texture2D texture;

        public OtherMainGame()
            : base()
        {
            this.Focus();
        }

        protected override void Initialize()
        {
            // Create the cube
            this.cube = new CubePrimitive(this.GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Load content
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            // Create a black gray texture
            this.texture = new Texture2D(this.GraphicsDevice, 50, 50);
            Color[] _data = new Color[50 * 50];
            this.texture.GetData(_data);
            for (Int32 i = 0; i < _data.Length; ++i)
                _data[i] = Color.Gray;
            this.texture.SetData(_data);

            base.LoadContent();
        }
        private Color backcolor = Color.CornflowerBlue;

        protected override void Update(GameTime gameTime)
        {
            // TODO: Update logic

            // Mouse input
            MouseState _state = Input.WPFMouse.GetState();

            if (_state.LeftButton == ButtonState.Pressed && _state.RightButton == ButtonState.Released)
                this.backcolor = Color.Green;
            else if (_state.LeftButton == ButtonState.Released && _state.RightButton == ButtonState.Pressed)
                this.backcolor = Color.Yellow;
            else if (_state.LeftButton == ButtonState.Pressed && _state.RightButton == ButtonState.Pressed)
                this.backcolor = Color.Blue;
            else
                this.backcolor = Color.CornflowerBlue;

            // Keyboard input
            KeyboardState _keyboardState = Input.WPFKeyboard.GetState();

            if (_keyboardState.IsKeyDown(Keys.A) && _keyboardState.IsKeyDown(Keys.Z))
                this.backcolor = Color.Black;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(this.backcolor);

            // from Nick Gravelyn's sample (http://blogs.msdn.com/b/nicgrave/archive/2011/03/25/wpf-hosting-for-xna-game-studio-4-0.aspx)

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
            this.cube.Draw(world, view, projection, Color.Green);

            // Draw a square with the spriteBatch
            this.spriteBatch.Begin();
            this.spriteBatch.Draw(this.texture, new Vector2(10, 10), Color.White);
            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
