using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoJump
{
    public class Cactus : Sprite
    {
        float XVelocity = -15;
        float xPosition;
        public Cactus(Texture2D texture, Rectangle position, Rectangle sourceRectangle)
            :base(texture, position, sourceRectangle)
        {
            xPosition = position.X;
        }
        public void Update()
        {
            xPosition += XVelocity;
            Position.X = (int)xPosition;
        }
    }
}
