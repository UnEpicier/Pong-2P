using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using shared;
using System;
using System.Collections.Generic;

namespace client
{
    public class Game1 : Game
    {
        // Network
        EventBasedNetListener listener;
        NetManager client;
        NetPeer _server;
        NetPacketProcessor processor;

        /*
         None => Waiting for minimum 2 clients
         Idle => Preparaing the game, waiting for client 1 to start the game
         Playing => Game actually playing
         Paused => Game paused by a player
         Ended => Game Ended, waiting for client 1 decision (exiting or restart)
         Disconnected => One of the two players has been disconnected
         */
        string gameState = "None";
        /*
         1 => Player 1
         2 => Player 2
         3 => Spectator (default)
         */
        int controller = 2;

        // Game instances
        private Player pl1;
        private Player pl2;
        private Ball ball;
        private ScreenBounds sb1;
        private ScreenBounds sb2;
        private KillZone killZone1;
        private KillZone killZone2;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private CollisionComponent _collisionComponent;
        private readonly List<IEntity> _entities = new List<IEntity>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Dictionary<string, string> settings = Lib.ReadAllSettings();
            if (!settings.ContainsKey("SERVER_IP") || (settings.ContainsKey("SERVER_IP") && Lib.ReadSetting("SERVER_IP") == null))
            {
                Console.Error.WriteLine("Server IP missing in config file!");
            }
            else if (!settings.ContainsKey("PORT") || (settings.ContainsKey("PORT") && Lib.ReadSetting("PORT") == null))
            {
                Console.Error.WriteLine("Port missing in config file!");
            }
            else if (!settings.ContainsKey("PSWD") || (settings.ContainsKey("PSWD") && Lib.ReadSetting("PSWD") == null))
            {
                Console.Error.WriteLine("Server password missing in config file!");
            }

        }

        protected override void Initialize()
        {
            _collisionComponent = new CollisionComponent(new RectangleF(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
            // Network
            listener = new EventBasedNetListener();
            processor = new NetPacketProcessor();

            listener.PeerConnectedEvent += server =>
            {
                _server = server;
                Console.WriteLine($"Connected to {server}");
            };

            listener.NetworkReceiveEvent += (server, reader, deliveryMethod) =>
            {
                processor.ReadAllPackets(reader, server);
            };

            processor.SubscribeReusable<GameStateChange>(GameStateHandler);
            processor.SubscribeReusable<Assignation>(AssignationHandler);
            processor.SubscribeReusable<Position>(Positionhandler);

            client = new NetManager(listener);
            client.Start();
            client.Connect(Lib.ReadSetting("SERVER_IP"), int.Parse(Lib.ReadSetting("PORT")), Lib.ReadSetting("PSWD"));

            // Window
            Window.Title = "Pong";
            Window.AllowAltF4 = false;

            // PLAYERS
            pl1 = new Player(
                new RectangleF(new Point(15, _graphics.PreferredBackBufferHeight / 2 - 50), new Size2(15, 100)),
                _graphics.PreferredBackBufferHeight,
                processor
            );
            _entities.Add(pl1);

            pl2 = new Player(
                new RectangleF(new Point(_graphics.PreferredBackBufferWidth - 30, _graphics.PreferredBackBufferHeight / 2 - 50), new Size2(15, 100)),
                _graphics.PreferredBackBufferHeight,
                processor
            );
            _entities.Add(pl2);

            // BALL
            ball = new Ball(
                new CircleF(new Point(_graphics.PreferredBackBufferWidth / 2 - 10, _graphics.PreferredBackBufferHeight / 2 - 10), 10)
            );
            _entities.Add(ball);

            // SCREEN BOUNDS
            sb1 = new ScreenBounds(
                new RectangleF(new Point(0, -1), new Size2(_graphics.PreferredBackBufferWidth, 1))
            );
            _entities.Add(sb1);
            sb2 = new ScreenBounds(
                new RectangleF(new Point(0, _graphics.PreferredBackBufferHeight), new Size2(_graphics.PreferredBackBufferWidth, 1))
            );
            _entities.Add(sb2);

            // KILLZONE
            /*killZone1 = new KillZone(
                 new Vector2(0, 0),
                 15,
                 _graphics.PreferredBackBufferHeight
             );
             _entities.Add(killZone1);

             killZone2 = new KillZone(
                 new Vector2(_graphics.PreferredBackBufferWidth - 15, 0),
                 15,
                 _graphics.PreferredBackBufferHeight
             );
             killZone2.isLeft = false;
             _entities.Add(killZone2);*/


            foreach (IEntity actor in _entities)
            {
                _collisionComponent.Insert(actor);
            }

            base.Initialize();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            client.DisconnectPeer(_server);
            base.OnExiting(sender, args);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // Network
            client.PollEvents();

            pl1.UpdateStats(controller, gameState, _server);
            pl2.UpdateStats(controller, gameState, _server);
            foreach (IEntity entity in _entities)
            {
                entity.Update(gameTime);
            }

            _collisionComponent.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            foreach (IEntity entitiy in _entities)
            {
                entitiy.Draw(_spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        // Network
        private void AssignationHandler(Assignation assignation)
        {
            controller = assignation.controller;
            ball.Velocity = new Vector2(assignation.ballX, assignation.ballY);
        }

        private void Positionhandler(Position position)
        {
            if (!pl1.canMove && position.controller == 0)
            {
                pl1.setPos(new Vector2(position.x, position.y));
            }
            else if (!pl2.canMove && position.controller == 1)
            {
                pl2.setPos(new Vector2(position.x, position.y));
            }
        }

        private void GameStateHandler(GameStateChange change)
        {
            gameState = change.gameState;

            if (gameState == "Playing")
            {
                if (controller == 0)
                {
                    pl1.SetControllable(true);
                    pl2.SetControllable(false);
                }
                else if (controller == 1)
                {
                    pl1.SetControllable(false);
                    pl2.SetControllable(true);
                }

                ball.StartMoving();
            }
            else if (gameState == "Paused" || gameState == "Ended" || gameState == "Disconnected")
            {
                pl1.SetControllable(false);
                pl2.SetControllable(false);

                ball.StopMoving();
            }
        }
    }
}