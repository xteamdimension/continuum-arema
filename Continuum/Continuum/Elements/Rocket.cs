using Microsoft.Xna.Framework;
using Continuum.Weapons;
using Continuum.State;
using Continuum.Utilities;
using System;
using Continuum.Management;

namespace Continuum.Elements
{
    /// <summary>
    /// Proiettile dell'arma "RocketLauncher"
    /// </summary>
    public class Rocket : Bullet
    {
        public Rocket(Vector2 StartPosition, Vector2 Direction, RocketLauncher rocketLauncher, GameState gameState)
        {
            InitializeBullet(StartPosition, Direction, TextureConstant.ROCKET ,rocketLauncher, gameState);
        }

        public Rocket() { }

        protected override float EvaluateRotation(float RotationDelta)
        {
            return Utility.CalculateXAngleFromVector(direction) + (float)Math.PI/2;
        }

        public override void HasCollided(int Value, object arg)
        {
            if (!(arg is GunBullet))
            {
                lifeState = LifeState.DEAD;
            }
        }
    }
}
