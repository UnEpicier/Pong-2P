using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using shared;

namespace client
{
    internal class Player : IEntity
    {

        public Vector2 Velocity;
        public IShapeF Bounds { get; }
        public float _screenHeight;

        public bool canMove = false;
        private Vector2 DefaultPosition { get; set; }

        // Network
        NetPeer _server;
        NetPacketProcessor _processor;
        int _controller = 3;
        string _gameState = "None";

        // Constructors
        public Player(RectangleF rectangleF, float screenHeight, NetPacketProcessor processor)
        {
            Bounds = rectangleF;
            _screenHeight = screenHeight;
            DefaultPosition = Bounds.Position;
            _processor = processor;

            Velocity = new Vector2(0, 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.White, 3);
        }

        // Inputs controls
        public void Update(GameTime gameTime)
        {
            if (canMove)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        float asked = Bounds.Position.Y - Velocity.Y * gameTime.GetElapsedSeconds() * 50;
                        if (asked <= 0f)
                        {
                            asked = 0f;
                        }
                        Bounds.Position = new Vector2(Bounds.Position.X, asked);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        float asked = Bounds.Position.Y + Velocity.Y * gameTime.GetElapsedSeconds() * 50;
                        if (asked >= _screenHeight)
                        {
                            asked = _screenHeight;
                        }
                        Bounds.Position = new Vector2(Bounds.Position.X, asked);
                    }

                    Position packet = new() { controller = _controller, x = Bounds.Position.X, y = Bounds.Position.Y };
                    _processor.Send(_server, packet, DeliveryMethod.ReliableOrdered);
                }
            }

            // START / RESTART
            if (_controller == 0 && (_gameState == "Idle" || _gameState == "Ended") && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                GameStateChange packet = new() { gameState = "Playing" };
                _processor.Send(_server, packet, DeliveryMethod.ReliableOrdered);
            }

            // PAUSE SWITCH
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && _controller == 0)
            {
                if (_gameState == "Playing")
                {
                    GameStateChange packet = new() { gameState = "Paused" };
                    _processor.Send(_server, packet, DeliveryMethod.ReliableOrdered);
                }
                else if (_gameState == "Paused")
                {
                    GameStateChange packet = new() { gameState = "Playing" };
                    _processor.Send(_server, packet, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        public void OnCollision(CollisionEventArgs collisionsInfos) { }

        public void setPos(Vector2 pos)
        {
            Bounds.Position = pos;
        }

        public void BackToDefaultPos()
        {
            Bounds.Position = DefaultPosition;
        }

        public void SetControllable(bool state)
        {
            canMove = state;
        }

        public void UpdateStats(int controller, string gameState, NetPeer server)
        {
            _controller = controller;
            _gameState = gameState;
            _server = server;
        }
    }
}
 