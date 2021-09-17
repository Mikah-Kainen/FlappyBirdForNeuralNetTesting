using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NeuralNetwork;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FlappyBird
{
    public class PlayScreen : Screen
    {
        ErrorFunction meanSquared = new ErrorFunction((double output, double desired) => (output - desired) * (output - desired), (double output, double desired) => -2 * (output - desired));
        ActivationFunction tanh = new ActivationFunction(Math.Tanh, (double input) => 1 - Math.Tanh(input) * Math.Tanh(input));

        public Texture2D WhitePixel;
        public GraphicsDevice GraphicsDevice;
        public List<Pipe> Pipes;
        public double LastElapsedTime;
        public Random Random;
        public Vector2 ScreenSize;
        public Bird[] Flappies;
        public int timeToNextPipe;
        public NeuralNet[] Nets;
        public PlayScreen(GraphicsDevice graphicsDevice, Vector2 screenSize)
            :base()
        {
            GraphicsDevice = graphicsDevice;
            WhitePixel = Color.White.CreatePixel(GraphicsDevice);
            Pipes = new List<Pipe>();
            LastElapsedTime = -3000;
            Random = new Random();
            Flappies = new Bird[100];
            Nets = new NeuralNet[Flappies.Length];
            ScreenSize = screenSize;
            Vector2 flappySize = new Vector2(40, 40);
            int[] Layers = new int[]
            {
                2,3,2,1,
            };
            for (int i = 0; i < Flappies.Length; i ++)
            {
                Flappies[i] = new Bird(Color.White.CreatePixel(graphicsDevice), Color.Green, screenSize / 2 - flappySize, Vector2.One, flappySize, Vector2.Zero);
                Flappies[i].Pos = new Vector2(ScreenSize.X / 2 - Flappies[i].Size.X, ScreenSize.Y / 2 - Flappies[i].Size.Y - i * 2);
                GameObjects.Add(Flappies[i]);

                Nets[i] = new NeuralNet(meanSquared, tanh, Layers);
                Nets[i].Randomize(Random,);
            }
            timeToNextPipe = 4000;
        }

        public override void Update(GameTime gameTime)
        {
            for(int i = 0; i < Flappies.Length; i ++)
            {
                if(Flappies[i].IsVisible)
                {
                    Flappies[i].SurvivalTime += gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            if(gameTime.TotalGameTime.TotalMilliseconds - LastElapsedTime > timeToNextPipe)
            {
                timeToNextPipe = Random.Next(1500, 6000);
                LastElapsedTime = gameTime.TotalGameTime.TotalMilliseconds;
                Pipe nextPipe = new Pipe(40, new Vector2(40, 150), ScreenSize, Random, GraphicsDevice);
                Pipes.Add(nextPipe);
                GameObjects.Add(nextPipe);
            }
            base.Update(gameTime);


            for (int i = 0; i < Flappies.Length; i ++)
            {
                bool shouldInvisible = false;
                foreach (Pipe pipe in Pipes)
                {
                    if (Flappies[i].HitBox.Intersects(pipe.HitBox) || Flappies[i].HitBox.Intersects(pipe.OtherPart.HitBox))
                    {
                        shouldInvisible = true;
                    }
                }
                if (Flappies[i].Pos.Y + Flappies[i].HitBox.Height > ScreenSize.Y || shouldInvisible)
                {
                    Flappies[i].IsVisible = false;
                }
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
            GameObjects.Clear();
            for (int i = 0; i < Flappies.Length; i ++)
            {
                Flappies[i].Pos = new Vector2(ScreenSize.X / 2 - Flappies[i].Size.X, ScreenSize.Y / 2 - Flappies[i].Size.Y - i * 2);
                Flappies[i].IsVisible = true;
                Flappies[i].SurvivalTime = 0;
                GameObjects.Add(Flappies[i]);
            }
            timeToNextPipe = 4000;
        }
    }
}
