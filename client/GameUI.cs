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
    internal class GameUI
    {
        private int score1;
        private int score2;

        public GameUI() { }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Size2 windowSize)
        {
            // Mid lines
            for(int i = 0; i < windowSize.Height / 21; i++)
            {
                if (i%2==0)
                {
                    spriteBatch.DrawLine(
                        new Vector2(windowSize.Width / 2, windowSize.Height / 21 * i),
                        new Vector2(windowSize.Width / 2, windowSize.Height / 21 * (i+1)),
                        Color.White
                    );
                }
            }

            // Scores
            int spacing = 25;
            spriteBatch.DrawString(font, $"{score1}", new Vector2(windowSize.Width / 2 - font.MeasureString($"{score1}").X - spacing, 5), Color.LightGray);
            spriteBatch.DrawString(font, $"{score2}", new Vector2(windowSize.Width / 2 + spacing, 5), Color.LightGray);
        }

        public void Update(int _score1, int _score2)
        {
            score1 = _score1;
            score2 = _score2;
        }
    }
}
