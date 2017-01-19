using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Snake
{
    public class SnakeParam
    {
        public int snakeSize = 1;
        public float speed = 3f;
        public float move = 1;
        public int X = 4;
        public List<Vector2> snakeBody;
        public List<Vector3> snakeBodyWithDirs;
        public int BodySize { get { return snakeBody.Count; } }

        public Vector2 snakePosition;
        public Vector2 snakeBodyPosition;
        public Texture2D snakeTexture;
        public Point snakeSpriteSize;


        public SnakeParam()
        {
            snakeBody = new List<Vector2>();
            snakePosition = Vector2.Zero;
            snakeBodyPosition = Vector2.Zero;
        }

        private void MoveHorizontally()
        {
            snakePosition.X += move * speed;
        }

        private void MoveVertically()
        {
            snakePosition.Y += move * speed;
        }

        public void Move()
        {
            if (X == 1 || X == 0)
                MoveHorizontally();
            else
            if (X == 2 || X == 3)
                MoveVertically();
        }

        public void Turn(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                if (X != (int)Game1.direct.right)
                {
                    move = -1;
                    X = (int)Game1.direct.left;
                }
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                if (X != (int)Game1.direct.left)
                {
                    move = 1;
                    X = (int)Game1.direct.right;
                }
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (X != (int)Game1.direct.down)
                {
                    move = -1;
                    X = (int)Game1.direct.up;
                }
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (X != (int)Game1.direct.up)
                {
                    move = 1;
                    X = (int)Game1.direct.down;
                }
            }
        }
    }
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D foodTexture;
        Vector2 foodPosition;
        Point foodSpriteSize;
        public enum direct : int { right, left, up, down };
        SnakeParam snake = new SnakeParam();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            foodPosition = Vector2.Zero;
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
            snake.snakeTexture = Content.Load<Texture2D>("body");
            foodTexture = Content.Load<Texture2D>("nyamka");
            snake.snakeSpriteSize = new Point(snake.snakeTexture.Width, snake.snakeTexture.Height);
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
            GenerateFoodPosition();
            Vector2 oldSnakePosition = snake.snakePosition;

            snake.Move();
            snake.Turn(keyboardState);

            if (snake.snakePosition.X > Window.ClientBounds.Width - snake.snakeTexture.Width - 1 || snake.snakePosition.X < 0 || snake.snakePosition.Y > Window.ClientBounds.Height - snake.snakeTexture.Height - 1 || snake.snakePosition.Y < 0)
            {
                snake.X = 4;
            }

            if ((Collide()))
            {
                Increase();
                snake.snakeBody.Add(Vector2.Zero);
                if (snake.BodySize > 1)
                    for (int i = snake.BodySize - 1; i > 0; i--)
                        snake.snakeBody[i] = snake.snakeBody[i - 1];
                snake.snakeBody[0] = oldSnakePosition;
                foodPosition = Vector2.Zero;
            }
            else
            {
                if (snake.BodySize > 1)
                    for (int i = snake.BodySize - 1; i > 0; i--)
                        snake.snakeBody[i] = snake.snakeBody[i - 1];
                if (snake.BodySize > 0)
                    snake.snakeBody[0] = oldSnakePosition;
            }
            base.Update(gameTime);
        }

        private void GenerateFoodPosition()
        {
            Random random = new Random();
            if (foodPosition == Vector2.Zero)
            {
                do
                {
                    foodPosition.X = random.Next(0, Window.ClientBounds.Width - foodTexture.Width - 1);
                    foodPosition.Y = random.Next(0, Window.ClientBounds.Height - foodTexture.Height - 1);
                } while (FoodPositionOnSnake());
            }
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
            spriteBatch.Draw(snake.snakeTexture, snake.snakePosition, Color.White);
            for (int i = 0; i < snake.BodySize; i++)
                spriteBatch.Draw(snake.snakeTexture, snake.snakeBody[i], Color.White);
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
            Rectangle snakeSpriteRect = new Rectangle((int)snake.snakePosition.X,
                (int)snake.snakePosition.Y, snake.snakeSpriteSize.X, snake.snakeSpriteSize.Y);
            Rectangle foodSpriteRect = new Rectangle((int)foodPosition.X,
                (int)foodPosition.Y, foodSpriteSize.X, foodSpriteSize.Y);

            return snakeSpriteRect.Intersects(foodSpriteRect);
        }

        protected void Increase()
        {
            snake.snakeSize++;
        }
    }
}
