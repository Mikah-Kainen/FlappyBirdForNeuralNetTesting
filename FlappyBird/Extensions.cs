using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace FlappyBird
{
    public static class Extensions
    {

        public static Texture2D CreatePixel(this Color tint, GraphicsDevice graphicsDevice)
        {
            Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { tint});
            return pixel;
        }
    }
}
