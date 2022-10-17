using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    internal class Ball
    {
        private Vector2 Position { get; set; }
        private int Radius { get; set; }
        private float InitialSpeed { get; set; }
        private float Acceleration { get; set; }
        private int Delta { get; set; } // Delta time = milliseconds

        // Constructors
        public Ball(GraphicsDevice graphicsDevice)
        {
            this.Position = new Vector2(0,0);
            this.Radius = 5;
            this.InitialSpeed = 0.5f;
            this.Acceleration = 0.1f;
            this.Delta = 100;
        }

        public Ball(Vector2 position, int radius, float initialSpeed, float acceleration, int delta)
        {
            this.Position = position;
            this.Radius = radius;
            this.InitialSpeed = initialSpeed;
            this.Acceleration = acceleration;
            this.Delta = delta;
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
            return this;
        }

        public Ball StopMoving()
        {
            return this;
        }
    }
}
