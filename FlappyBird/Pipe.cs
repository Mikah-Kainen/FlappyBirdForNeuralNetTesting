using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace FlappyBird
{
    public class Pipe : Sprite
    {
        public Vector2 SpaceBetweenPipes { get; set; }
        public Vector2 ScreenSize { get; set; }
        public Sprite OtherPart => new Sprite(Tex, Tint, new Vector2(Pos.X, 0), Vector2.One, new Vector2(Size.X * Scale.X, ScreenSize.Y - Size.Y * Scale.Y - SpaceBetweenPipes.Y), Vector2.Zero);

        public Pipe(int padding, Vector2 spaceBetweenPipes, Vector2 screenSize, Random random, GraphicsDevice graphicsDevice)
            : base(Color.Black.CreatePixel(graphicsDevice), Color.White, Vector2.One, Vector2.One, new Vector2(spaceBetweenPipes.X, random.Next(padding, (int)(screenSize.Y - padding - spaceBetweenPipes.Y))), Vector2.Zero)
        {
            SpaceBetweenPipes = spaceBetweenPipes;
            ScreenSize = screenSize;
            Pos = new Vector2(screenSize.X, screenSize.Y - Size.Y * Scale.Y);
        }

        public override void Update(GameTime gameTime)
        {
            Pos = new Vector2(Pos.X - 2, Pos.Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            OtherPart.Draw(spriteBatch);
        }
    }
}
