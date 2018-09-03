using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EchoProtype
{
    public class EchoWave
    {
        // public references & variables
        public Player parent;
        public SpriteBatch spriteBatch;

        // private references & variables
        float startTime;
        float existTime = 1f;
        float moveSpeed = 800f;
        Vector2 scaleSpeed = new Vector2(3f, 10f);
        Vector2 startPos;
        Vector2 centerRecStart = new Vector2(20, 60);
        Vector2 topRecStart = new Vector2(20, 20);
        Vector2 bottomStart = new Vector2(20, 20);
        public List<Rectangle> collisionRectangles = new List<Rectangle>();

        // constructor
        public EchoWave(GameTime time, Vector2 pos)
        {
            var currentTime = (float)time.TotalGameTime.TotalMilliseconds / 1000;
            startTime = currentTime;
            startPos = pos;

            // calculate the rectangels
            var centerRec = new Rectangle();
            centerRec.Size = new Point((int)centerRecStart.X, (int)centerRecStart.Y);
            centerRec.X = (int)(startPos.X - centerRecStart.X / 2);
            centerRec.Y = (int)(startPos.Y - centerRecStart.Y / 2);
            collisionRectangles.Add(centerRec);

            // TODO: add other rectangles
            //var topRec = new Rectangle();
            //topRec.Size = new Point((int)topRecStart.X, (int)topRecStart.Y);
            //topRec.X = centerRec.X - (int)topRecStart.X;
            //topRec.Y = centerRec.Y - (int)topRecStart.Y;
        }

        // update functions
        public void Update(GameTime gt)
        {
            var currentTime = (float)gt.TotalGameTime.TotalMilliseconds / 1000;
            if(currentTime - startTime <= existTime)
            {
                var passedTime = currentTime - startTime;
                // update the recs here
                var centerRec = collisionRectangles[0];
                var currentSizeCenter = centerRecStart * new Vector2(1 + passedTime * scaleSpeed.X, 1 + passedTime * scaleSpeed.Y);
                centerRec.Size = new Point((int)currentSizeCenter.X, (int)currentSizeCenter.Y);
                var currentCenterX = startPos.X + moveSpeed * passedTime;
                centerRec.X = (int)currentCenterX - centerRec.Size.X / 2;
                centerRec.Y = (int)startPos.Y - centerRec.Size.Y / 2;

                collisionRectangles[0] = centerRec;
            }
            else
            {
                parent.removeList.Add(this);
                Console.WriteLine("EchoWave: remove");
            }
        }

        public void Draw()
        {
            // temp draw the rectangles
            if(spriteBatch != null)
            {
                spriteBatch.Begin();

                spriteBatch.Draw(GameContent.instance.imgEcho, collisionRectangles[0], Color.White);

                spriteBatch.End();
            }
        }
    }
}
