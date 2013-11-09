
namespace Continuum.Utilities
{
    /// <summary>
    /// Variabile aleatoria dinamica con distribuzione normale. È dinamica perchè la sua media viene modificata linearmente con il tempo.
    /// </summary>
    public class DynamicNormalRandomVariable : NormalRandomVariable
    {
        public float initialMean;
        public float meanIncrementPerMinute;
        public float max;
        public float min;

        /// <summary>
        /// Costruttore vuoto per serializzazione.
        /// </summary>
        public DynamicNormalRandomVariable()
        {
            this.initialMean = 0;
            this.meanIncrementPerMinute = 0;
            this.max = 0;
            this.min = 0;
        }

        /// <summary>
        /// Crea una nuova variabile aleatoria dinamica con distribuzione normale
        /// </summary>
        /// <param name="initialMean">La media iniziale</param>
        /// <param name="standardDeviation">La deviazione standard</param>
        /// <param name="meanIncrementPerMinute">L'aumento della media per ogni minuti</param>
        /// <param name="max">Il massimo valore restituibile</param>
        /// <param name="min">Il minimo valore restituibile</param>
        public DynamicNormalRandomVariable(float initialMean, float standardDeviation, float? meanIncrementPerMinute, float? max, float? min)
            : base(initialMean, standardDeviation)
        {
            this.initialMean = initialMean;
            this.meanIncrementPerMinute = meanIncrementPerMinute == null ? 0 : meanIncrementPerMinute.Value;
            this.max = max == null ? float.MaxValue : max.Value;
            this.min = min == null ? 0 : min.Value;
        }

        /// <summary>
        /// Aggiorna il valore corrente della media
        /// </summary>
        /// <param name="Delta"></param>
        public void Update(float Delta)
        {
            mean = initialMean + (meanIncrementPerMinute * Delta / 60);
        }

        /// <summary>
        /// Ritorna il prossimo valore casuale con distribuzione normale
        /// </summary>
        /// <returns>Il prossimo valore casuale con distribuzione normale</returns>
        public override float Next()
        {
            float n = base.Next();
            if (n > max)
                n = max;
            else if (n < min)
                n = min;
            return n;
        }
    }
}
