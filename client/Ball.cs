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
        public Vector2 DefaultPosition = new Vector2(0,0);
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
            this.Position = position;
            this.DefaultPosition = position;
            this.Radius = radius;
            this.Speed = speed;
        }

        // Default Position
        public Ball GetBackToDefaultPos()
        {
            this.Position = this.DefaultPosition;
            return this;
        }

        // Draw ball
        public Ball Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {

            spriteBatch.DrawCircle(
                this.Position,
                this.Radius,
                360,
                Color.White,
                this.Radius
            ) ;
            return this;
        }

        // Start | Stop ball moving
        public Ball StartMoving()
        {
            this.MoveX = random.Next((int)-this.Speed, (int)this.Speed);
            this.MoveY = random.Next((int)-this.Speed, (int)this.Speed);
            this.CanMove = true;
            return this;
        }

        public Ball Move(GameTime gameTime, float screenHeight, float screenWidth, Player p1, Player p2)
        {
            if (this.CanMove)
            {
                // X
                if (this.Position.X < 0)
                {
                    this.MoveX -= this.Speed;
                }
                else
                {
                    this.MoveX += this.Speed;
                }
                // Y
                if (this.Position.Y < 0)
                {
                    this.MoveY -= this.Speed;
                }
                else
                {
                    this.MoveY += this.Speed;
                }

                this.Position.X += this.MoveX;
                this.Position.Y += this.MoveY;

                // Collisions
                if (this.Position.Y < this.Radius*2 && this.MoveY < 0)
                {
                    this.MoveY = Math.Abs(this.MoveY);
                }
                if (this.Position.Y > screenHeight - this.Radius*2 && this.MoveY > 0)
                {
                    this.MoveY = -this.MoveY;
                }

                if (
                    (this.Position.X < p1.Position.X + p1.width && this.Position.X > p1.Position.X && this.Position.Y > p1.Position.Y && this.Position.Y < p1.Position.Y + p1.height) ||
                    (this.Position.X+ this.Radius*2 > p2.Position.X && this.Position.X + this.Radius*2 < p2.Position.X + p2.width && this.Position.Y > p2.Position.Y && this.Position.Y < p2.Position.Y + p2.height)
                   )
                {
                    this.MoveX = -this.MoveX;
                }

            }

            return this;
        }

        public Ball StopMoving()
        {
            this.CanMove = false;
            return this;
        }
    }
}
