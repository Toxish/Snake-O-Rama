using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Snake
{
    public struct PosAndDir
    {
        public float X;
        public float Y;
        public int Dir;
        public static PosAndDir Zero { get { return new PosAndDir(0, 0, 4); } }
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public PosAndDir(float x, float y, int dir)
        {
            X = x;
            Y = y;
            Dir = dir;
        }
    }
    public class SnakeParam
    {
        public int snakeSize = 1;
        public static int speed = 3;
        public static int move = 1;
        public int X = 4;
        public List<PosAndDir> snakeBody;
        public List<Vector3> snakeBodyWithDirs;
        public int BodySize { get { return snakeBody.Count; } }

        public PosAndDir snakePosition;
        public Vector2 snakeBodyPosition;
        public Texture2D snakeTexture;
        public Point snakeSpriteSize;

        public static PosAndDir GetNewBodyPos(PosAndDir pad)
        {
            PosAndDir newpad = new PosAndDir();
            newpad.Dir = pad.Dir;
            const int delta = 18;
            switch (pad.Dir)
            {
                case 0:
                    newpad.X = pad.X - delta;
                    newpad.Y = pad.Y;
                    break;
                case 1:
                    newpad.X = pad.X + delta;
                    newpad.Y = pad.Y;
                    break;
                case 2:
                    newpad.Y = pad.Y + delta;
                    newpad.X = pad.X;
                    break;
                case 3:
                    newpad.Y = pad.Y - delta;
                    newpad.X = pad.X;
                    break;
            }
            return newpad;
        }

        public SnakeParam()
        {
            snakeBody = new List<PosAndDir>();
            snakePosition = PosAndDir.Zero;
            snakeBodyPosition = Vector2.Zero;
        }

        public void Move()
        {
            switch (X)
            {
                case 0:
                    snakePosition.X += speed;
                    snakePosition.Dir = (int)Game1.direct.right;
                    break;
                case 1:
                    snakePosition.X -= speed;
                    snakePosition.Dir = (int)Game1.direct.left;
                    break;
                case 2:
                    snakePosition.Y -= speed;
                    snakePosition.Dir = (int)Game1.direct.up;
                    break;
                case 3:
                    snakePosition.Y += speed;
                    snakePosition.Dir = (int)Game1.direct.down;
                    break;
            }
        }

        public void Turn(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                if (X != (int)Game1.direct.right)
                    X = (int)Game1.direct.left;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                if (X != (int)Game1.direct.left)
                    X = (int)Game1.direct.right;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (X != (int)Game1.direct.down)
                    X = (int)Game1.direct.up;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (X != (int)Game1.direct.up)
                    X = (int)Game1.direct.down;
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
            PosAndDir oldSnakePosition = snake.snakePosition;

            snake.Move();
            snake.Turn(keyboardState);

            if (snake.snakePosition.X > Window.ClientBounds.Width - snake.snakeTexture.Width - 1 || snake.snakePosition.X < 0 || snake.snakePosition.Y > Window.ClientBounds.Height - snake.snakeTexture.Height - 1 || snake.snakePosition.Y < 0)
            {
                snake.X = 4;
            }

            if (Collide())
            {
                Increase();
                snake.snakeBody.Add(PosAndDir.Zero);
                if (snake.BodySize > 1)
                    for (int i = snake.BodySize - 1; i > 0; i--)
                        snake.snakeBody[i] = SnakeParam.GetNewBodyPos(snake.snakeBody[i - 1]);
                snake.snakeBody[0] = SnakeParam.GetNewBodyPos(oldSnakePosition);
                foodPosition = Vector2.Zero;
            }
            else
            {
                if (snake.BodySize > 1)
                    for (int i = snake.BodySize - 1; i > 0; i--)
                        snake.snakeBody[i] = SnakeParam.GetNewBodyPos((snake.snakeBody[i - 1]));
                if (snake.BodySize > 0)
                    snake.snakeBody[0] = SnakeParam.GetNewBodyPos(oldSnakePosition);
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
            spriteBatch.Draw(snake.snakeTexture, snake.snakePosition.ToVector2(), Color.White);
            for (int i = 0; i < snake.BodySize; i++)
                spriteBatch.Draw(snake.snakeTexture, snake.snakeBody[i].ToVector2(), Color.White);
            spriteBatch.Draw(foodTexture, foodPosition, Color.White);
            spriteBatch.End();

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
