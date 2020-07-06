using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoJump
{
    public struct Frame
    {
        public Rectangle SourceRectangle;
        public int NumFrames;
        public Frame(Rectangle src, int numFrames)
        {
            SourceRectangle = src;
            NumFrames = numFrames;
        }
    }
    public class AnimatedSprite : Sprite
    {
        protected Frame[][] Frames;
        public int CurrentFrame = 0;
        public int CurrentFrameSet = 0;
        protected int numFrames;
        public AnimatedSprite(Texture2D texture, Rectangle position, int numFrames, Frame[][] frames)
            :base(texture, position, frames[0][0].SourceRectangle)
        {
            this.texture = texture;
            Position = position;
            Frames = frames;
            this.numFrames = numFrames;
        }
        public async Task Update()
        {
            CurrentFrame = (CurrentFrame + 1) % Frames[CurrentFrameSet].Length;
            Frame currentFrame = Frames[CurrentFrameSet][CurrentFrame];
            for (int i = 0; i < currentFrame.NumFrames; i++)
            {
                currentFrame = Frames[CurrentFrameSet][CurrentFrame];
                await Task.Yield();
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Frames[CurrentFrameSet][CurrentFrame].SourceRectangle, Color.White);
        }
    }
}
