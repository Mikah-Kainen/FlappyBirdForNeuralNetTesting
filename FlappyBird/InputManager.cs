using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlappyBird
{
    public static class InputManager
    {

        public static KeyboardState KeyboardState { get; private set; }
        public static MouseState MouseState { get; private set; }

        public static void Update(GameTime gameTime)
        {
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
        }
    }
}
