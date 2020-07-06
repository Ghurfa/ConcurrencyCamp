using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipModel = ConcurrencyAsteroidsLibrary.Models.Ship;

namespace AsteroidsGame
{
    public class Ship : Sprite
    {
        public static Texture2D ShipTexture;
        public static SpriteFont ShipNameFont;
        public string Name;
        public int Score;
        public int Lives;
        public List<Bullet> Bullets;
        public Ship(Vector2 position, float rotation, string name)
            :base(position, rotation, ShipTexture)
        {
            center = new Vector2(15, 24);
            Bullets = new List<Bullet>();
            Angle = rotation;
            Name = name;
            Score = 0;
            Lives = 3;
        }
        public Ship(ShipModel model)
            : base(new Vector2(model.X, model.Y), model.Rotation, ShipTexture)
        {
            center = new Vector2(15, 24);
            Bullets = new List<Bullet>();
            Angle = model.Rotation;
            Name = model.Name;
            Score = model.Score;
            Lives = model.Lives;
        }
        public void Shoot()
        {
            Vector2 nosePosition = Position + Sprite.CalculateVector(Angle, 24);
            Bullet newBullet = new Bullet(nosePosition, Angle);
            Bullets.Add(newBullet);
        }

        public void UpdatePosition(Viewport viewport)
        {
            base.UpdatePosition();
            if (Position.X < 0)
            {
                Position.X = viewport.Width;
            }
            else if (Position.X > viewport.Width)
            {
                Position.X = 0;
            }
            if (Position.Y < 0)
            {
                Position.Y = viewport.Height;
            }
            else if (Position.Y > viewport.Height)
            {
                Position.Y = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, Angle + MathHelper.PiOver2, center, Vector2.One, SpriteEffects.None, 1);
            spriteBatch.DrawString(ShipNameFont, $"{Name} ({Score.ToString()})", Position - new Vector2(ShipTexture.Width / 2, ShipTexture.Height + 10), Color.LightGray);
            spriteBatch.DrawString(ShipNameFont, Lives.ToString(), Position - new Vector2(ShipTexture.Width / 2, ShipTexture.Height), Color.Red);
        }
    }
}
