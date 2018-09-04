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
        private int grayScale { get; set; }

        public float speed;
        public bool Destroyed { get; set; } //does brick still exist?
        public bool Visible { get; set; }
        public Rectangle hitBox;
        private Random rand;

        private int damage { get; set; }
        public bool sticky { get; set; }

        private GameManager gameManager;

        private Texture2D imgStag { get; set; }  //cached image of the brick
        private SpriteBatch spriteBatch;  //allows us to write on backbuffer when we need to draw self
        private Player player;

        public Stalagmite(float x, float y, float speed, GameManager gameManager, Random rand)
        {
            X = x;
            Y = y;
            this.speed = speed;
            damage = 1;
            sticky = false;
            player = gameManager.player;
            this.rand = rand;
            if (Y > -10 && Y < 400)
            {
                switch (rand.Next(0, 2))
                {
                    case 0:
                        {
                            imgStag = gameManager.gameContent.imgfloatingRock;
                            break;
                        }

                    case 1:
                        {
                            imgStag = gameManager.gameContent.imgfloatingTangles;
                            sticky = true;
                            break;
                        }
                }
            }
            else if (Y <= -10)
            {
                switch (rand.Next(0,2))
                {
                    case 0:
                        {
                            imgStag = gameManager.gameContent.imgStalactite1;
                            break;
                        }

                    case 1:
                        {
                            imgStag = gameManager.gameContent.imgStalactite2;
                            break;
                        }
                }
            }
            else
            {
                switch (rand.Next(0, 2))
                {
                    case 0:
                        {
                            imgStag = gameManager.gameContent.imgStalagmite1;
                            break;
                        }

                    case 1:
                        {
                            imgStag = gameManager.gameContent.imgStalagmite2;
                            break;
                        }
                }
            }

            Width = imgStag.Width;
            Height = imgStag.Height;

            dmgDelayTime = 750;
            damageTimer = 0;
            visionDelayTime = 1500;
            visionTimer = 0;
            this.spriteBatch = gameManager.spriteBatch;
            hitBox = new Rectangle((int)X, (int)Y, (int)(Width), (int)(Height));// Rectangle for the wall collider
            Destroyed = true;
            Visible = false;
            grayScale = 255;
            this.gameManager = gameManager;
        }

        public void Draw()
        {
            if (!Destroyed && Visible)
            {
                var newColor = new Color(grayScale, grayScale, grayScale, 255);


                spriteBatch.Draw(imgStag, new Vector2(X, Y), null, newColor, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
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
                    if (!sticky)
                    {
                        player.Health -= damage;
                        player.hurt = true;
                        gameManager.soundEffects[1].CreateInstance().Play();
                    }
                    else
                    {
                        var hitSound = GameContent.instance.slowSound.CreateInstance();
                        hitSound.Volume = 1f;
                        hitSound.Play();
                    }
                        damageTimer = (float)gameTime.TotalGameTime.TotalMilliseconds;
                        player.canTakeDamage = false;                                        
                }

                if(sticky)
                {
                    player.pushBack(11);
                }
                else
                {
                    player.pushBack(7);
                }
            }

            for (int i = 0; i < player.echoWaves.Count; i++)
            {
                for (int j = 0; j < player.echoWaves[i].collisionRectangles.Count; j++)
                    if (!Destroyed && HitTest(player.echoWaves[i].collisionRectangles[j], hitBox))
                    {
                        visionTimer = (float)gameTime.TotalGameTime.TotalMilliseconds;
                        Visible = true;
                        var hitSound = GameContent.instance.echoCast.CreateInstance();
                        hitSound.Volume = 0.01f;
                        hitSound.Play();
                    }
            }

            VisionUpdate(gameTime);
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

        private void VisionUpdate(GameTime gameTime)
        {
            if (Visible)
            {
                grayScale = 255 - (int)(((float)gameTime.TotalGameTime.TotalMilliseconds - visionTimer) / 1000 * 255);
                if (gameTime.TotalGameTime.TotalMilliseconds >= visionTimer + visionDelayTime)
                {
                    Visible = false;
                    grayScale = 255;
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
            if (Y > -10 && Y < 400)
            {
                switch (rand.Next(0, 2))
                {
                    case 0:
                        {
                            imgStag = gameManager.gameContent.imgfloatingRock;
                            break;
                        }

                    case 1:
                        {
                            imgStag = gameManager.gameContent.imgfloatingTangles;
                            sticky = true;
                            break;
                        }
                }

            }
            else if (Y <= -10)
            {
                switch (rand.Next(0, 2))
                {
                    case 0:
                        {
                            imgStag = gameManager.gameContent.imgStalactite1;
                            break;
                        }

                    case 1:
                        {
                            imgStag = gameManager.gameContent.imgStalactite2;
                            break;
                        }
                }
            }
            else
            {
                switch (rand.Next(0, 2))
                {
                    case 0:
                        {
                            imgStag = gameManager.gameContent.imgStalagmite1;
                            break;
                        }

                    case 1:
                        {
                            imgStag = gameManager.gameContent.imgStalagmite2;
                            break;
                        }
                }
            }
        }
    }
}


