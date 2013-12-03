using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Continuum.Utilities
{
    /// <summary>
    /// Variabile dipendente dal tempo. Il suo valore aumenta o diminuisce linearmente in base al tempo corrente.
    /// </summary>
    public class TimeDependentVar
    {
        private float initialValue;
        private float delta;
        private float maxValue;
        private float minValue;
        private float incrementPerMinute;

        /// <summary>
        /// Inizializza una nuova variabile dipendente dal tempo
        /// </summary>
        /// <param name="InitialValue">Il valore iniziale che assume la variabile</param>
        /// <param name="MaxValue">Il massimo valore che può assumere la variabile</param>
        /// <param name="MinValue">Il minimo valore che può assumere la variabile</param>
        /// <param name="IncrementPerMinute">Un incremento al minuto del valore della variabile</param>
        /// <param name="DecrementPerMinute">Un decremento al minuto del valore della variabile</param>
        public TimeDependentVar(float InitialValue, float? MaxValue, float? MinValue, float? IncrementPerMinute, float? DecrementPerMinute)
        {
            this.initialValue = InitialValue;
            this.maxValue = MaxValue.HasValue ? MaxValue.Value : float.MaxValue;
            this.minValue = MinValue.HasValue ? MinValue.Value : 0;
            this.incrementPerMinute = (IncrementPerMinute.HasValue ? IncrementPerMinute.Value : 0) - (DecrementPerMinute.HasValue ? DecrementPerMinute.Value : 0);
        }

        /// <summary>
        /// Il valore della variabile
        /// </summary>
        public float Value
        {
            get
            {
                return Math.Min(Math.Max(initialValue + (incrementPerMinute * delta / 60), minValue), maxValue);
            }
        }

        public void Update(float Delta)
        {
            this.delta = Delta;
        }
    }
}
