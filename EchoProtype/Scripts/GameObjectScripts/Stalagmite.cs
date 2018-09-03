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
    class Stalagmite
    {

        public float X { get; set; } //x position of brick on screen
        public float Y { get; set; } //y position of brick on screen
        public float Width { get; set; } //width of brick
        public float Height { get; set; } //height of brick

        private float damageTimer { get; set; }
        private float dmgDelayTime { get; set; }
        private float visionTimer { get; set; }
        private float visionDelayTime { get; set; }

        public float speed;
        public bool Destroyed { get; set; } //does brick still exist?
        public bool Visible { get; set; }
        public Rectangle hitBox;

        public int damage { get; set; }

        private GameManager gameManager;

        private Texture2D imgStag { get; set; }  //cached image of the brick
        private SpriteBatch spriteBatch;  //allows us to write on backbuffer when we need to draw self
        private Player player;

        public Stalagmite(float x, float y, float speed, GameManager gameManager)
        {
            X = x;
            Y = y;
            this.speed = speed;
            damage = 1;
            player = gameManager.player;

            if (Y > -10 && Y < 450)
            {
                imgStag = gameManager.gameContent.imgfloatingRock;
            }
            else if (Y <= -10)
            {
                int choice = new Random().Next(0, 2);
                if (choice == 0)
                {
                    imgStag = gameManager.gameContent.imgStalactite1;
                }
                else
                {
                    imgStag = gameManager.gameContent.imgStalactite2;
                }
            }
            else
            {
                int choice = new Random().Next(0, 2);
                if (choice == 0)
                {
                    imgStag = gameManager.gameContent.imgStalagmite1;
                }
                else
                {
                    imgStag = gameManager.gameContent.imgStalagmite2;
                }
            }

                Width = imgStag.Width;
            Height = imgStag.Height;

            dmgDelayTime = 750;
            damageTimer = 0;
            visionDelayTime = 1000;
            visionTimer = 0;
            this.spriteBatch = gameManager.spriteBatch;
            hitBox = new Rectangle((int)X, (int)Y, (int)(Width), (int)(Height));// Rectangle for the wall collider
            Destroyed = true;
            Visible = false;
            this.gameManager = gameManager;
        }

        public void Draw()
        {
            if (!Destroyed && Visible)
            {
                spriteBatch.Draw(imgStag, new Vector2(X, Y), null, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            }
        }

        public void Update(GameTime gameTime)
        {
            hitBox = new Rectangle((int)X, (int)Y, (int)(Width), (int)(Height));
            Move();       

            KeyboardState newKeyboardState = Keyboard.GetState();

            if (damageTimer > 0 && gameTime.TotalGameTime.TotalMilliseconds >= (damageTimer + dmgDelayTime))
            {
                player.hurt = false;
                damageTimer = 0;
                player.canTakeDamage = true;
            }

            //checks for collisions  against the player        
            if (!Destroyed && HitTest(player.playerRect, hitBox))
            {
                //makes player take damage
                if (player.canTakeDamage)
                {
                    player.Health -= damage;
                    damageTimer = (float)gameTime.TotalGameTime.TotalMilliseconds;
                    player.canTakeDamage = false;
                    player.hurt = true;
                    gameManager.soundEffects[1].CreateInstance().Play();
                }

                // collision pushes player in the direction opposite of their movement.
                if (newKeyboardState.IsKeyDown(Keys.Left))
                {
                    player.MoveRight();
                    player.MoveRight();
                    player.MoveRight();
                }
                if (newKeyboardState.IsKeyDown(Keys.Up))
                {
                    player.MoveLeft();
                    player.MoveLeft();
                    player.MoveLeft();
                    player.MoveDown();
                }
                if (newKeyboardState.IsKeyDown(Keys.Down))
                {
                    player.MoveLeft();
                    player.MoveLeft();
                    player.MoveLeft();
                    player.MoveUp();
                }
                else
                {
                    player.MoveLeft();
                    player.MoveLeft();
                    player.MoveLeft();
                }
            }

            for (int i = 0; i < player.echoWaves.Count; i++)
            {
                for (int j = 0; j < player.echoWaves[i].collisionRectangles.Count; j++)
                    if (!Destroyed && HitTest(player.echoWaves[i].collisionRectangles[j], hitBox))
                    {
                        visionTimer = (float)gameTime.TotalGameTime.TotalMilliseconds;
                        Visible = true;
                    }
            }

            VisionCheck(gameTime);
        }

        private bool HitTest(Rectangle r1, Rectangle r2)
        {
            if (Rectangle.Intersect(r1, r2) != Rectangle.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void VisionCheck(GameTime gameTime)
        {
            if (Visible)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds >= visionTimer + visionDelayTime)
                {
                    Visible = false;
                }
            }
        }

        private void Move()
        {
            if (!Destroyed)
            {
                X -= speed;
            }
        }
            public void assignImage()
            {
            if (Y > -10 && Y < 450)
            {
                imgStag = gameManager.gameContent.imgfloatingRock;
            }
            else if (Y <= -10)
            {
                int choice = new Random().Next(0, 2);
                if (choice == 0)
                {
                    imgStag = gameManager.gameContent.imgStalactite1;
                }
                else
                {
                    imgStag = gameManager.gameContent.imgStalactite2;
                }
            }
            else
            {
                int choice = new Random().Next(0, 2);
                if (choice == 0)
                {
                    imgStag = gameManager.gameContent.imgStalagmite1;
                }
                else
                {
                    imgStag = gameManager.gameContent.imgStalagmite2;
                }
            }
        }
    }
}


