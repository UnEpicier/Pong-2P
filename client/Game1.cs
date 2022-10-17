using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace client
{
    public class Game1 : Game
    {
        private Player pl1;
        private Player pl2;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            /*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();*/
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            pl1.DrawPlayer(_spriteBatch, GraphicsDevice);
            pl2.DrawPlayer(_spriteBatch, GraphicsDevice);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}