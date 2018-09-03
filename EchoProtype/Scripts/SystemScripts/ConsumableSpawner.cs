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
    class ConsumableSpawner
    {
        private int maxX;
        private int minX;
        private int maxY;
        private int minY;
        private int maxTime;
        private int minTime;
        private int counter;
        private int speed;
        private float spawnTimer;
        private float deltaTime;
        private GameManager gameManager;
        public Consumable[] consumables;
        private Random rand;

        public ConsumableSpawner(int totalNumObs, int maxX, int minX, int maxY, int minY, int maxTime, int minTime,int speed, GameManager gameManager)
        {
            this.maxTime = maxTime;
            this.minTime = minTime;
            this.maxX = maxX;
            this.minX = minX;
            this.minY = minY;
            this.maxY = maxY;
            this.gameManager = gameManager;
            consumables = new Consumable[totalNumObs];
            rand = new Random();
            this.speed = speed;
            spawnTimer = 0;
            deltaTime = rand.Next(minTime, maxTime);
            for (int i = 0; i < consumables.Length; i++)
            {
                float X = rand.Next(minX, maxX);
                float Y = rand.Next(minY, maxY);

                consumables[i] = new Consumable((int)X, (int)Y, speed, RandomType(), gameManager);
            }
        }
        public void Draw()
        {
            for (int i = 0; i < consumables.Length; i++)
            {
                consumables[i].Draw();
            }
        }

        public void CleanUp()
        {
            for (int i = 0; i < consumables.Length; i++)
            {
                if (consumables[i].X < -50)
                {
                    consumables[i].Destroyed = true;
                }
            }

        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < consumables.Length; i++)
            {
                consumables[i].Update(gameTime);
            }

            Spawn(gameTime);
            CleanUp();
        }
        public void Spawn(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds >= (spawnTimer + deltaTime) && counter < consumables.Length && gameManager.gameStart)
            {
                consumables[counter].Destroyed = false;
                counter++;
                deltaTime = rand.Next(minTime, maxTime);
                spawnTimer = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }

            if (counter == consumables.Length & consumables[consumables.Length - 1].Destroyed == true)
            {
                for (int i = 0; i < consumables.Length; i++)
                {
                    consumables[i].X = rand.Next(minX, maxX);
                    consumables[i].Y = rand.Next(minY, maxY);
                }
                //speed += 1;
                counter = 0;
            }
        }
        
        private Consumable.Type RandomType()
        {
            int choice = rand.Next(0,20);
            if(choice >= 18)
            {
                return Consumable.Type.Health;
            }
            else
            {
                return Consumable.Type.AddPoints;
            }
        }

        public void reset()
        {
            for (int i = 0; i < consumables.Length; i++)
            {
                float X = rand.Next(minX, maxX);
                float Y = rand.Next(minY, maxY);
                consumables[i] = new Consumable((int)X, (int)Y, speed, RandomType(), gameManager);
            }
            counter = 0;
        }
    }
}
