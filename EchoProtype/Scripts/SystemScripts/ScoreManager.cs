using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoProtype
{
    public class Scoremanager
    {

        private SpriteBatch spriteBatch;
        private GameContent gameContent;
        private int screenWidth;
        private int screenHeight;
        public int flytime;
        private int currenttime;
        public bool flag = false;
        private int extraPoints;
        public Scoremanager(GameManager gameManager)
        {
            this.spriteBatch = gameManager.spriteBatch;
            this.gameContent = gameManager.gameContent;
            this.screenHeight = gameManager.screenHeight;
            this.screenWidth = gameManager.screenWidth;
        }
        public void Draw(GameTime gameTime)
        {

            string scoreMsg = "Score : ";
            if (!flag)
            {
                currenttime = gameTime.TotalGameTime.Seconds - flytime + extraPoints;
            }
            scoreMsg += currenttime;
            Vector2 stringSpace = gameContent.labelFont.MeasureString(scoreMsg);
            spriteBatch.DrawString(gameContent.labelFont, scoreMsg, new Vector2(60, 40), Color.White);
        }

        public void AddPoints(int points)
        {
            extraPoints += points;
        }

        public void SubtractPoints(int points)
        {
            extraPoints -= points;
        }

        public void resetScore(GameTime gameTime)
        {
            flytime = gameTime.TotalGameTime.Seconds;
            extraPoints = 0;
            flag = false;
        }
    }
}
