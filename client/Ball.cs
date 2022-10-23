using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    internal class Ball
    {
        public Vector2 Position = new Vector2(0, 0);
        public Vector2 DefaultPosition = new Vector2(0, 0);
        public int Radius = 5;
        public bool CanMove = false;

        // Movement
        private float MoveX = 0f;
        private float MoveY = 0f;
        private float Speed = 100f;

        Random random = new Random();

        // Constructors
        public Ball(GraphicsDevice graphicsDevice, GameTime gameTime)
        {
        }

        public Ball(Vector2 position, int radius, float speed)
        {
            Position = position;
            DefaultPosition = position;
            Radius = radius;
            Speed = speed;
        }

        // Default Position
        public Ball GetBackToDefaultPos()
        {
            Position = DefaultPosition;
            return this;
        }

        // Draw ball
        public Ball Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {

            spriteBatch.DrawCircle(
                Position,
                Radius,
                360,
                Color.White,
                Radius
            );
            return this;
        }

        // Start | Stop ball moving
        public Ball StartMoving()
        {
            /*MoveX = random.Next((int)-Speed, (int)Speed);
            MoveY = 1000f;*/
            MoveX = Speed;
            MoveY = Speed;
            CanMove = true;
            return this;
        }

        public Ball Move(GameTime gameTime, float screenHeight, float screenWidth, Player p1, Player p2)
        {
            if (CanMove)
            {
                /*// X
                if (Position.X < 0)
                {
                    MoveX -= Speed;
                }
                else
                {
                    MoveX += Speed;
                }
                // Y
                if (Position.Y < 0)
                {
                    MoveY -= Speed;
                }
                else
                {
                    MoveY += Speed;
                }*/

                Position.X += MoveX + Speed;
                Position.Y += MoveY + Speed;

                // Collisions
                if (Position.Y < Radius * 2 && MoveY < 0)
                {
                    MoveY = Math.Abs(MoveY);
                }
                if (Position.Y > screenHeight - Radius * 2 && MoveY > 0)
                {
                    MoveY = -MoveY;
                }

                if (
                    Position.X < p1.Position.X + p1.width && Position.X > p1.Position.X && Position.Y > p1.Position.Y && Position.Y < p1.Position.Y + p1.height ||
                    Position.X + Radius * 2 > p2.Position.X && Position.X + Radius * 2 < p2.Position.X + p2.width && Position.Y > p2.Position.Y && Position.Y < p2.Position.Y + p2.height
                   )
                {
                    MoveX = -MoveX;
                }

            }

            return this;
        }

        public Ball StopMoving()
        {
            CanMove = false;
            return this;
        }
    }
}
