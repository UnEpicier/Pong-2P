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
    internal class UI
    {
        public UI()
        {

        }

        public void Draw(SpriteBatch spriteBatch, float screenWidth, float screenHeight, SpriteFont spriteFont, int score1, int score2)
        {
            // Lines
            int divider = 21;
            for(int i = 0; i < screenHeight / divider; i++)
            {
                if (i % 2 == 0)
                {
                    spriteBatch.DrawLine(
                        new Vector2(screenWidth / 2, screenHeight / divider * i),
                        new Vector2(screenWidth / 2, screenHeight / divider * (i+1)),
                        Color.White
                    );
                }
            }

            // Scores
            int spacing = 25;
            spriteBatch.DrawString(spriteFont, $"{score1}", new Vector2(screenWidth / 2 - spriteFont.MeasureString($"{score1}").X - spacing, 5), Color.LightGray);
            spriteBatch.DrawString(spriteFont, $"{score2}", new Vector2(screenWidth / 2 + spacing, 5), Color.LightGray);
        }
    }
}
