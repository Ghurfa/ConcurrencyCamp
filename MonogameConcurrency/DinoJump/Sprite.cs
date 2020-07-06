using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoJump
{
    public class Sprite
    {
        protected Texture2D texture;
        public Rectangle Position;
        public Rectangle SourceRectangle;

        public Sprite(Texture2D texture, Rectangle position, Rectangle sourceRectangle)
        {
            this.texture = texture;
            Position = position;
            SourceRectangle = sourceRectangle;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, SourceRectangle, Color.White);
        }
    }
}
