using Microsoft.Xna.Framework;
using Continuum.Weapons;
using Continuum.State;
using Continuum.Utilities;

namespace Continuum.Elements
{
    /// <summary>
    /// Proiettile dell'arma "Gun"
    /// </summary>
    public class GunBullet : Bullet
    {
        public override Rectangle DestinationRectangle
        {
            get
            {
                if (isPlayerBullet)
                    return Utility.newRectangleFromCenterPosition(new Vector2((int)CurrentPosition.X, (int)CurrentPosition.Y), ((Width * damage/4) + 8), (Height * damage/4 + 8));
                else
                    return base.DestinationRectangle;
            }
        }

        public GunBullet(Vector2 StartPosition, Vector2 Direction, Gun gun, GameState gameState)
        {
            if (gun.isPlayerWeapon)
                InitializeBullet(StartPosition, Direction, TextureConstant.GUNBULLET, gun, gameState);
            else
                InitializeBullet(StartPosition, Direction, TextureConstant.ENEMYBULLET, gun, gameState);
        }

        public GunBullet() { }
    }
}
