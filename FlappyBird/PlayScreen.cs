﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace FlappyBird
{
    public class PlayScreen : Screen
    {

        public  Texture2D WhitePixel;
        public GraphicsDevice GraphicsDevice;

        public PlayScreen(GraphicsDevice graphicsDevice)
            :base()
        {
            GraphicsDevice = graphicsDevice;
            whitePixel = Color.White.CreatePixel(GraphicsDevice);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
        }


    }
}
