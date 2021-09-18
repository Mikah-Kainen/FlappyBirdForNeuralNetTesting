using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlappyBird
{
    public class Bird : Sprite
    {

        private bool wasMousePressed;
        public double SurvivalTime { get; set; }
        public double ShouldJump { get; set; }

        public Bird(Texture2D tex, Color tint, Vector2 pos, Vector2 size, Vector2 scale, Vector2 origin)
            :base(tex, tint, pos, size, scale, origin)
        {
            wasMousePressed = false;
            SurvivalTime = 0;
            ShouldJump = 0;
        }

        public override void Update(GameTime gameTime)
        {
            //if (InputManager.MouseState.LeftButton == ButtonState.Pressed)
            //{
            //    wasMousePressed = true;
            //}
            //else if(wasMousePressed)
            //{
            //    if (Pos.Y - 100 > 0)
            //    {
            //        Pos = new Vector2(Pos.X, Pos.Y - 100);
            //    }
            //    else
            //    {
            //        Pos = new Vector2(Pos.X, 0);
            //    }
            //    wasMousePressed = false;
            //}
            if(ShouldJump > 0)
            {
                    if (Pos.Y - 100 > 0)
                    {
                        Pos = new Vector2(Pos.X, Pos.Y - 100);
                    }
                    else
                    {
                        Pos = new Vector2(Pos.X, 0);
                    }
            }
            Pos = new Vector2(Pos.X, Pos.Y + 2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
