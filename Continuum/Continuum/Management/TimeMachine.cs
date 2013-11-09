using Microsoft.Xna.Framework;
using Continuum.Utilities;

namespace Continuum.Management
{
    /// <summary>
    /// Un flusso temporale
    /// </summary>
    public class TimeMachine
    {
        /// <summary>
        /// Il valore attuale del tempo, in secondi
        /// </summary>
        public float time;

        /// <summary>
        /// L'andamento del tempo (derivata)
        /// </summary>
        public float continuum;

        /// <summary>
        /// Il tempo trascorso dall'ultimo aggiornamento della time machine.
        /// </summary>
        public float elapsedContinuumTime;

        public TimeMachine()
        {
            continuum = Constants.CONTINUUM_MAX;
            time = 0;
        }

        public void Update(float TotalElapsedSeconds)
        {
            elapsedContinuumTime = TotalElapsedSeconds * continuum;
            time += elapsedContinuumTime;
        }
    }
}
