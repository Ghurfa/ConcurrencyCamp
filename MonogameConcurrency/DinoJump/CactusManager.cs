using DinoJump.Synchronization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DinoJump
{
    public class CactusManager : IEnumerable<Cactus>
    {
        private LinkedList<Cactus> cacti;
        private CancellationToken deadToken;
        private Texture2D spriteSheet;
        private Rectangle[] frames;
        private int floorHeight;
        private Random random;
        private Func<Task> updateCactiTask;
        public CactusManager(Texture2D spriteSheet, int floorHeight, Rectangle[] frames)
        {
            cacti = new LinkedList<Cactus>();
            this.spriteSheet = spriteSheet;
            this.frames = frames;
            this.floorHeight = floorHeight;
            random = new Random();

            updateCactiTask = async () =>
            {
                while (deadToken == null ? true : !deadToken.IsCancellationRequested)
                {
                    foreach (Cactus cactus in cacti)
                    {
                        cactus.Update();
                    }
                    if (cacti.Count > 0)
                    {
                        Cactus frontCactus = cacti.First();
                        if (frontCactus.Position.X + frontCactus.SourceRectangle.Width < 0)
                        {
                            cacti.RemoveFirst();
                        }
                    }
                    if (cacti.Count < 8)
                    {
                        if (cacti.Count == 0)
                        {
                            AddCactus(1500);
                        }
                        else
                        {
                            int backCactusRightSide = cacti.Last().Position.Right;
                            int gap = random.Next(400, 600);
                            AddCactus(backCactusRightSide + gap);
                        }
                    }
                    await Task.Yield();
                }
            };
            Coroutines.StartCoroutineAsync(updateCactiTask);
        }
        public void SetCancellationToken(CancellationToken token)
        {
            deadToken = token;
        }

        public void Restart()
        {
            cacti.Clear();
            deadToken = default;
            Coroutines.StartCoroutineAsync(updateCactiTask);
        }

        private void AddCactus(int xPosition)
        {
            Rectangle srcRect = frames[random.Next(frames.Length)];
            Cactus newCactus = new Cactus(spriteSheet, new Rectangle(xPosition, floorHeight - srcRect.Height, srcRect.Width, srcRect.Height), srcRect);
            cacti.AddLast(newCactus);
        }

        public IEnumerator<Cactus> GetEnumerator()
        {
            foreach(Cactus cactus in cacti)
            {
                yield return cactus;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (Cactus cactus in cacti)
            {
                yield return cactus;
            }
        }
    }
}
