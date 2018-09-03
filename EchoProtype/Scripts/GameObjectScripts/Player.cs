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
    public class Player
    {
        public float X { get; set; } //x position of paddle on screen
        public float Y { get; set; } //y position of paddle on screen
        public float Width { get; set; } //width of paddle
        public float Height { get; set; } //height of paddle
        public float ScreenWidth { get; set; } //width of game screen
        public float ScreenHeight { get; set; } 

        public int Health { get; set; }//health
        public Rectangle playerRect { get; set; }
        public bool Destroyed { get; set; }     

        private List<Texture2D> _batImages { get; set; }
        private int _currentBatIndex { get; set; }
        private Point _batImageSize { get; set; }
        private Texture2D _sightBlocker { get; set; }
        private Point _sightImageSize { get; set; }

        private Texture2D _heartpic { get; set; }

        private Point _heartImageSize { get; set; }

        private float _imageChangeSpan = 0.1f;
        private float _lastChangeTime { get; set; }

        private float _rotationAngle { get; set; }
        private GameManager gameManager { get; set; }

        private SpriteBatch spriteBatch;  //allows us to write on backbuffer when we need to draw self
        public bool canTakeDamage = true;

        private Color batColor { get; set; }//change

        public bool hurt;

        Stack<Rectangle> locations = new Stack<Rectangle>();
        public float echoCoolDown = 1f;
        public float lastEchoTime { get; set; }
        public List<EchoWave> echoWaves = new List<EchoWave>();
        public List<EchoWave> removeList = new List<EchoWave>();

        public Player(float x, float y, GameManager gameManager)
        {
            //:'(
            this.gameManager = gameManager;
            Destroyed = false;
            hurt = false;
            X = x;
            Y = y;
            Width = 20;//change
            Height = 15;
            Health = 5;


            _batImages = gameManager.gameContent.batList;
            _currentBatIndex = 0;
            _batImageSize = new Point(_batImages[0].Width, _batImages[0].Height);

            _sightBlocker = gameManager.gameContent.blacksmall;
            _sightImageSize = new Point(_sightBlocker.Width, _sightBlocker.Height);

            _heartpic = gameManager.gameContent.redheart;
            _heartImageSize = new Point(_heartpic.Width, _heartpic.Height);

            _lastChangeTime = 0;

            _rotationAngle = 0;

            lastEchoTime = 0;

            this.spriteBatch = gameManager.spriteBatch;
            ScreenWidth = gameManager.screenWidth;
            ScreenHeight = gameManager.screenHeight;
        }

        public void AddLocations(int n)
        {
            Rectangle currentrec = locations.Pop();
            if (Health == 0)
            {
                return;
            }
            locations.Push(currentrec);
            for (int i = 1; i < n; i++)
            {
                locations.Push(new Rectangle(currentrec.X + 20 * i, currentrec.Y, currentrec.Width, currentrec.Height));
            }
        }

        public void Update(GameTime gameTime)
        {
            playerRect = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);//keeps track of player position

            var currentTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            currentTime /= 1000;

            if (Health <= 0 || X < -50)
            {
                Destroyed = true;
                gameManager.scoreManager.flag = true;
                gameManager.gameOver = true;
            }

            if (currentTime - _lastChangeTime > _imageChangeSpan)
            {
                _currentBatIndex = (_currentBatIndex + 1) % 4;

                _lastChangeTime = currentTime;
            }

            // update all echoWaves
            removeList.Clear();
            foreach (var e in echoWaves)
            {
                e.Update(gameTime);
            }
            foreach (var e in removeList)
            {
                echoWaves.Remove(e);
            }

            KeyboardState newKeyboardState = Keyboard.GetState();

            //process keyboard events                           
            if (newKeyboardState.IsKeyDown(Keys.Left))
            {
                MoveLeft();
            }
            if (newKeyboardState.IsKeyDown(Keys.Right))
            {
                MoveRight();
            }

            if (newKeyboardState.IsKeyDown(Keys.Up))
            {
                MoveUp();
            }
            if (newKeyboardState.IsKeyDown(Keys.Down))
            {
                MoveDown();
            }

            // echowave control
            if (newKeyboardState.IsKeyDown(Keys.Space))
            {
                if (gameManager.gameStart && !gameManager.gameOver)
                {
                    CreateEchoWave(gameTime);
                }
            }
        }

        public void Draw()
        {         
            if (!Destroyed)
            {
                var batDestinationRec = new Rectangle();
                var batSize = new Point(100, 100);

                batDestinationRec.X = (int)this.X;
                batDestinationRec.Y = (int)this.Y;
                batDestinationRec.Size = batSize;

                var sightOffset = new Point(-10, -30);
                var sightSize = new Point(5000, 10000);

                //var sightDestinationRec = new Rectangle();
                //sightDestinationRec.X = (int)this.X;
               // sightDestinationRec.Y = (int)this.Y;

                //sightDestinationRec.Size = sightSize;

                var heartDestinationRec = new Rectangle();
                var heartSize = new Point(100, 100);
                heartDestinationRec.X = (int)batDestinationRec.X;
                heartDestinationRec.Y = (int)batDestinationRec.Y - 80;
                heartDestinationRec.Width = 20;
                heartDestinationRec.Height = 20;
                locations.Push(heartDestinationRec);
                this.AddLocations(this.Health);

                //spriteBatch.Begin();

                //spriteBatch.Draw(_sightBlocker,
                //  sightDestinationRec,
                //null,
                //batColor,
                //_rotationAngle,
                //new Vector2(_sightImageSize.X / 2 + sightOffset.X, _sightImageSize.Y / 2 + sightOffset.Y),
                //SpriteEffects.None,
                //0f
                //);

                //spriteBatch.End();

                foreach (var e in echoWaves)
                {
                    e.Draw();
                }

                if (_currentBatIndex == 1)
                {
                    gameManager.soundEffects[0].CreateInstance().Play();
                }

                spriteBatch.Begin();
                if (!hurt)
                {
                    batColor = Color.White;
                }
                else
                {
                    batColor = Color.Red;
                }

                spriteBatch.Draw(_batImages[_currentBatIndex],
                    batDestinationRec,
                    null,
                    batColor,
                    _rotationAngle,
                    new Vector2(_batImageSize.X / 2, _batImageSize.Y / 2),
                    SpriteEffects.None,
                    0f
                    );
                spriteBatch.End();

                spriteBatch.Begin();
                foreach (Rectangle rec in locations)
                {
                    spriteBatch.Draw(_heartpic,
                        rec,
                        null,
                        Color.Red,
                        _rotationAngle,
                        new Vector2(_heartImageSize.X / 2, _heartImageSize.Y / 2),
                        SpriteEffects.None,
                        0f
                        );
                }
                spriteBatch.End();
                this.locations = new Stack<Rectangle>();
            }
        }

        public void MoveLeft()
        {
            X = X - 5;

        }
        public void MoveUp()
        {
            Y = Y - 5;
            if(Y < 20)
            {
                Y = 20;
            }
            _rotationAngle = Math.Max(_rotationAngle, -MathHelper.Pi / 2);
        }
        public void MoveDown()
        {
            Y = Y + 5;

            if (Y > ScreenHeight - 20)
            {
                Y = ScreenHeight - 20;
            }
            _rotationAngle = Math.Min(_rotationAngle, MathHelper.Pi / 2);
        }
        public void MoveRight()
        {
            X = X + 5;

            if (X > ScreenWidth - 50)
            {
                X = ScreenWidth - 50;
            }
        }

        public void CreateEchoWave(GameTime gt)
        {
            var currentTime = (float)gt.TotalGameTime.TotalMilliseconds / 1000;
            if (currentTime - lastEchoTime > echoCoolDown)
            {
                lastEchoTime = currentTime;
                var echoWave = new EchoWave(gt, new Vector2(X, Y));
                echoWaves.Add(echoWave);
                echoWave.parent = this;
                echoWave.spriteBatch = spriteBatch;
                var castSound = GameContent.instance.echoCast.CreateInstance();
                castSound.Volume = 0.3f;
                castSound.Play();
            }
        }
    }
}
