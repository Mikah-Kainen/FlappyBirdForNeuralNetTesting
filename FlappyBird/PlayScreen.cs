using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NeuralNetwork;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace FlappyBird
{
    public class PlayScreen : Screen
    {
        public const double RandomizeMin = -50;
        public const double RandomizeMax = 50;

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
        public int CurrentGeneration;

        public bool WasSpacePressed;
        public bool WasEnterPressed;

        public PlayScreen(GraphicsDevice graphicsDevice, Vector2 screenSize)
            : base()
        {
            GraphicsDevice = graphicsDevice;
            WhitePixel = Color.White.CreatePixel(GraphicsDevice);
            Pipes = new List<Pipe>();
            LastElapsedTime = -3000;
            Random = new Random();
            Flappies = new Bird[1000];
            Nets = new NeuralNet[Flappies.Length];
            ScreenSize = screenSize;
            Vector2 flappySize = new Vector2(40, 40);
            int[] Layers = new int[]
            {
                2,3,2,1,
            };
            for (int i = 0; i < Flappies.Length; i++)
            {
                Flappies[i] = new Bird(Color.White.CreatePixel(graphicsDevice), Color.Green, screenSize / 2 - flappySize, Vector2.One, flappySize, Vector2.Zero);
                Flappies[i].Pos = new Vector2(ScreenSize.X / 5 - Flappies[i].Size.X, ScreenSize.Y / 2 - Flappies[i].Size.Y);
                GameObjects.Add(Flappies[i]);

                Nets[i] = new NeuralNet(ErrorFunctions.MeanSquared, ActivationFunctions.Tanh, Layers);
                Nets[i].Randomize(Random, RandomizeMin, RandomizeMax);
            }
            timeToNextPipe = 4000;
            ClosestPipe = null;
            CurrentGeneration = 1;

            WasSpacePressed = false;
            WasEnterPressed = false;
        }


        NeuralNet savedNet;


        public override void Update(GameTime gameTime)
        {
            if (InputManager.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                WasSpacePressed = true;
            }
            if (InputManager.KeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Space) && WasSpacePressed)
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

                Flappies = newFlappies.ToArray();
                Nets = newNets.ToArray();

                savedNet = Nets[0];

                var json = JsonConvert.SerializeObject(Nets[0]);
                System.IO.File.WriteAllText("NeuralNet.json", json);
                WasSpacePressed = false;
            }
            if (InputManager.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                WasEnterPressed = true;
            }
            if (InputManager.KeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Enter) && WasEnterPressed)
            {
                RestartGame(gameTime);
                Nets = new NeuralNet[1];
                Nets[0] = savedNet;
                Flappies[0].IsVisible = true;


                string json = System.IO.File.ReadAllText("NeuralNet.json");
                //Nets[0] = JsonConvert.DeserializeObject<NeuralNet>(json);
                for (int i = Nets[0].Layers.Length - 1; i > 0; i--)
                {
                    Nets[0].Layers[i].PreviousLayer = Nets[0].Layers[i - 1];
                    Nets[0].Layers[i].Output = new double[Nets[0].Layers[i].Neurons.Length];
                    for (int x = 0; x < Nets[0].Layers[i].Neurons.Length; x++)
                    {
                        for (int z = 0; z < Nets[0].Layers[i].PreviousLayer.Neurons.Length; z++)
                        {
                            Nets[0].Layers[i].Neurons[x].Dentrites[z].Previous = Nets[0].Layers[i].PreviousLayer.Neurons[z];
                        }
                    }
                }

                double[] inputs;
                if (ClosestPipe != null)
                {
                    inputs = new double[]
                    {
                        ClosestPipe.Pos.X + ClosestPipe.HitBox.Width - Flappies[0].Pos.X,
                        Flappies[0].Pos.Y - ClosestPipe.Pos.Y - ClosestPipe.SpaceBetweenPipes.Y * 3 / 4,
                    };

                }
                else
                {
                    inputs = new double[]
                    {
                            ScreenSize.X / 2,
                            ScreenSize.Y / 2,
                    };
                }
                Flappies[0].ShouldJump = Nets[0].Compute(inputs)[0];
                WasEnterPressed = false;
                CurrentGeneration--;
            }

            if (Pipes.Count > 0 && Pipes[0].Pos.X + Pipes[0].HitBox.Width < 0)
            {
                Pipes.RemoveAt(0);
            }

            bool shouldTrain = true;
            int FlappyCount = 0;
            double TotalMilliseconds = 0;
            for (int i = 0; i < Nets.Length; i++)
            {
                if (Flappies[i].IsVisible)
                {
                    FlappyCount++;
                    TotalMilliseconds = Flappies[i].SurvivalTime;
                    Flappies[i].SurvivalTime += gameTime.ElapsedGameTime.TotalSeconds;
                    double[] inputs;
                    if (ClosestPipe != null)
                    {
                        inputs = new double[]
                        {
                        ClosestPipe.Pos.X + ClosestPipe.HitBox.Width - Flappies[i].Pos.X,
                        Flappies[i].Pos.Y - ClosestPipe.Pos.Y - ClosestPipe.SpaceBetweenPipes.Y * 3 / 4,
                        };

                    }
                    else
                    {
                        inputs = new double[]
                        {
                            ScreenSize.X / 2,
                            ScreenSize.Y / 2,
                        };
                    }
                    Flappies[i].ShouldJump = Nets[i].Compute(inputs)[0];
                    shouldTrain = false;
                }
            }
            for(int i = Nets.Length; i < Flappies.Length; i ++)
            {
                Flappies[i].IsVisible = false;
            }
            if (shouldTrain)
            {
                Train();
                RestartGame(gameTime);
            }

            if (gameTime.TotalGameTime.TotalMilliseconds - LastElapsedTime > timeToNextPipe)
            {
                timeToNextPipe = Random.Next(1500, 4000);
                LastElapsedTime = gameTime.TotalGameTime.TotalMilliseconds;
                Pipe nextPipe = new Pipe(40, new Vector2(40, 150), ScreenSize, Random, GraphicsDevice);
                Pipes.Add(nextPipe);
                GameObjects.Add(nextPipe);
            }


            for (int i = 0; i < Flappies.Length; i++)
            {
                bool shouldInvisible = false;
                for (int x = 0; x < Pipes.Count; x++)
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

            Game1.Title = $"FlappyCount: {FlappyCount}, GameTime: {TotalMilliseconds:0.00}, CurrentGeneration: {CurrentGeneration}";

            base.Update(gameTime);
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

            Flappies = newFlappies.ToArray();
            Nets = newNets.ToArray();

            for (int i = Nets.Length * 1 / 10; i < Nets.Length * 9 / 10; i++)
            {
                for (int x = 0; x < Nets[i].Layers.Length; x++)
                {
                    NeuralNet parent = Nets[Random.Next(0, 10)];
                    int length = Nets[i].Layers[x].Neurons.Length;
                    int crossoverPoint = Random.Next(0, length);
                    for (int z = 0; z < crossoverPoint; z++)
                    {
                        Layer previousLayer = null;
                        if(x > 0)
                        {
                            previousLayer = Nets[i].Layers[x - 1];
                        }
                        Nets[i].Layers[x].Neurons[z] = new Neuron(parent.Layers[x].Neurons[z], Nets[i].Layers[x].Neurons[z].Dentrites, previousLayer);
                    }
                    for(int z = crossoverPoint; z < Nets[i].Layers[x].Neurons.Length; z ++)
                    {
                        Layer previousLayer = null;
                        if (x > 0)
                        {
                            previousLayer = Nets[i].Layers[x - 1];
                        }
                        Nets[i].Layers[x].Neurons[z] = new Neuron(Nets[i].Layers[x].Neurons[z], Nets[i].Layers[x].Neurons[z].Dentrites, previousLayer);
                    }

                    int mutateIndex = Random.Next(0, Nets[i].Layers[x].Neurons.Length);
                    Nets[i].Layers[x].Neurons[mutateIndex].Randomize(Random, RandomizeMin, RandomizeMax);
                }
            }

            for (int i = Nets.Length * 9 / 10; i < Nets.Length; i++)
            {
                Nets[i].Randomize(Random, RandomizeMin, RandomizeMax);
            }
        }

        public void RestartGame(GameTime gameTime)
        {
            Pipes.Clear();
            LastElapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds - 3000;
            GameObjects.Clear();
            for (int i = 0; i < Flappies.Length; i++)
            {
                Flappies[i].Pos = new Vector2(ScreenSize.X / 5 - Flappies[i].Size.X, ScreenSize.Y / 2 - Flappies[i].Size.Y);
                Flappies[i].IsVisible = true;
                Flappies[i].SurvivalTime = 0;
                GameObjects.Add(Flappies[i]);
            }
            timeToNextPipe = 4000;
            ClosestPipe = null;
            CurrentGeneration++;
        }
    }
}
