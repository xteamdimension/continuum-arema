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

        private TimeDependentVar maxSimultaneousElements; // Il numero massimo di elementi presenti contemporaneamente.
        private TimeDependentVar maxSecondsWithoutElements; // Il numero massimo di secondi che possono passare senza che siano presenti elementi in campo
        private float aliveElementFoundAt; // L'ultimo momento in cui è stata registrata la presenza di un elemento

        private float aliveElementsCountUpdatedAt; // L'ultimo momento in cui è stato contato il numero di elementi (serve per non ripetere conte inutili)
        private int aliveElementsCount; // Il numero di elementi vivi in questo momento

        /// <summary>
        /// Inizializza il Randomizer
        /// </summary>
        /// <param name="probability">La probabilità che avvenga un lancio per ogni secondo</param>
        /// <param name="probabilityIncrementPerMinute">L'aumento della probabilità per ogni minuto. Per il valore predefinito inserire null.</param>
        /// <param name="probabilityMax">Il massimo valore che può assumere la probabilità. Per il valore predefinito inserire null.</param>
        /// <param name="gameState">Il GameState</param>
        protected void InitializeRandomizer(float probability, float? probabilityIncrementPerMinute, float? probabilityMax, TimeDependentVar maxSimultaneousElements, TimeDependentVar maxSecondsWithoutElements, string texture, GameState gameState)
        {
            this.initialProbability = probability;
            this.probability = initialProbability;
            this.probabilityIncrementPerMinute = probabilityIncrementPerMinute.HasValue ? probabilityIncrementPerMinute.Value : 0;
            this.probabilityMax = probabilityMax.HasValue ? probabilityMax.Value : 1;
            this.texture = texture;
            this.maxSimultaneousElements = maxSimultaneousElements;
            this.maxSecondsWithoutElements = maxSecondsWithoutElements;
            InitializeTimeTraveler(Vector2.Zero, 1, texture, gameState);
            aliveElementFoundAt = 0;
            aliveElementsCountUpdatedAt = -1;
            aliveElementsCount = 0;
            second = 0;
        }

        public override Vector2 EvaluatePosition(float Delta)
        {
            // Aggiorna la variabile dipendente dal tempo che determina il numero massimo di elementi presenti su schermo
            if (maxSecondsWithoutElements != null)
            {
                maxSecondsWithoutElements.Update(Delta);
            }

            // Aggiorna la variabile dipendente dal tempo che determina il numero massimo di elementi presenti su schermo
            if (maxSimultaneousElements != null)
            {
                maxSimultaneousElements.Update(Delta);
            }

            if (Delta > second && gs.levelTime.continuum > 0)
            {
                probability = initialProbability + (probabilityIncrementPerMinute * Delta / 60);
                if (probability > probabilityMax)
                    probability = probabilityMax;
                int launches = TestLaunch();
                if (launches > 0)
                {
                    for (int i = 0; i < launches; i++)
                    {
                        if(maxSimultaneousElements == null || AliveElementsCount(Delta) < maxSimultaneousElements.Value)
                            Launch();
                    }
                }
            }
            second = (int)Delta + 1;

            if (maxSecondsWithoutElements != null)
            {
                if (AliveElementsCount(Delta) > 0)
                {
                    aliveElementFoundAt = Delta;
                }
                if (aliveElementFoundAt > Delta + maxSecondsWithoutElements.Value)
                {
                    Launch(); // Lancia un nuovo nemico se è scaduto il tempo massimo
                    aliveElementFoundAt = Delta;
                }
            }

            return Vector2.Zero;
        }

        /// <summary>
        /// Conta il numero di elementi attualmente in vita
        /// </summary>
        /// <returns>il numero di elementi attualmente in vita</returns>
        private int AliveElementsCount(float Delta)
        {
            // Controlla se il numero di elementi vivi è appena stato contato e decide se contarlo o prendere il valore già salvato
            if(aliveElementsCountUpdatedAt != Delta)
            {
                aliveElementsCount = GetAliveElementsCount();
                aliveElementsCountUpdatedAt = Delta;
            }
            return aliveElementsCount;
        }

        protected abstract int GetAliveElementsCount();

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
