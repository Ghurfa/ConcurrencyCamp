using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RockModel = ConcurrencyAsteroidsLibrary.Models.Rock;

namespace AsteroidsGame
{
    public class Asteroid : Sprite
    {
        public static Texture2D AsteroidTexture;
        public Vector2 Size;
        public Asteroid(Vector2 position, float angle)
            :base(position, angle, AsteroidTexture)
        {
            Velocity = Vector2.Zero;
            Size = new Vector2(75, 75);
        }
        public Asteroid(RockModel model)
            :base(new Vector2(model.X, model.Y), model.Rotation, AsteroidTexture)
        {
            Velocity = new Vector2(model.SpeedX, model.SpeedY);
            Size = new Vector2(model.Width, model.Height);
        }

        /*public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Bullet.BulletTexture, Position, null, Color.White, Angle, center, new Vector2(10, 10), SpriteEffects.None, 1);
        }*/
    }
}
