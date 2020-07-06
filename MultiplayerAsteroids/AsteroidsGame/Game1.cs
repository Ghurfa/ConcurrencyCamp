using ConcurrencyAsteroidsLibrary.Models;
using CoroutinesLib;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShipModel = ConcurrencyAsteroidsLibrary.Models.Ship;
using BulletModel = ConcurrencyAsteroidsLibrary.Models.Bullet;
using RockModel = ConcurrencyAsteroidsLibrary.Models.Rock;

namespace AsteroidsGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        AsteroidsServerHubConnector connector;

        string playerName = "Lorenzo";
        Ship player;

        List<Ship> localShips;
        List<Asteroid> localAsteroids;
        List<Bullet> localBullets;

        HubConnection connection;

        SpriteFont gameFont;

        KeyboardState ks;

        Func<Task> UpdateShipPosition;
        Func<Task> MoveShip;
        Func<Task> ShootBullets;
        Func<Task> UpdateBullets;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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

            connection = new HubConnectionBuilder().WithUrl("http://192.168.1.172:5000/tetrisHub").WithAutomaticReconnect().Build();
            var oldSyncContext = SynchronizationContext.Current;

            SynchronizationContext.SetSynchronizationContext(null);

            connector = new AsteroidsServerHubConnector(connection);
            connector.OnReceiveAsteroids += async rocks =>
            {
                await Coroutines.ContinueOnMainThread();
                localAsteroids.Clear();
                foreach (RockModel rock in rocks)
                {
                    localAsteroids.Add(new Asteroid(rock));
                }
            };
            connector.OnReceiveBullets += async bullets =>
            {
                await Coroutines.ContinueOnMainThread();
                localBullets.Clear();
                foreach(BulletModel bulletModel in bullets)
                {
                    localBullets.Add(new Bullet(bulletModel));
                }
            };
            connector.OnReceiveShips += async ships =>
            {
                await Coroutines.ContinueOnMainThread();
                localShips.Clear();
                for (int i = 0; i < ships.Count; i++)
                {
                    if(ships[i].Name == playerName)
                    {
                        player.Lives = ships[i].Lives;
                        player.Score = ships[i].Score;
                    }
                    else
                    {
                        localShips.Add(new Ship(ships[i]));
                    }
                }
            };

            //Connect then send data
            Coroutines.StartCoroutineAsync(async () =>
            {
                while (connection.State == HubConnectionState.Disconnected)
                {
                    await connection.StartAsync();
                }
                await connector.NewPlayer(playerName);
                Task.Run(connector.GetRemoteRocks);
                Task.Run(connector.GetRemoteShips);
                Task.Run(connector.GetRemoteBullets);
                Coroutines.StartCoroutineAsync(UpdateBullets);
                Coroutines.StartCoroutineAsync(UpdateShipPosition);
            });

            connection.Reconnected += async s =>
            {
                await connector.NewPlayer(playerName);                
            };

            UpdateShipPosition = async() =>
            {
                while (true)
                {
                    try
                    {
                        await connector.UpdateShipPosition(player.Position.X, player.Position.Y, player.Angle);
                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
            };

            MoveShip = async () =>
            {
                while (true)
                {
                    float turnSpeed = 0.1f;
                    float dampFactor = 0.98f;
                    if (ks.IsKeyDown(Keys.Left))
                    {
                        player.Angle -= turnSpeed;
                    }
                    else if (ks.IsKeyDown(Keys.Right))
                    {
                        player.Angle += turnSpeed;
                    }
                    player.Velocity *= dampFactor;
                    if (ks.IsKeyDown(Keys.Up))
                    {
                        float angle = player.Angle;
                        float acceleration = 0.15f;
                        float maxSpeed = 3;
                        Vector2 step = Sprite.CalculateVector(angle, acceleration);
                        //if(player.Velocity.Length() < maxSpeed)
                        //{
                            player.Velocity += step;
                        //}
                    }
                    player.UpdatePosition(GraphicsDevice.Viewport);
                    await Task.Delay(5);
                }
            };
            ShootBullets = async () =>
            {
                while (true)
                {
                    if (ks.IsKeyDown(Keys.Space))
                    {
                        //player.Shoot();
                        await connector.NewBullet();
                        await Task.Delay(250);
                    }
                    else
                    {
                        await Task.Yield();
                    }
                }
            };

            UpdateBullets = async () =>
            {
                while (true)
                {
                    /*for (int i = 0; i < player.Bullets.Count; i++)
                    {
                        Bullet bullet = player.Bullets[i];
                        bullet.UpdatePosition();
                        if (bullet.Position.X < 0 || bullet.Position.X > GraphicsDevice.Viewport.Width ||
                            bullet.Position.Y < 0 || bullet.Position.Y > GraphicsDevice.Viewport.Height)
                        {
                            player.Bullets.RemoveAt(i);
                            i--;
                        }
                    }
                    for (int i = 0; i < localBullets.Count; i++)
                    {
                        Bullet bullet = localBullets[i];
                        bullet.UpdatePosition();
                        if (bullet.Position.X < 0 || bullet.Position.X > GraphicsDevice.Viewport.Width ||
                            bullet.Position.Y < 0 || bullet.Position.Y > GraphicsDevice.Viewport.Height)
                        {
                            localBullets.RemoveAt(i);
                            i--;
                        }
                    }*/
                    foreach(Bullet bullet in localBullets)
                    {
                        bullet.UpdatePosition();
                    }
                    await Task.Yield();
                }
            };

            SynchronizationContext.SetSynchronizationContext(oldSyncContext);

            localShips = new List<Ship>();
            localBullets = new List<Bullet>();
            localAsteroids = new List<Asteroid>();
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
            Ship.ShipTexture = Content.Load<Texture2D>("asteroids ship");
            Ship.ShipNameFont = Content.Load<SpriteFont>("shipNameFont");
            Bullet.BulletTexture = Content.Load<Texture2D>("bullet");
            player = new Ship(new Vector2(GraphicsDevice.Viewport.Width / 2 - Ship.ShipTexture.Width / 2, GraphicsDevice.Viewport.Height / 2 - Ship.ShipTexture.Height / 2), 0, playerName);

            Asteroid.AsteroidTexture = Content.Load<Texture2D>("asteroid");

            gameFont = Content.Load<SpriteFont>("gameText");

            Coroutines.StartCoroutine(MoveShip);
            Coroutines.StartCoroutine(ShootBullets);

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
            Coroutines.ExecuteContinuations();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ks = Keyboard.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            foreach (Bullet bullet in localBullets)
            {
                bullet.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);
            foreach (Bullet bullet in player.Bullets)
            {
                bullet.Draw(spriteBatch);
            }

            foreach (Sprite otherShip in localShips)
            {
                otherShip.Draw(spriteBatch);
            }

            foreach (Asteroid asteroid in localAsteroids)
            {
                asteroid.Draw(spriteBatch);
            }

            if (connection.State == HubConnectionState.Connected)
            {
                spriteBatch.DrawString(gameFont, "Connected", Vector2.Zero, Color.Green);
            }
            else
            {
                spriteBatch.DrawString(gameFont, "Not connected", Vector2.Zero, Color.Red);
            }

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
