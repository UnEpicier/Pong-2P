using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace client
{
    internal class ScreenBounds : IEntity
    {
        public IShapeF Bounds { get; }

        public ScreenBounds(RectangleF rectangleF)
        {
            Bounds = rectangleF;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.White, 3);
        }

        public void Update(GameTime gameTime) { }

        public void OnCollision(CollisionEventArgs collisionInfos) { }
    }
}
