using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    internal class KillZone
    {
        private Vector2 Position;
        private int Width;
        private int Height;
        public bool isLeft = true;

        public bool Debug = false;

        public KillZone(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            Texture2D texture = new Texture2D(graphicsDevice, Width, Height);
            Color[] data = new Color[Width * Height];
            for (int i = 0; i < data.Length; ++i)
            {
                if (Debug)
                {
                    data[i] = Color.Red;
                }
                else
                {
                    data[i] = Color.Transparent;
                }
            }
            texture.SetData(data);
            spriteBatch.Draw(texture, Position, Color.White);
        }

        public void Collisions(Ball ball)
        {
            if (isLeft)
            {
                if (ball.Position.X < Position.X + Width && ball.CanMove)
                {
                    //ball.StopMoving();
                }
            }
            else
            {
                if (ball.Position.X > Position.X && ball.CanMove)
                {
                    //ball.StopMoving();
                }
            }
        }
    }
}
