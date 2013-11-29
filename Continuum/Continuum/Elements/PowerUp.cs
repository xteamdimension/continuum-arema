using Microsoft.Xna.Framework;
using Continuum.Utilities;
using Continuum.State;
using Continuum.Management;

namespace Continuum.Elements
{
    /// <summary>
    /// Power Up
    /// </summary>
    public class PowerUp : TimeTraveler
    {
        public PowerUpType Type;

        /// <summary>
        /// Costruttore della classe PowerUp
        /// </summary>
        /// <param name="Position">La posizione da cui parte il PowerUp</param>
        /// <param name="Texture">La texture del PowerUp</param>
        /// <param name="gameState"></param>
        public PowerUp(Vector2 Position, PowerUpType Type, string Texture, GameState gameState)
        {
            InitializeTimeTraveler(Position, Constants.POWERUP_SPEED, Texture, gameState, 0, 0);
            this.Type = Type;
        }

        public PowerUp() { }

        /// <summary>
        /// Valuta la posizione del PowerUp
        /// </summary>
        /// <returns>La posizione del PowerUp</returns>
        public override Vector2 EvaluatePosition(float Delta)
        {
            Vector2 evaluate = new Vector2();
            evaluate.X = startPosition.X;
            evaluate.Y = startPosition.Y + Delta;
            return evaluate;
        }

        public override void HasCollided(int Value, object arg)
        {
            if (lifeState != LifeState.DEAD)
            {
                this.lifeState = LifeState.DEAD;
                SoundManager.PlaySound("powerUp");
            }
        }
    }
}
