using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using shared;

namespace client
{
    internal class KillZone : IEntity
    {
        public IShapeF Bounds { get; }
        private NetPacketProcessor _processor;
        private NetPeer _server;
        private string _gameState;

        public KillZone(RectangleF rectangleF, NetPacketProcessor processor)
        {
            Bounds = rectangleF;
            _processor = processor;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Transparent);
        }

        public void Update(GameTime gameTime) { }

        public void OnCollision(CollisionEventArgs collisionInfos)
        {
            if (collisionInfos.Other.GetType().Equals(typeof(Ball)) && _gameState != "Ended")
            {
                GameStateChange packet = new() { gameState = "Ended" };
                _processor.Send(_server, packet, DeliveryMethod.ReliableOrdered);
            }
        }

        public void UpdateStat(NetPeer server, string gameState)
        {
            _server = server;
            _gameState = gameState;
        }
    }
}
