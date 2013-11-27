using System;
using Continuum.Utilities;
using Microsoft.Xna.Framework;
using Continuum.State;

namespace Continuum.Elements
{
    /// <summary>
    /// Granata al plasma. Segue la sua traiettoria ed esplode creando danno ad area alla prima collisione valida. Altrimenti esplode al termine del path.
    /// </summary>
    public class PlasmaGranade : Bullet
    {
        public const float PLASMA_GRANADE_SPEED = 1;
        public QuadraticBezierCurve Path;

        public PlasmaGranade(QuadraticBezierCurve Path, string TextureName, GameState gameState)
        {
            InitializeTimeTraveler(Path.startPoint, PLASMA_GRANADE_SPEED, TextureName, gameState);
            this.Path = Path;
            damage = 0;
            isPlayerBullet = true;
        }

        public PlasmaGranade() { }

        /// <summary>
        /// Valuta la posizione della granata
        /// </summary>
        /// <returns>La posizione della granata</returns>
        public override Vector2 EvaluatePosition(float Delta)
        {
            //Controllare di non superare la fine della curva
            if (Delta >= 1)
                Detonate();
            return Path.Evaluate(Delta);
        }

        public override void HasCollided(int Value, object arg)
        {
            if(!(arg is Bullet))
                Detonate();
        }

        private void Detonate()
        {
            if (lifeState != LifeState.DEAD)
            {
                // gs.newAreaDamage(CurrentPosition, TextureConstant.ANIMATION_EXPLOSION, 100, 10, 10, 2, 128, 1);
                for (int i = 0; i < 20; i++)
                {
                    ExplosionParticle ep = new ExplosionParticle(CurrentPosition, gs);
                    gs.explosionParticles.AddLast(ep);
                    gs.collisions.Insert(ep);
                }
            }
            lifeState = LifeState.DEAD;
        }

        public override string ToString()
        {
            return "PlasmaGranade - " + lifeState.ToString();
        }
    }
}
