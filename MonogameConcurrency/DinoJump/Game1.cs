using DinoJump.Synchronization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DinoJump
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Color backgroundColor = new Color(247, 247, 247);

        Texture2D spriteSheet;

        Dinosaur dino;
        CactusManager Cacti;
        Sprite ground1;
        Sprite ground2;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1200;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            SynchronizationContext.SetSynchronizationContext(Coroutines.SyncContext);
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
            spriteSheet = Content.Load<Texture2D>("dino jump");

            Frame[][] dinoFrames = new Frame[][] {
                new Frame[]{
                    new Frame(new Rectangle(1514, 2, 88, 94), 8),
                    new Frame(new Rectangle(1602, 2, 88, 94), 8)
                },
                new Frame[]{
                    new Frame(new Rectangle(1338, 2, 88, 94), int.MaxValue)
                },
                new Frame[]
                {
                    new Frame(new Rectangle(1866, 36, 118, 60), 8),
                    new Frame(new Rectangle(1984, 36, 118, 60), 8)
                },
                new Frame[]
                {
                    new Frame(new Rectangle(1690, 2, 88, 94), int.MaxValue)
                }
            };
            Rectangle[] cactusFrames = new Rectangle[]
            {
                new Rectangle(652, 2, 50, 96),
                new Rectangle(702, 2, 100, 96),
                new Rectangle(802, 2, 150, 96),
                new Rectangle(446, 2, 34, 70),
                new Rectangle(480, 2, 68, 70),
                new Rectangle(548, 2, 102, 70)
            };

            int floorHeight = 350;
            Cacti = new CactusManager(spriteSheet, floorHeight, cactusFrames);
            dino = new Dinosaur(spriteSheet, new Rectangle(100, floorHeight - dinoFrames[0][0].SourceRectangle.Height, 88, 94), dinoFrames, Cacti);
            Rectangle groundSrcRect = new Rectangle(2, 104, 2400, 24);
            ground1 = new Sprite(spriteSheet, new Rectangle(0, floorHeight - groundSrcRect.Height, groundSrcRect.Width, groundSrcRect.Height), groundSrcRect);
            ground2 = new Sprite(spriteSheet, new Rectangle(2400, floorHeight - groundSrcRect.Height, groundSrcRect.Width, groundSrcRect.Height), groundSrcRect);

            Cacti.SetCancellationToken(dino.GetDeadToken());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        KeyboardState keyboardState = Keyboard.GetState();
        KeyboardState lastKeyboardState = Keyboard.GetState();
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Coroutines.ExecuteContinuations();
            keyboardState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            dino.Update(keyboardState);

            if (dino.IsDead && keyboardState.IsKeyDown(Keys.R) && !lastKeyboardState.IsKeyDown(Keys.R))
            {
                Cacti.Restart();
                dino.Restart(Cacti);
                Cacti.SetCancellationToken(dino.GetDeadToken());
            }

            // TODO: Add your update logic here

            lastKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);
            spriteBatch.Begin();

            ground1.Draw(spriteBatch);
            ground2.Draw(spriteBatch);

            foreach (Cactus cactus in Cacti)
            {
                cactus.Draw(spriteBatch);
            }
            dino.Draw(spriteBatch);

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
