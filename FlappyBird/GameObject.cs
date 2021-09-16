using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace FlappyBird
{
    public abstract class GameObject
    {
        public Vector2 Pos { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Origin { get; set; }

        public Rectangle HitBox => new Rectangle((int)(Pos.X - Origin.X * Scale.X), (int)(Pos.Y - Origin.Y * Scale.Y), (int)(Size.X * Scale.X), (int)(Size.Y * Scale.Y));

        public object Tag { get; set; }
        public bool IsVisible { get; set; }

        public GameObject(Vector2 pos, Vector2 size, Vector2 scale, Vector2 origin)
        {
            Pos = pos;
            Size = size;
            Scale = scale;
            Origin = origin;

            IsVisible = true;
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
