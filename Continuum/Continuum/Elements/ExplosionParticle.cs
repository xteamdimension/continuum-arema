using Microsoft.Xna.Framework;
using Continuum.State;
using Continuum.Utilities;
using System;

namespace Continuum.Elements
{
    /// <summary>
    /// Scheggia
    /// </summary>
    public class Chip : TimeTraveler
    {
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
        /// Il danno che apporta la scheggia
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
                    lastDamage = Math.Max(0, a * delta * delta + Constants.GRANADE_DAMAGE);
                    updatedAt = delta; // Si salva il momento in cui è stato calcolato il danno.
                }
                return (int)lastDamage + 1;
            }
        }

        /// <summary>
        /// Crea una scheggia
        /// </summary>
        /// <param name="Position">La posizione da cui parte la scheggia</param>
        /// <param name="Direction">La direzione della scheggia. Se impostato a Vector2.Zero la direzione è casuale.</param>
        /// <param name="Speed">La velocità della scheggia</param>
        /// <param name="Duration">La durata della vita della scheggia</param>
        /// <param name="Damage">Il danno recato dalla scheggia. Impostare a 0 se la scheggia non fa danno</param>
        /// <param name="Life">Il valore di vita della scheggia. Impostare a 0 se la scheggia non fa danno</param>
        /// <param name="TextureGroupName">Il nome del gruppo di texture che viene utilizzato per renderizzare la scheggia</param>
        /// <param name="gameState">Il GameState</param>
        public Chip(Vector2 Position, Vector2 Direction, float Speed, float DeltaDuration, int Damage, int Life, string TextureGroupName, GameState gameState)
        {
            if (Direction == Vector2.Zero)
                Direction = new Vector2(Utility.NextRandom(0, 2f) - 1f, Utility.NextRandom(0, 2f) - 1f);
            Direction.Normalize();
            this.Direction = Direction;
            this.lastDamage = Damage;
            this.life = Life;
            this.DeltaDuration = DeltaDuration;
            this.a = -(Damage) / (DeltaDuration * DeltaDuration);

            float rotationSpeed = Utility.NextRandom(-20f, 20f);
            string[] textureNames = new string[] {TextureGroupName + "1", TextureGroupName + "2", TextureGroupName + "3"};
            InitializeTimeTraveler(Position, Speed, textureNames[Utility.NextRandom(0,3)], gameState, 0, rotationSpeed);
        }

        public Chip() { }

        public override Vector2 EvaluatePosition(float Delta)
        {
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
