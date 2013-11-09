using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;
using Continuum.Management;
using Continuum.State;
using System.Windows;



namespace Continuum
{
    public class InputManager
    {
        public int MaxTouchPoints { get; set; }

        private Vector2? tapVector;
        private Vector2? freeDragVector;
        private Vector2? flickVector;

        private GameState gs;
        private TimeManager timeManager;

        /// <summary>
        /// Indicano le posizioni in cui c'è contatto nello schermo in cui si tocca
        /// </summary>
        public TouchLocation[] locations;

        //Accelerometro
        Accelerometer accelerometer; 
        
        /// <summary>
        /// Dati dell'accelerometro
        /// </summary>
        public Vector3 AccelerometerReading { get; private set; }
        
        /// <summary>
        /// Coordinate di riferimento dello zero corrente dell'accelerometro
        /// </summary>
        public Vector3 accelerometerCurrentZero;

        bool firstValueAccelerometer;

        /// <summary>
        /// Vettore di correzione delle coordinate dell'accelerometro
        /// </summary>
        public Vector3 AccelerometerReadingCorrected
        {
            get
            {
                Vector3 v = Vector3.Zero;
                if (AccelerometerReading.X > accelerometerCurrentZero.X)
                {
                    v.X = (AccelerometerReading.X - accelerometerCurrentZero.X) / (1 - accelerometerCurrentZero.X);
                }
                else
                {
                    v.X = (AccelerometerReading.X - accelerometerCurrentZero.X) / (1 + accelerometerCurrentZero.X);
                }

                if (AccelerometerReading.Y > accelerometerCurrentZero.Y)
                {
                    v.Y = (AccelerometerReading.Y - accelerometerCurrentZero.Y) / (1 - accelerometerCurrentZero.Y);
                }
                else
                {
                    v.Y = (AccelerometerReading.Y - accelerometerCurrentZero.Y) / (1 + accelerometerCurrentZero.Y);
                }

                if (AccelerometerReading.Z > accelerometerCurrentZero.Z)
                {
                    v.Z = (AccelerometerReading.Z - accelerometerCurrentZero.Z) / (1 - accelerometerCurrentZero.Z);
                }
                else
                {
                    v.Z = (AccelerometerReading.Z - accelerometerCurrentZero.Z) / (1 + accelerometerCurrentZero.Z);
                }
                return v;
            }
            private set{}
        }


        public InputManager(GameState gs, TimeManager tm)
        {
            this.gs = gs;
            timeManager = tm;

            gs.firstFinger = null;
            gs.secondFinger = null;

            TouchPanelCapabilities panelCapabilities = TouchPanel.GetCapabilities();
            
            if (!panelCapabilities.IsConnected)
                throw new Exception("This is not supposed to happen. Screen is not connected!");

            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.FreeDrag | GestureType.Flick;

            locations = new TouchLocation[panelCapabilities.MaximumTouchCount];
            
            tapVector = null;
            freeDragVector = null;
            flickVector = null;

            accelerometer = new Accelerometer();
            AccelerometerReading = Vector3.Zero;
            accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<Microsoft.Devices.Sensors.AccelerometerReading>>(accelerometer_CurrentValueChanged);
            accelerometer.Start();
            firstValueAccelerometer = false;
        }

        public void RecalibrateAccelerometer(Vector3 newCoordinates)
        {
            accelerometerCurrentZero = newCoordinates; 
        }
        public void RecalibrateAccelerometer()
        {
                accelerometerCurrentZero = AccelerometerReading;
        }

        private void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            AccelerometerReading = e.SensorReading.Acceleration;
            if (!firstValueAccelerometer)
            {
                firstValueAccelerometer = true;
                RecalibrateAccelerometer();
            }
        }

        /// <summary>
        /// Stoppa l'accelerometro (pausa)
        /// </summary>
        public void AccelerometerStop() {
            accelerometer.Stop();
        }

        /// <summary>
        /// Start accelerometro
        /// </summary>
        public void AccelerometerStart()
        {
            accelerometer.Start();
        }

