using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Snake
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteBatch foodSprite;
        Texture2D snakeTexture;
        Texture2D foodTexture;
        Vector2 snakePosition;
        Vector2 snakeBodyPosition;
        Vector2 foodPosition;
        Point snakeSpriteSize;
        Point foodSpriteSize;
        int X = 4;
        int snakeSize = 1;
        float speed = 2f;
        float move = 1;
        enum direct : int {right, left, up, down}

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            foodPosition = new Vector2(Window.ClientBounds.Width / 2, 0);
            snakePosition = Vector2.Zero;
            snakeBodyPosition = Vector2.Zero;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            snakeTexture = Content.Load<Texture2D>("body");
            foodTexture = Content.Load<Texture2D>("nyamka");
            snakeSpriteSize = new Point(snakeTexture.Width, snakeTexture.Height);
            foodSpriteSize = new Point(foodTexture.Width, foodTexture.Height);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Random random = new Random();
            if (foodPosition == Vector2.Zero)
            {
                do
                {
                    foodPosition.X = random.Next(Window.ClientBounds.Width - foodTexture.Width - 1, 0);
                    foodPosition.Y = random.Next(Window.ClientBounds.Height - foodTexture.Height - 1, 0);
                } while (FoodPositionOnSnake());
            }
            else { }

            if (X == 0 || X == 1)
            {
                snakePosition.X += move * speed;
            }
            else if (X == 2 || X == 3)
            {
                snakePosition.Y += move * speed;
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                move = -1;
                X = (int)direct.left;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                move = 1;
                X = (int)direct.right;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                move = -1;
                X = (int)direct.up;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                move = 1;
                X = (int)direct.down;
            }
            // TODO: Add your update logic here
            if (snakePosition.X > Window.ClientBounds.Width - snakeTexture.Width - 1 || snakePosition.X < 0 || snakePosition.Y > Window.ClientBounds.Height - snakeTexture.Height - 1 || snakePosition.Y < 0)
            {
                X = 4;
            }
            if ((Collide()))
            {
                Increase();
                foodPosition = new Vector2(Window.ClientBounds.Width / 2, 100);
            }
            base.Update(gameTime);
        }

        private bool FoodPositionOnSnake()
        {
            return false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            for (int i = 0; i < snakeSize; i++)
            {
                if (X == 0 && snakeSize > 1)
                {
                    spriteBatch.Draw(snakeTexture, snakePosition, Color.White);
                    snakeBodyPosition.X = snakePosition.X - 20;
                    spriteBatch.Draw(snakeTexture, snakeBodyPosition, Color.White);
                }
                else if (X == 1 && snakeSize > 1)
                {
                    spriteBatch.Draw(snakeTexture, snakePosition, Color.White);
                    snakeBodyPosition.X = snakePosition.X +20;
                    spriteBatch.Draw(snakeTexture, snakeBodyPosition, Color.White);
                }
                else if (X == 2 && snakeSize > 1)
                {
                    spriteBatch.Draw(snakeTexture, snakePosition, Color.White);
                    snakeBodyPosition.X = snakePosition.Y - 20;
                    spriteBatch.Draw(snakeTexture, snakeBodyPosition, Color.White);
                }
                else if (X == 3 && snakeSize > 1)
                {
                    spriteBatch.Draw(snakeTexture, snakePosition, Color.White);
                    snakeBodyPosition.X = snakePosition.Y + 20;
                    spriteBatch.Draw(snakeTexture, snakeBodyPosition, Color.White);
                }
                else
                spriteBatch.Draw(snakeTexture, snakePosition, Color.White);
            }
            spriteBatch.Draw(foodTexture, foodPosition, Color.White);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        /// <summary>
        /// Обработка столкновений
        /// </summary>
        /// <returns></returns>
        protected bool Collide()
        {
            Rectangle goodSpriteRect = new Rectangle((int)snakePosition.X,
                (int)snakePosition.Y, snakeSpriteSize.X, snakeSpriteSize.Y);
            Rectangle evilSpriteRect = new Rectangle((int)foodPosition.X,
                (int)foodPosition.Y, foodSpriteSize.X, foodSpriteSize.Y);

            return goodSpriteRect.Intersects(evilSpriteRect);
        }
        protected void Increase()
        {
            if(X == 0)
            {
                snakeSize++;

            }
        }
    }
}
