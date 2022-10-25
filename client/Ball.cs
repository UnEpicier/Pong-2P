using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace client
{
    internal class Ball: IEntity
    {
        public Vector2 DefaultPosition;

        // Movements
        public Vector2 Velocity;
        public bool CanMove = false;
        public IShapeF Bounds { get; }

        // Constructors
        public Ball(CircleF circleF)
        {
            Bounds = circleF;
            DefaultPosition = Bounds.Position;
        }

        // Default Position
        public Ball GetBackToDefaultPos()
        {
            Bounds.Position = DefaultPosition;
            return this;
        }

        // Draw ball
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle((CircleF)Bounds, 10, Color.White, 3f);
        }

        // Start | Stop ball moving
        public void StartMoving()
        {
            CanMove = true;
        }

        public void Update(GameTime gameTime)
        {
            if (CanMove)
            {
                Bounds.Position += Velocity * gameTime.GetElapsedSeconds() * 50;
            }
        }

        public void StopMoving()
        {
            CanMove = false;
        }
        
        // Collisions
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            Bounds.Position -= collisionInfo.PenetrationVector;
        }
    }
}
