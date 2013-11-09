using Microsoft.Xna.Framework;
using Continuum.State;
using System;
using Continuum.Utilities;

namespace Continuum.Elements
{
    /// <summary>
    /// Crea nuovi oggetti in modo casuale
    /// </summary>
    public abstract class Randomizer : TimeTraveler
    {
        public int second;
        public float initialProbability;
        public float probability;
        public float probabilityIncrementPerMinute;
        public float probabilityMax;
        public string texture;

        /// <summary>
        /// Inizializza il Randomizer
        /// </summary>
        /// <param name="probability">La probabilità che avvenga un lancio per ogni secondo</param>
        /// <param name="probabilityIncrementPerMinute">L'aumento della probabilità per ogni minuto. Per il valore predefinito inserire null.</param>
        /// <param name="probabilityMax">Il massimo valore che può assumere la probabilità. Per il valore predefinito inserire null.</param>
        /// <param name="gameState">Il GameState</param>
        protected void InitializeRandomizer(float probability, float? probabilityIncrementPerMinute, float? probabilityMax, string texture, GameState gameState)
        {
            this.initialProbability = probability;
            this.probability = initialProbability;
            this.probabilityIncrementPerMinute = probabilityIncrementPerMinute.HasValue ? probabilityIncrementPerMinute.Value : 0;
            this.probabilityMax = probabilityMax.HasValue ? probabilityMax.Value : 1;
            this.texture = texture;
            InitializeTimeTraveler(Vector2.Zero, 1, texture, gameState);
            second = 0;
        }

        public override Vector2 EvaluatePosition(float Delta)
        {
            if (Delta > second && gs.levelTime.continuum > 0)
            {
                probability = initialProbability + (probabilityIncrementPerMinute * Delta / 60);
                if (probability > probabilityMax)
                    probability = probabilityMax;
                int launches = TestLaunch();
                if (launches > 0)
                {
                    for(int i=0; i<launches; i++) 
                        Launch();
                }
            }
            second = (int)Delta + 1;

            return Vector2.Zero;
        }

        /// <summary>
        /// Esegue un singolo lancio
        /// </summary>
        protected abstract void Launch();


        /// <summary>
        /// Esegue il test statistico per decidere se eseguire un lancio
        /// </summary>
        /// <returns>Il numero di lanci da eseguire</returns>
        private int TestLaunch()
        {
            float r = Utility.NextRandom(0f,1f);
            float factor = 1;
            int launches = 0;
            for (int i = 0; i < Constants.MAX_RANDOMIZER_LAUNCHES; i++)
            {
                if (r < probability / factor)
                    launches++;
                else
                    break;
                factor *= 4;
            }
            return launches;
        }
    }
}
