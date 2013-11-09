using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Continuum.Utilities
{
    /// <summary>
    /// Tiene in memoria un singolo cambiamento di variabile di un TimeTraveler, e permette il ripristino tornando indietro nel tempo.
    /// </summary>
    public class ElementRecord
    {
        public float Time;
        private RewindMethod rewind;
        public object Value { get; private set; }
        
        /// <summary>
        /// Crea un'istanza della classe ElementRecord, memorizzando un metodo da invocare quando il TimeTraveler torna indietro nel tempo.
        /// </summary>
        /// <param name="Time">Il tempo della TimeMachine in cui avviene il cambiamento di variabile</param>
        /// <param name="Rewind">Il metodo da invocare per ripristinare la variabile al suo valore precedente</param>
        /// <param name="Value">Il valore della variabile, prima che venga cambiata</param>
        public ElementRecord(float Time, RewindMethod Rewind, object Value){
            this.Time = Time;
            this.rewind = Rewind;
            this.Value = Value;
        }

        /// <summary>
        /// Esegue il ripristino dello stato.
        /// </summary>
        public void Rewind()
        {
            rewind(Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
