using Microsoft.Xna.Framework;
using Continuum.State;
using Continuum.Utilities;
using System;

namespace Continuum.Elements
{
    public class ExplosionParticle : TimeTraveler
    {
        private const float DELTA_DURATION = 100f;
        private Vector2 Direction;
        public float Alpha { get; private set; }

        public ExplosionParticle(Vector2 Position, GameState gameState)
        {
            this.Direction = new Vector2(Utility.NextRandom(0,2f)-1f,Utility.NextRandom(0,2f)-1f);
            this.Direction.Normalize();
            float speed = Utility.NextRandom(50f, 200f);
            string[] textureNames = {"explosionParticle1", "explosionParticle2", "explosionParticle3"};
            float rotationSpeed = Utility.NextRandom(-20f, 20f);
            InitializeTimeTraveler(Position, speed, textureNames[Utility.NextRandom(0,3)], gameState, 0, rotationSpeed);
        }

        public ExplosionParticle() { }

        public override Vector2 EvaluatePosition(float Delta)
        {
            if (lifeState != LifeState.DEAD)
            {
                float q = DELTA_DURATION / 2f;
                Alpha = (-(Math.Abs(-Delta + q)) + q) / q;
            }
            if (Delta > DELTA_DURATION && lifeState != LifeState.DEAD)
                lifeState = LifeState.DEAD;
            return startPosition + Direction * Delta;
        }

        public override void HasCollided(int Value, object Arg) 
        {
            if (lifeState != LifeState.DEAD)
                lifeState = LifeState.DEAD;
        }
    }
}
