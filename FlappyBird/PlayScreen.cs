using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NeuralNetwork;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public Pipe ClosestPipe;
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
                Flappies[i].Pos = new Vector2(ScreenSize.X / 5 - Flappies[i].Size.X, ScreenSize.Y / 2 - Flappies[i].Size.Y - i * 2);
                GameObjects.Add(Flappies[i]);

                Nets[i] = new NeuralNet(meanSquared, tanh, Layers);
                Nets[i].Randomize(Random, -50, 50);
            }
            timeToNextPipe = 4000;
            ClosestPipe = null;
        }

        public override void Update(GameTime gameTime)
        {
            if(Pipes.Count > 0 && Pipes[0].Pos.X + Pipes[0].HitBox.Width < 0)
            {
                Pipes.RemoveAt(0);
            }

            bool shouldTrain = true;
            for(int i = 0; i < Flappies.Length; i ++)
            {
                if(Flappies[i].IsVisible)
                {
                    Flappies[i].SurvivalTime += gameTime.ElapsedGameTime.TotalSeconds;
                    double[] inputs;
                    if (ClosestPipe != null)
                    {
                        inputs = new double[]
                        {
                        ClosestPipe.Pos.X + ClosestPipe.HitBox.Width - Flappies[i].Pos.X,
                        Flappies[i].Pos.Y - ClosestPipe.Pos.Y - ClosestPipe.SpaceBetweenPipes.Y * 3 / 4,
                        //check this later
                        };

                    }
                    else
                    {
                        inputs = new double[]
                        {
                            100, 
                            0,
                        };
                    }
                    Flappies[i].ShouldJump = Nets[i].Compute(inputs)[0];
                    shouldTrain = false;
                }
            }
            if(shouldTrain)
            {
                Train();
                RestartGame(gameTime);
            }

            if(gameTime.TotalGameTime.TotalMilliseconds - LastElapsedTime > timeToNextPipe)
            {
                timeToNextPipe = Random.Next(1500, 4000);
                LastElapsedTime = gameTime.TotalGameTime.TotalMilliseconds;
                Pipe nextPipe = new Pipe(40, new Vector2(40, 150), ScreenSize, Random, GraphicsDevice);
                Pipes.Add(nextPipe);
                GameObjects.Add(nextPipe);
            }
            base.Update(gameTime);


            for (int i = 0; i < Flappies.Length; i ++)
            {
                bool shouldInvisible = false;
                for(int x = 0; x < Pipes.Count; x ++ )
                {
                    if (Flappies[i].HitBox.Intersects(Pipes[x].HitBox) || Flappies[i].HitBox.Intersects(Pipes[x].OtherPart.HitBox))
                    {
                        shouldInvisible = true;
                    }

                    if (ClosestPipe == null)
                    {
                        ClosestPipe = Pipes[0];
                    }
                    else if (ClosestPipe.Pos.X + ClosestPipe.HitBox.Width < Flappies[i].Pos.X)
                    {
                        ClosestPipe = Pipes[x + 1];
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

        public void Train()
        {
            List<Bird> newFlappies = new List<Bird>();
            List<NeuralNet> newNets = new List<NeuralNet>();
            /*(IEnumerable<Bird>, IEnumerable<NeuralNet>)*/
            var result =
                       Enumerable.Zip<Bird, NeuralNet, (Bird bird, NeuralNet net)>(Flappies, Nets, (b, n) => (b, n))
                       .OrderBy(pair => pair.bird.SurvivalTime)
                       .Select(pair =>
             {
                 newFlappies.Insert(0, pair.bird);
                 newNets.Insert(0, pair.net);
                 return pair;
             }).ToList();

            //var IWannaSeeThisList = Flappies;
            //var IAlsoWannaSeeThisOne = Nets;
            Flappies = newFlappies.ToArray();
            Nets = newNets.ToArray();


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
