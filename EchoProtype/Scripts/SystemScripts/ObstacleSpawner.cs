using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace EchoProtype
{
    class ObstacleSpawner
    {
        private int maxX;
        private int minX;
        private int maxY;
        private int minY;
        private int maxTime;
        private int minTime;
        private int speed;
        private int orgSpeed;
        private int counter;
        private float spawnTimer;
        private float deltaTime;
        private GameManager gameManager;
        public Stalagmite[] obstacles;
        private Random rand;

        public ObstacleSpawner(int totalNumObs, int maxX,int minX,int maxY, int minY,int maxTime,int minTime, int speed,GameManager gameManager)
        {
            this.maxTime = maxTime;
            this.minTime = minTime;
            this.maxX = maxX;
            this.minX = minX;
            this.minY = minY;
            this.maxY = maxY;
            this.speed = speed;
            orgSpeed = speed;
            this.gameManager = gameManager;
            obstacles = new Stalagmite[totalNumObs];
            rand = new Random();
            spawnTimer = 0;
            deltaTime = rand.Next(minTime, maxTime);
            for (int i = 0; i < obstacles.Length; i++)
            {
                float X = rand.Next(minX, maxX);
                float Y = rand.Next(minY, maxY);

                obstacles[i] = new Stalagmite((float)X,(float)Y,speed,gameManager, rand);
            }
        }
        public void Draw()
        {          
            for (int i = 0; i < obstacles.Length; i++)
            {
                    obstacles[i].Draw();
            }           
        }             

        public void CleanUp()
        {
            for (int i = 0; i < obstacles.Length; i++)
            {
                if (obstacles[i].X < -50)
                {
                    obstacles[i].Destroyed = true;
                    obstacles[i].sticky = false;
                }
            }

        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < obstacles.Length; i++)
            {
                obstacles[i].Update(gameTime);
            }

            Spawn(gameTime);
            CleanUp();           
        }

        public void Spawn(GameTime gameTime)
        {           
            if (gameTime.TotalGameTime.TotalMilliseconds >= (spawnTimer + deltaTime) && counter < obstacles.Length && gameManager.gameStart)
            {
                obstacles[counter].Destroyed = false;
                counter++;
                deltaTime = rand.Next(minTime, maxTime);               
                spawnTimer = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }

            if (counter == obstacles.Length & obstacles[obstacles.Length - 1].Destroyed == true)
            {
                for (int i = 0; i < obstacles.Length; i++)
                {
                    obstacles[i].X = rand.Next(minX, maxX);
                    obstacles[i].Y = rand.Next(minY, maxY);
                    obstacles[i].assignImage();
                }
                speed += 1;
                counter = 0;
            }
        }

        public void reset()
        {
            for (int i = 0; i < obstacles.Length; i++)
            {
                float X = rand.Next(minX, maxX);
                float Y = rand.Next(minY, maxY);

                obstacles[i] = new Stalagmite((float)X, (float)Y, orgSpeed, gameManager, rand);
            }
            counter = 0;
        }
    }
}
