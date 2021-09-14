using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FlappyBird
{
    public class PlayScreen : Screen
    {

        public Texture2D WhitePixel;
        public GraphicsDevice GraphicsDevice;
        public List<Pipe> Pipes;
        public double LastElapsedTime;
        public Random Random;
        public Vector2 ScreenSize;

        public PlayScreen(GraphicsDevice graphicsDevice, Vector2 screenSize)
            :base()
        {
            GraphicsDevice = graphicsDevice;
            WhitePixel = Color.White.CreatePixel(GraphicsDevice);
            Pipes = new List<Pipe>();
            LastElapsedTime = 0;
            Random = new Random();
            ScreenSize = screenSize;
        }

        public override void Update(GameTime gameTime)
        {
            if(gameTime.TotalGameTime.TotalMilliseconds - LastElapsedTime > 500)
            {
                LastElapsedTime = gameTime.TotalGameTime.TotalMilliseconds;
                Pipes.Add(new Pipe(new Vector2(50, 100), ScreenSize, Random, GraphicsDevice));
            }
            foreach (Pipe pipe in Pipes)
            {
                pipe.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach(Pipe pipe in Pipes)
            {
                pipe.Draw(spriteBatch);
            }
        }


    }
}
