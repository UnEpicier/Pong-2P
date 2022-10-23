using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using shared;

namespace client
{
    public class Game1 : Game
    {
        // Network
        EventBasedNetListener listener = new();
        NetManager client;
        NetPeer server;
        NetPacketProcessor processor = new();

        // Game instances
        private Player pl1;
        private Player pl2;
        private Ball ball;
        private KillZone killZone1;
        private KillZone killZone2;

        private UI UI;
        private SpriteFont spriteFont;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Dictionary<string, string> settings = Lib.ReadAllSettings();
            if (!settings.ContainsKey("SERVER_IP") || (settings.ContainsKey("SERVER_IP") && Lib.ReadSetting("SERVER_IP") == null))
            {
                Console.Error.WriteLine("Server IP missing in config file!");
            } else if (!settings.ContainsKey("PORT") || (settings.ContainsKey("PORT") && Lib.ReadSetting("PORT") == null))
            {
                Console.Error.WriteLine("Port missing in config file!");
            } else if (!settings.ContainsKey("PSWD") || (settings.ContainsKey("PSWD") && Lib.ReadSetting("PSWD") == null))
            {
                Console.Error.WriteLine("Server password missing in config file!");
            }
        }

        protected override void Initialize()
        {
            // Network
            client = new NetManager(listener);
            client.Start();
            server = client.Connect(Lib.ReadSetting("SERVER_IP"), int.Parse(Lib.ReadSetting("PORT")), Lib.ReadSetting("PSWD"));
            if (server == null)
            {
                // TODO: Replace by a error message :)
                Exit();
            }
            processor.SubscribeReusable<Assignation>(AssignationHandler);

            // Window
            Window.Title = "Pong";

            // UI
            UI = new UI();

            // PLAYERS
            pl1 = new Player(
                new Vector2(15, _graphics.PreferredBackBufferHeight / 2 - 50),
                15,
                100,
                100f
            );

            pl2 = new Player(
                new Vector2(_graphics.PreferredBackBufferWidth - 30, _graphics.PreferredBackBufferHeight / 2 - 50),
                15,
                100,
                100f
            );

            // BALL
            ball = new Ball(
                new Vector2(_graphics.PreferredBackBufferWidth / 2 - 5, _graphics.PreferredBackBufferHeight / 2 - 5),
                10,
                1f
            );

            // KILLZONE
            killZone1 = new KillZone(
                new Vector2(0, 0),
                15,
                _graphics.PreferredBackBufferHeight
            );

            killZone2 = new KillZone(
                new Vector2(_graphics.PreferredBackBufferWidth - 15, 0),
                15,
                _graphics.PreferredBackBufferHeight
            );
            killZone2.isLeft = false;


            // TEST
            ball.StartMoving();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("scoreFont");
        }

        protected override void Update(GameTime gameTime)
        {
            pl1.InputsControls(gameTime, _graphics.PreferredBackBufferHeight);

            ball.Move(gameTime, _graphics.PreferredBackBufferHeight, _graphics.PreferredBackBufferWidth, pl1, pl2);

            killZone1.Collisions(ball);
            killZone2.Collisions(ball);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            UI.Draw(_spriteBatch, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, spriteFont, 0, 0);

            pl1.DrawPlayer(_spriteBatch, GraphicsDevice);
            pl2.DrawPlayer(_spriteBatch, GraphicsDevice);

            ball.Draw(_spriteBatch, GraphicsDevice);

            killZone1.Draw(_spriteBatch, GraphicsDevice);
            killZone2.Draw(_spriteBatch, GraphicsDevice);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void AssignationHandler(Assignation assignation)
        {
            if (assignation != null)
            {
                if (assignation.controller == 0) {
                    pl1.SetControllable(true);
                    pl2.SetControllable(false);
                }
                else
                {
                    pl1.SetControllable(false);
                    pl2.SetControllable(true);
                }
            }
            else
            {
                Console.Error.WriteLine("Packet malformed!");
            }
        }
    }
}