using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace FlappyBird
{
    public class Screen
    {
        public List<GameObject> GameObjects;

        public Screen()
        {

        }

        public void Update(GameTime gameTime)
        {
            foreach(GameObject Object in GameObjects)
            {
                Object.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (GameObject Object in GameObjects)
            {
                Object.Draw(spriteBatch);
            }
        }
       
    }
}
