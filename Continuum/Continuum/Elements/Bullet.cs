using Microsoft.Xna.Framework;
using Continuum.Weapons;
using Continuum.State;
using Continuum.Utilities;
namespace Continuum.Elements
{
    public abstract class Bullet : TimeTraveler
    {
        public Vector2 direction;
        public int damage;

        public void InitializeBullet(Vector2 StartPosition, Vector2 Direction, string TextureName, IWeapons gun, GameState gameState)
        {
            InitializeTimeTraveler(StartPosition, gun.GetSpeed(gun.Level), TextureName, gameState);
            direction = Vector2.Normalize(Direction);
            damage = gun.GetDamage(gun.Level);
            if (gun.isPlayerWeapon)
                this.isPlayerBullet = true;
            else
                this.isPlayerBullet = false;
        }

        /// <summary>
        /// Valuta la posizione del Bullet
        /// </summary>
        /// <returns>La posizione del Bullet</returns>
        public override Vector2 EvaluatePosition(float Delta)
        {
            Vector2 evaluate = new Vector2();
            evaluate.X = startPosition.X + (direction.X * Delta);
            evaluate.Y = startPosition.Y + (direction.Y * Delta);
            return evaluate;
        }

        public override void HasCollided(int Value,object arg)
        {
            lifeState = LifeState.DEAD;
        }

        public bool isPlayerBullet { get; set; }

        public override string ToString()
        {
            return "Bullet - " + lifeState.ToString();
        }
    }
}
