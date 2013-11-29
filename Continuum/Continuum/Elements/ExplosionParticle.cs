using Microsoft.Xna.Framework;
using Continuum.State;
using Continuum.Utilities;
using System;

namespace Continuum.Elements
{
    public class ExplosionParticle : TimeTraveler
    {
        private static ExplosionParticle debugParticle;

        private float DeltaDuration;
        private Vector2 Direction;
        private float a; //Cofficient a of the parabola.
        private float updatedAt;
        private float lastDamage;
        private int life;

        /// <summary>
        /// Componente alfa del colore con cui viene renderizzato
        /// </summary>
        public float Alpha { get; private set; }
        
        /// <summary>
        /// Il danno che apporta la particella
        /// </summary>
        public int Damage {
            // Calcola il danno utilizzando una parabola.
            // Il danno diminuisce con il quadrato della distanza (che nel nostro caso corrisponde a delta = tempo passato * velocità)
            // La parabola è Y = a X^2 + c
            // Dove:
            //      Y -> danno
            //      X -> distanza
            //      a -> coefficiente (negativo) pensato per avere Y=0 quando la particella ha percorso tutta la sua distanza
            //      c -> coefficiente che trasla la parabola in alto. in questo modo il danno di partenza è quello massimo prestabilito
            get
            {
                // Calcola il danno attuale e lo memorizza per evitare calcoli ripetuti.
                if (updatedAt != delta)
                {
                    lastDamage = Math.Max(0, a * delta * delta + Constants.EXPLOSION_PARTICLE_DAMAGE);
                    updatedAt = delta; // Si salva il momento in cui è stato calcolato il danno.
                }
                return (int)lastDamage + 1;
            }
        }

        public ExplosionParticle(Vector2 Position, GameState gameState, bool granade)
        {
            this.Direction = new Vector2(Utility.NextRandom(0,2f)-1f,Utility.NextRandom(0,2f)-1f);
            this.Direction.Normalize();
            this.lastDamage = Constants.EXPLOSION_PARTICLE_DAMAGE;
            this.life = Constants.EXPLOSION_PARTICLE_DAMAGE;
            float speed = Utility.NextRandom(50f, 200f);
            string[] textureNames;
            if (granade)
            {
                if (debugParticle == null) debugParticle = this;
                textureNames = new string[] { "granadeExplosionParticle1", "granadeExplosionParticle2", "granadeExplosionParticle3" };
                DeltaDuration = 200f;
                speed += 100f;
                // Inizializzo il coefficiente a della parabola, che serve per calcolare la diminuzione del danno della particella
                // in modo proporzionale al quadrato della distanza da cui è partita.
                a = -(Constants.EXPLOSION_PARTICLE_DAMAGE) / (DeltaDuration * DeltaDuration);
            }
            else
            {
                textureNames = new string[] { "explosionParticle1", "explosionParticle2", "explosionParticle3" };
                DeltaDuration = 100f;
            }
            float rotationSpeed = Utility.NextRandom(-20f, 20f);
            InitializeTimeTraveler(Position, speed, textureNames[Utility.NextRandom(0,3)], gameState, 0, rotationSpeed);
        }

        public ExplosionParticle() { }

        public override Vector2 EvaluatePosition(float Delta)
        {
            if(debugParticle != null && debugParticle.GetHashCode() == this.GetHashCode())

            if (lifeState != LifeState.DEAD)
            {
                float q = DeltaDuration / 2f;
                Alpha = (-(Math.Abs(-Delta + q)) + q) / q;
            }
            if (Delta > DeltaDuration && lifeState != LifeState.DEAD)
                lifeState = LifeState.DEAD;
            return startPosition + Direction * Delta;
        }

        public override void HasCollided(int Value, object Arg) 
        {
            life -= Value;
            if (life <= 0 && lifeState != LifeState.DEAD)
                lifeState = LifeState.DEAD;
        }
    }
}
