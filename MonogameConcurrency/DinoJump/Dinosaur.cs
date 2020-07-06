using DinoJump.Synchronization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DinoJump
{
    public class Dinosaur : AnimatedSprite
    {
        public float YVelocity;
        public float FloorHeight;
        public bool IsDead { get; private set; }
        private float yPosition;
        private float gravity = 2;
        private KeyboardState ks;
        private KeyboardState lastKs;
        private Func<Task> animationTask;
        private Func<Task> jumpTask;
        private Func<Task> duckTask;
        private Func<Task> checkDeadTask;
        private CancellationTokenSource jumpCancelSource;
        private CancellationTokenSource duckCancelSource;
        private CancellationTokenSource deadCancelSource;

        public Dinosaur(Texture2D texture, Rectangle position, Frame[][] frames, CactusManager cacti)
            : base(texture, position, 2, frames)
        {
            yPosition = position.Y;
            FloorHeight = position.Y + frames[0][0].SourceRectangle.Height;

            Restart(cacti);

            ks = Keyboard.GetState();
        }

        public void Update(KeyboardState ks)
        {
            lastKs = this.ks;
            this.ks = ks;
        }

        public void Restart(CactusManager cacti)
        {
            YVelocity = 0;
            IsDead = false;
            changeFrameSet(0);
            jumpCancelSource = new CancellationTokenSource();
            duckCancelSource = new CancellationTokenSource();
            deadCancelSource = new CancellationTokenSource();

            animationTask = async () =>
            {
                CancellationToken deadToken = deadCancelSource.Token;
                while (true)
                {
                    if (deadToken.IsCancellationRequested) break;
                    await base.Update();
                }
            };
            Coroutines.StartCoroutineAsync(animationTask);

            jumpTask = async () =>
            {
                CancellationToken deadToken = deadCancelSource.Token;
                while (true)
                {
                    if (deadToken.IsCancellationRequested) break;
                    CancellationToken duckToken = duckCancelSource.Token;
                    if (ks.IsKeyDown(Keys.Up) && !lastKs.IsKeyDown(Keys.Up) && Position.Bottom >= FloorHeight && !duckToken.IsCancellationRequested)
                    {
                        jumpCancelSource.Cancel();
                        YVelocity = -30;
                        gravity = 2;
                        changeFrameSet(1);
                        do
                        {
                            await Task.Yield();
                            YVelocity += gravity;
                            yPosition += YVelocity;
                            Position.Y = (int)yPosition;
                            if (duckToken.IsCancellationRequested || deadToken.IsCancellationRequested) break;
                        } while (Position.Bottom < FloorHeight);
                        if (deadToken.IsCancellationRequested) break;
                        jumpCancelSource = new CancellationTokenSource();
                        if (!duckToken.IsCancellationRequested)
                        {
                            changeFrameSet(0);
                        }
                    }
                    await Task.Yield();
                }
            };
            Coroutines.StartCoroutineAsync(jumpTask);

            duckTask = async () =>
            {
                CancellationToken deadToken = deadCancelSource.Token;
                while (true)
                {
                    if (deadToken.IsCancellationRequested) break;
                    if (ks.IsKeyDown(Keys.Down) && !lastKs.IsKeyDown(Keys.Down))
                    {
                        duckCancelSource.Cancel();
                        gravity = 5;
                        while (Position.Bottom < FloorHeight)
                        {
                            await Task.Yield();
                            YVelocity += gravity;
                            yPosition += YVelocity;
                            Position.Y = (int)yPosition;
                            if (deadToken.IsCancellationRequested) break;
                        }
                        if (deadToken.IsCancellationRequested) break;
                        YVelocity = 0;
                        changeFrameSet(2);
                        while (ks.IsKeyDown(Keys.Down))
                        {
                            if (deadToken.IsCancellationRequested) break;
                            await Task.Yield();
                        }
                        if (deadToken.IsCancellationRequested) break;
                        changeFrameSet(0);
                        duckCancelSource = new CancellationTokenSource();
                    }
                    await Task.Yield();
                }
            };
            Coroutines.StartCoroutineAsync(duckTask);

            checkDeadTask = async () =>
            {
                CancellationToken deadToken = deadCancelSource.Token;
                while (true)
                {
                    foreach (Cactus cactus in cacti)
                    {
                        if (cactus.Position.Intersects(Position))
                        {
                            YVelocity = 0;
                            changeFrameSet(3, false);
                            deadCancelSource.Cancel();
                            IsDead = true;
                            if(Position.Bottom > FloorHeight)
                            {
                                yPosition = FloorHeight - Position.Height;
                                Position.Y = (int)yPosition;
                            }
                        }
                    }
                    if (IsDead) break;
                    await Task.Yield();
                }
            };
            Coroutines.StartCoroutineAsync(checkDeadTask);
        }

        public CancellationToken GetDeadToken()
        {
            return deadCancelSource.Token;
        }

        private void changeFrameSet(int frameSet, bool moveToGround = true)
        {
            CurrentFrameSet = frameSet;
            CurrentFrame = 0;
            Position.Width = Frames[CurrentFrameSet][CurrentFrame].SourceRectangle.Width;
            Position.Height = Frames[CurrentFrameSet][CurrentFrame].SourceRectangle.Height;
            if (moveToGround)
            {
                yPosition = FloorHeight - Position.Height;
                Position.Y = (int)yPosition;
            }
        }
    }
}
