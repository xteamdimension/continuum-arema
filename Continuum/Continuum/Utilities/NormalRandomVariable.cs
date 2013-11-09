using System;

namespace Continuum.Utilities
{
    /// <summary>
    /// Variabile aleatoria con distribuzione normale
    /// </summary>
    public class NormalRandomVariable
    {
        public double mean;
        public double standardDeviation;

        /// <summary>
        /// Costruttore vuoto per serializzazione.
        /// </summary>
        public NormalRandomVariable()
        {
            this.mean = 0;
            this.standardDeviation = 0;
        }

        public NormalRandomVariable(float mean, float standardDeviation)
        {
            this.mean = mean;
            this.standardDeviation = standardDeviation;
        }

        public virtual float Next()
        {
            double u1 = Utility.NextRandom(0d, 1d);
            double u2 = Utility.NextRandom(0d, 1d);
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mean + standardDeviation * randStdNormal;
            return (float)randNormal;
        }
    }
}
