using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    internal class Player
    {
        public Vector2 Position { get; set; }
        private Vector2 DefaultPosition { get; set; }
        public int width;
        public int height;
        public float speed;
        public bool canMove = false;

        // Constructors
        public Player()
        {
            this.Position = new Vector2(0, 0);
            this.DefaultPosition = new Vector2(0, 0);
            this.width = 20;
            this.height = 100;
            this.speed = 100f;
        }

        public Player(Vector2 position, int width, int height, float speed)
        {
            this.Position = position;
            this.DefaultPosition = position;
            this.width = width;
            this.height = height;
            this.speed = speed;
        }

        // Update the player's position
        public Player setPos(Vector2 pos)
        {
            this.Position = pos;
            return this;
        }

        // Set | Get default player position
        public Player setDefaultPos(Vector2 pos)
        {
            this.DefaultPosition = pos;
            return this;
        }

        public Vector2 getDefaultPos()
        {
            return this.DefaultPosition;
        }

        public Player BackToDefaultPos()
        {
            this.Position = this.DefaultPosition;
            return this;
        }

        // Set controllable by input
        public Player SetControllable(bool state)
        {
            this.canMove = state;
            return this;
        }

        // Draw player on screen
        public Player DrawPlayer(SpriteBatch spriteBatch, GraphicsDevice _graphicsDevice)
        {
            Texture2D texture = new Texture2D(_graphicsDevice, this.width, this.height);
            Color[] data = new Color[this.width * this.height];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.White;
            }
            texture.SetData(data);

            spriteBatch.Draw(texture, this.Position, Color.White);
            return this;
        }

        // Inputs controls
        public Player InputsControls(GameTime gameTime, float screenHeight)
        {
            if (this.canMove)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    Vector2 asked = new Vector2(this.Position.X, this.Position.Y - this.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (asked.Y <= 0)
                    {
                        asked.Y = 0f;
                    }
                    this.Position = asked;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    Vector2 asked = new Vector2(this.Position.X, this.Position.Y + this.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (asked.Y >= screenHeight - this.height)
                    {
                        asked.Y = screenHeight - this.height;
                    }
                    this.Position = asked;
                }
            }
            return this;
        }
    }
}
