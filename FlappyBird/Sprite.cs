using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace FlappyBird
{
    public class Sprite : GameObject
    {
        public Texture2D Tex { get; set; }
        public Color Tint { get; set; }

        public Sprite(Texture2D tex, Color tint, Vector2 pos, Vector2 size, Vector2 scale, Vector2 origin)
            :base(pos, size, scale, origin)
        {
            Tex = tex;
            Tint = tint;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Tex, HitBox, Tint);
        }
    }
}
