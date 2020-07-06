using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletModel = ConcurrencyAsteroidsLibrary.Models.Bullet;

namespace AsteroidsGame
{
    public class Bullet : Sprite
    {
        public static Texture2D BulletTexture;
        public string PlayerName;
        public Bullet(Vector2 position, float angle = 0, float speed = 2)
            : base(position, angle, BulletTexture)
        {
            Velocity = Sprite.CalculateVector(angle, speed);
        }
        public Bullet(BulletModel bulletModel)
            : base(new Vector2(bulletModel.X, bulletModel.Y), 0, BulletTexture)
        {
            Velocity = new Vector2(bulletModel.SpeedX, bulletModel.SpeedY);
        }
    }
}
