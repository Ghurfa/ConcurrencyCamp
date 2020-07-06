using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidsGame
{
    public class Sprite
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Angle;
        protected Texture2D texture;
        protected Vector2 center;
        public Sprite(Vector2 position, float angle, Texture2D texture)
        {
            Position = position;
            this.texture = texture;
            center = new Vector2(texture.Width / 2, texture.Height / 2);
            Angle = angle;
            Velocity = Vector2.Zero;
        }
        public static Vector2 CalculateVector(float angle, float distance)
        {
            float xDelta = distance * (float)Math.Cos(angle);
            float yDelta = distance * (float)Math.Sin(angle);
            return new Vector2(xDelta, yDelta);
        }
        public virtual void UpdatePosition()
        {
            Position += Velocity;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, Angle, center, Vector2.One, SpriteEffects.None, 1);
        }
    }
}