        /// <summary>
        /// Aggiorna i rilevamenti. Richiama questo metodo prima di controllare qualsiasi altra cosa.
        /// </summary>
        public void Update()
        {
            //Update Gestures predefinite
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gestSample = TouchPanel.ReadGesture();

                switch (gestSample.GestureType)
                {
                    case GestureType.Tap:
                        tapVector = gestSample.Position;
                        break;
                    case GestureType.FreeDrag:
                        freeDragVector = gestSample.Position;
                        break;
                    case GestureType.Flick:
                        flickVector = gestSample.Delta;
                        break;
                }
            }

            if (gs.PerformMultitouchPlasmaGranade)
                UpdateMultitouchPlasmaGranade();

            if (gs.PerformGestureFlickDownRewindTime)
            {
                if (isFlickReadable())
                {
                    if (FlickDown())
                        timeManager.Back();
                }
            }

            if (gs.PerformGestureFlickDownStopRewind)
            {
                if (isTapReadable())
                {
                    timeManager.Stop();
                }
            }
        }

        private void UpdateMultitouchPlasmaGranade()
        {
            TouchCollection collection = TouchPanel.GetState();
            locations = new TouchLocation[collection.Count];
            for (int counter = 0; counter < collection.Count; counter++)
                locations[counter] = collection[counter];

            //Nessun dito ancora premuto
            if (gs.firstFinger == null && gs.secondFinger == null)
            {
                //Se è appena stato premuto il primo dito
                if (locations.Length > 0)
                {
                    gs.firstFinger = locations[0].Id;
                    gs.firstFingerPosition = locations[0].Position; //Salva la posizione
                    timeManager.FirstFinger();  //Segnala il primo dito
                    
                    //Se contemporaneamente è stato premuto anche il secondo dito
                    if (locations.Length > 1)
                    {
                        gs.secondFinger = locations[1].Id;
                        gs.secondFingerPosition = locations[1].Position;    //Salva la posizione
                        timeManager.SecondFinger(); //Segnala il secondo dito
                    }
                }
            }

            //Il primo dito già premuto, il secondo dito non ancora premuto
            if (gs.firstFinger != null && gs.secondFinger == null)
            {
                //Se è stato rilasciato il primo dito
                if (locations.Length == 0)
                {
                    gs.firstFinger = null;
                    timeManager.UndoFirstFinger();  //Annulla il primo dito
                }
                //Se c'è un solo dito premuto
                else if (locations.Length == 1)
                {
                    TouchLocation loc;
                    //Se il dito premuto è lo stesso di prima
                    if (collection.FindById(gs.firstFinger.Value, out loc))
                    {
                        gs.firstFingerPosition = loc.Position;  //Aggiorna la posizione
                    }
                    //Se il dito premuto non è lo stesso di prima
                    else
                    {
                        gs.firstFinger = locations[0].Id;  
                        gs.firstFingerPosition = locations[0].Position; //Salva la posizione
                    }
                }
                //Se è stato premuto almeno un altro dito in più
                else if (locations.Length > 1)
                {
                    TouchLocation loc;
                    //Se il primo dito è tra quelli premuti
                    if (collection.FindById(gs.firstFinger.Value, out loc))
                    {
                        int firstFingerLocationsIndex = -1;
                        int secondFingerLocationsIndex = -1;
                        for (int i = 0; i < locations.Length; i++)
                        {
                            if (locations[i].Id == gs.firstFinger.Value)   //Trova l'indice del primo dito nell'array di tocchi
                            {
                                firstFingerLocationsIndex = i;
                                if (secondFingerLocationsIndex != -1)
                                    break;
                            }
                            else
                            {
                                secondFingerLocationsIndex = i;
                                if (firstFingerLocationsIndex != -1)    //Seleziona come secondo dito il primo tocco != dal primo dito
                                    break;
                            }
                        }
                        gs.firstFinger = locations[firstFingerLocationsIndex].Id;
                        gs.secondFinger = locations[secondFingerLocationsIndex].Id;
                        gs.firstFingerPosition = locations[firstFingerLocationsIndex].Position; //Aggiorna la posizione del primo dito
                        gs.secondFingerPosition = locations[secondFingerLocationsIndex].Position;   //Salva la posizione del secondo dito
                        timeManager.SecondFinger(); //Segnala il secondo dito
                    }
                    //Se il primo dito non è tra quelli premuti
                    else
                    {
                        gs.firstFinger = locations[0].Id;
                        gs.secondFinger = locations[1].Id;
                        gs.firstFingerPosition = locations[0].Position; //Salva la posizione del primo dito
                        gs.secondFingerPosition = locations[1].Position;    //Salva la posizione del secondo dito
                        timeManager.SecondFinger(); //Segnala il secondo dito
                    }
                }
            }
            //Entrambe le dita già premute
            else if (gs.firstFinger != null && gs.secondFinger != null)
            {
                //Se ci sono meno di due dita premute
                if (locations.Length < 2){
                    timeManager.LaunchPlasmaGranade();  //Lancia la granata
                    gs.firstFinger = null;
                    gs.secondFinger = null;
                }
                else if (locations.Length >= 2)
                {
                    TouchLocation loc1, loc2;
                    //Se entrambe le dita sono ancora premute
                    if (collection.FindById(gs.firstFinger.Value, out loc1) && collection.FindById(gs.secondFinger.Value, out loc2))
                    {
                        gs.firstFingerPosition = loc1.Position; //Aggiorna la posizione del primo dito
                        gs.secondFingerPosition = loc2.Position;    //Aggiorna la posizione del secondo dito
                    }
                    //Se almeno un dito premuto prima non è più premuto
                    else
                    {
                        timeManager.LaunchPlasmaGranade();  //Lancia la granata
                        gs.firstFinger = null;
                        gs.secondFinger = null;
                    }
                }
            }
        }

        /// <summary>
        /// Indica se si è verificato un TAP, e se è possibile leggerne i dati.
        /// </summary>
        /// <returns>True se c'è un tap leggibile, false altrimenti</returns>
        public bool isTapReadable()
        {
            return tapVector != null ? true : false;
        }

        /// <summary>
        /// Rileva i dati del TAP verificatosi. Da richiamare solo se isTapReadable() restituisce true.
        /// </summary>
        /// <returns>Vector2 indicante la posizione del TAP.</returns>
        public Vector2 Tap()
        {
            if (tapVector == null)
                throw new Exception("Tap non disponibile. Richiamare isTapReadable prima di acquisire il dato");
            Vector2? returnValue = tapVector;
            flickVector = null;
            freeDragVector = null;
            tapVector = null;
            return returnValue.Value;
        }

        /// <summary>
        /// Indica se si è verificato un FREEDRAG, e se è possibile leggerne i dati.
        /// </summary>
        /// <returns></returns>
        public bool isFreeDragReadable()
        {
            return freeDragVector != null ? true : false;
        }

        /// <summary>
        /// Rileva i dati del FREEDRAG verificatosi. Da richiamare solo se isFreeDragReadable() restituisce true.
        /// </summary>
        /// <returns>Vector2 indicante la posizione del FREEDRAG nel momento in cui lo si legge.</returns>
        public Vector2 FreeDrag()
        {
            if(freeDragVector == null)
                throw new Exception("FreeDrag non disponibile. Richiamare isFreeDragReadable prima di acquisire il dato");
            Vector2? returnValue = freeDragVector;
            flickVector = null;
            freeDragVector = null;
            tapVector = null;
            return returnValue.Value;
        }

        /// <summary>
        /// Indica se si è verificata una Drag Down
        /// </summary>
        /// <returns></returns>
        public bool isFlickReadable()
        {
            return flickVector != null ? true : false;            
        }

        /// <summary>
        /// Riconosce se è stata eseguita la Gesture Flick Down
        /// </summary>
        /// <returns>Se è stata riconosciuta o meno la gesture Grag Down</returns>
        public bool FlickDown()
        {
            bool retval = flickVector.Value.Y > 0;
            flickVector = null;
            freeDragVector = null;
            tapVector = null;
            return retval;
        }
    }


}
