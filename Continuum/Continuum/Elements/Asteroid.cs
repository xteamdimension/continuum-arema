using Microsoft.Xna.Framework;
using Continuum.Utilities;
using Continuum.State;
using System;

namespace Continuum.Elements
{
    /// <summary>
    /// Asteroide
    /// </summary>
    public class Asteroid : TimeTraveler
    {
        public Vector2 direction;
        public int life;

        public override Rectangle DestinationRectangle
        {
            get
            {
                return new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, ((Width*life)+2000)/50, (Height*life+2000)/50);
            }
        }

        /// <summary>
        /// Costruttore della classe Asteroid
        /// </summary>
        /// <param name="Position">Posizione di partenza dell'asteroide</param>
        /// <param name="Direction">Direzione dell'asteroide</param>
        /// <param name="Speed">Velocità dell'asteroide</param>
        /// <param name="Life">Punti vita dell'asteroide (rappresenta anche il danno che reca agli oggetti con cui entra in collisione)</param>
        /// <param name="Texture">Il nome della texture dell'asteroide</param>
        /// <param name="StartRotation">La rotazione di partenza della texture</param>
        public Asteroid(Vector2 Position, Vector2 Direction, int Speed, int Life, string Texture, GameState gameState)
        {
            float StartRotation = Utility.NextRandom(0, (float)Math.PI * 2);
            float RotationSpeed = Utility.NextRandom(-1f, 1f);
            InitializeTimeTraveler(Position, Speed, Texture, gameState, StartRotation, RotationSpeed);            
            direction = Direction;
            life = Life;
        }

        public Asteroid() { }

        /// <summary>
        /// Infligge danno all'asteroide (per esempio quando è colpito da un proiettile)
        /// </summary>
        /// <param name="value">L'entità del danno</param>
        /// <param name="Direction">La direzione da cui arriva la fonte del danno</param>
        public void Damage(int value)
        {
            if (life > 0)
            {
                AddElementRecord(Value => life = (int)Value, life);
                life -= value;
            }
            if (life <= 0)
            {
                lifeState = LifeState.DEAD;
            }
        }

        /// <summary>
        /// Valuta la posizione dell'asteroide
        /// </summary>
        /// <returns>La posizione dell'asteroide</returns>
        public override Vector2 EvaluatePosition(float Delta)
        {
            Vector2 evaluate = new Vector2();
            evaluate.X = startPosition.X + (direction.X * Delta);
            evaluate.Y = startPosition.Y + (direction.Y * Delta);
            return evaluate;
        }

        public override void HasCollided(int Value,object arg)
        {
            this.Damage(Value);
        }

        public override string ToString()
        {
            return "Asteroid - " + lifeState.ToString() ;
        }
    }
}
