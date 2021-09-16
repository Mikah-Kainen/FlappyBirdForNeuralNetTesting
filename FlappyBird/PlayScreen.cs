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
        public Bird Flappy;
        public double gameLength;

        public PlayScreen(GraphicsDevice graphicsDevice, Vector2 screenSize)
            :base()
        {
            GraphicsDevice = graphicsDevice;
            WhitePixel = Color.White.CreatePixel(GraphicsDevice);
            Pipes = new List<Pipe>();
            LastElapsedTime = -3000;
            Random = new Random();
            ScreenSize = screenSize;
            Vector2 flappySize = new Vector2(40, 40);
            Flappy = new Bird(Color.White.CreatePixel(graphicsDevice), Color.Green, screenSize / 2 - flappySize, Vector2.One, flappySize, Vector2.Zero);
            GameObjects.Add(Flappy);
            gameLength = 0;
        }

        public override void Update(GameTime gameTime)
        {
            gameLength += 16;
            if(gameTime.TotalGameTime.TotalMilliseconds - LastElapsedTime > 4000)
            {
                LastElapsedTime = gameTime.TotalGameTime.TotalMilliseconds;
                Pipe nextPipe = new Pipe(40, new Vector2(40, 150), ScreenSize, Random, GraphicsDevice);
                Pipes.Add(nextPipe);
                GameObjects.Add(nextPipe);
            }
            base.Update(gameTime);

            bool shouldRestart = false;
            foreach (Pipe pipe in Pipes)
            {
                if (Flappy.HitBox.Intersects(pipe.HitBox) || Flappy.HitBox.Intersects(pipe.OtherPart.HitBox))
                {
                    shouldRestart = true;
                }
            }
            if(Flappy.Pos.Y + Flappy.HitBox.Height > ScreenSize.Y || shouldRestart)
            {
                RestartGame(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public void RestartGame(GameTime gameTime)
        {
            Pipes.Clear();
            LastElapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds - 3000;
            Flappy.Pos = ScreenSize / 2 - Flappy.Size;
            GameObjects.Clear();
            GameObjects.Add(Flappy);
            gameLength = 0;
        }
    }
}
