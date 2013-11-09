using Continuum.State;
using Continuum.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Continuum.Management
{
    /// <summary>
    /// Gestisce il tempo del gioco.
    /// </summary>
    public class TimeManager
    {
        private GameState gs;

        private float beginRewindContinuumLevel;
        private float continuumLeanLevel;
        private float continuumMinLevel;
        private float timerStartTime;
        private float timerElapsedTime;
        private float timerTargetDuration = -1;
        private LinkedList<ElementRecord> ElementRecords;
        private float maxvaltimetank;
        private QuadraticBezierCurve path;
        private float rotation;

        //DEBUG
        private float time_tank_timer;
        private bool time_tank_timer_start = false;

        /// <summary>
        /// Rotazione generica.
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
        }

        public QuadraticBezierCurve Path
        {
            get { return path; }
        }

        /// <summary>
        /// Valore che cresce da 0 a 1, dove 0 è il momento di inizializzazione di uno stato, e 1 è il momento di terminazione dello stesso.
        /// </summary>
        public float NormalizedDelta
        {
            get
            {
                return timerElapsedTime / timerTargetDuration;
            }
        }

        /// <summary>
        /// Proprietà di supporto. Restituisce true se si è in uno stato in cui c'è necessità di disegnare l'effetto delle linee.
        /// </summary>
        public bool DrawLines
        {
            get
            {
                return State == TimeState.CALIBRATION_PLASMA_GRANADE_PATH_CONTROL_POINT || State == TimeState.CALIBRATION_PLASMA_GRANADE_PATH_TARGET_POINT || State == TimeState.ENTER_PLASMA_GRANADE_LAUNCHER || State == TimeState.EXIT_PLASMA_GRANADE_LAUNCHER || State == TimeState.PLASMA_GRANADE_LAUNCHER ? true : false;
            }
        }

        public TimeState State
        {
            get
            {
                return gs.timeState;
            }

            private set
            {
                switch (gs.timeState)
                {
                    case TimeState.START_GAME_STATE:
                        if (value == TimeState.FORWARD)
                        {
                            gs.timeState = value;
                            StartGameStateInit();  //da fare
                            ForwardInit();
                        }
                        break;
                    case TimeState.FORWARD:
                        if (value == TimeState.BEGIN_REWIND)
                        {
                            gs.timeState = value;
                            BeginRewindInit();
                        }
                        else if (value == TimeState.BOOSTER)
                        {
                            gs.timeState = value;
                            BoosterInit();      //da fare
                        }
                        else if (value == TimeState.ENTER_PLASMA_GRANADE_LAUNCHER && gs.PlayerGranadeCount > 0)
                        {
                            gs.timeState = value;
                            EnterPlasmaGranadeLauncherInit();
                        }
                        break;
                    case TimeState.BEGIN_REWIND:
                        if (value == TimeState.START_REWIND)
                        {
                            gs.timeState = value;
                            StartRewindInit();
                        }
                        break;
                    case TimeState.START_REWIND:
                        if (value == TimeState.REWIND)
                        {
                            gs.timeState = value;
                            RewindInit();
                        }
                        break;
                    case TimeState.REWIND:
                        if (value == TimeState.BEGIN_FORWARD)
                        {
                            gs.timeState = value;
                            BeginForwardInit();
                        }
                        break;
                    case TimeState.BEGIN_FORWARD:
                        if (value == TimeState.START_FORWARD)
                        {
                            gs.timeState = value;
                            StartForwardInit();
                        }
                        break;
                    case TimeState.START_FORWARD:
                        if (value == TimeState.FORWARD)
                        {
                            gs.timeState = value;
                            ForwardInit();
                        }
                        break;
                    case TimeState.BOOSTER:
                        if (value == TimeState.FORWARD)
                        {
                            gs.timeState = value;
                            ForwardInit();
                        }
                        break;
                    case TimeState.ENTER_PLASMA_GRANADE_LAUNCHER:
                        if (value == TimeState.PLASMA_GRANADE_LAUNCHER)
                        {
                            gs.timeState = value;
                            PlasmaGranadeLauncherInit();
                        }
                        break;
                    case TimeState.PLASMA_GRANADE_LAUNCHER:
                        if (value == TimeState.EXIT_PLASMA_GRANADE_LAUNCHER)
                        {
                            gs.timeState = value;
                            ExitPlasmaGranadeLauncherInit();
                        }
                        else if (value == TimeState.CALIBRATION_PLASMA_GRANADE_PATH_TARGET_POINT)
                        {
                            gs.timeState = value;
                            CalibrationPlasmaGranadePathTargetPointInit();
                        }
                        break;
                    case TimeState.CALIBRATION_PLASMA_GRANADE_PATH_TARGET_POINT:
                        if (value == TimeState.CALIBRATION_PLASMA_GRANADE_PATH_CONTROL_POINT)
                        {
                            gs.timeState = value;
                            CalibrationPlasmaGranadePathControlPointInit();
                        }
                        else if (value == TimeState.PLASMA_GRANADE_LAUNCHER)
                        {
                            gs.timeState = value;
                            PlasmaGranadeLauncherInit();
                        }
                        break;
                    case TimeState.CALIBRATION_PLASMA_GRANADE_PATH_CONTROL_POINT:
                        if (value == TimeState.EXIT_PLASMA_GRANADE_LAUNCHER)
                        {
                            gs.timeState = value;
                            gs.newPlasmaGranade(path);
                            gs.PlayerGranadeCount--;
                            ExitPlasmaGranadeLauncherInit();
                        }
                        break;
                    case TimeState.EXIT_PLASMA_GRANADE_LAUNCHER:
                        if (value == TimeState.FORWARD)
                        {
                            gs.timeState = value;
                            ForwardInit();
                        }
                        break;
                }
            }
        }

        public TimeManager(GameState gs)
        {
            this.gs = gs;
            ElementRecords = new LinkedList<ElementRecord>();
            gs.timeState = TimeState.FORWARD;
        }

        private void StartGameStateInit()
        {
            ResetTimer(Utilities.Constants.INITIAL_BLACK_DURATION + Utilities.Constants.INITIAL_FADE_DURATION);
        }

        private void CalibrationPlasmaGranadePathControlPointInit()
        {
            path = new QuadraticBezierCurve(gs.playerPosition, gs.firstFingerPosition, gs.secondFingerPosition);
        }

        private void CalibrationPlasmaGranadePathTargetPointInit()
        {
            ResetTimer();
        }

        private void EnterPlasmaGranadeLauncherInit()
        {
            ResetTimer(Constants.PLASMA_GRANADE_LAUNCHER_ENTERING_EXITING_MODE_TIME);

            float lineDistance = (float)Constants.SCREEN_WIDTH / (float)Constants.GRID_COLUMNS;
            for (int i = 0; i < gs.gridVerticalPoints.Length; i++)
            {
                float x = lineDistance * (i);
                if (x <= 0) x = 1;
                if (i % 2 == 0)
                {
                    gs.gridVerticalPoints[i][0] = new Vector2(x, 0);
                    gs.gridVerticalPoints[i][1] = new Vector2(x, 0);
                }
                else
                {
                    gs.gridVerticalPoints[i][0] = new Vector2(x, Constants.SCREEN_HEIGHT);
                    gs.gridVerticalPoints[i][1] = new Vector2(x, Constants.SCREEN_HEIGHT);
                }
            }
            lineDistance = (float)Constants.SCREEN_HEIGHT / (float)Constants.GRID_ROWS;
            for (int i = 0; i < gs.gridHorizontalPoints.Length; i++)
            {
                float y = lineDistance * (i);
                if (y >= Constants.SCREEN_HEIGHT) y = Constants.SCREEN_HEIGHT - 1;
                if (i % 2 == 0)
                {
                    gs.gridHorizontalPoints[i][0] = new Vector2(0, y);
                    gs.gridHorizontalPoints[i][1] = new Vector2(0, y);
                }
                else
                {
                    gs.gridHorizontalPoints[i][0] = new Vector2(Constants.SCREEN_WIDTH, y);
                    gs.gridHorizontalPoints[i][1] = new Vector2(Constants.SCREEN_WIDTH, y);
                }
            }
        }

        private void PlasmaGranadeLauncherInit()
        {
            gs.firstFinger = null;
            gs.secondFinger = null;
            ResetTimer(Constants.PLASMA_GRANADE_TIME_OUT);
        }

        private void ExitPlasmaGranadeLauncherInit()
        {
            ResetTimer(Utilities.Constants.PLASMA_GRANADE_LAUNCHER_ENTERING_EXITING_MODE_TIME);
        }

        private void BoosterInit()
        {
            throw new NotImplementedException();
            //da fare
        }

        private void ForwardInit()
        {
            gs.playerTime.continuum = Constants.CONTINUUM_MAX;
            gs.levelTime.continuum = TachyonStreamSlowDown();
        }

        private void StartForwardInit()
        {
            ResetTimer();
        }

        private void BeginForwardInit()
        {
            ResetTimer();
            continuumMinLevel = gs.levelTime.continuum;
            continuumLeanLevel = (gs.levelTime.continuum / Constants.CONTINUUM_MIN) * Constants.CONTINUUM_LEAN;
        }

        private void RewindInit()
        {
            //Constants.TIME_TANK_CRITICAL_VALUE = maxvaltimetank - gs.timeTank; //da cambiare la time tank va sotto zero ricorda di cambiare il descrittore static in const nella classe Constants quando hai finito di fare esperimenti
            gs.playerTime.continuum = Constants.CONTINUUM_MIN;
            gs.levelTime.continuum = continuumMinLevel;
            time_tank_timer_start = false;
            Constants.TIME_TANK_CRITICAL_VALUE = time_tank_timer + 0.6f;
        }

        private void StartRewindInit()
        {
            maxvaltimetank = gs.timeTank; //da cambiare la time tank va sotto zero
            ResetTimer();
        }

        private void BeginRewindInit()
        {
            ResetTimer();
            beginRewindContinuumLevel = gs.levelTime.continuum;
            continuumLeanLevel = (gs.levelTime.continuum / Constants.CONTINUUM_MAX) * Constants.CONTINUUM_LEAN;
            continuumMinLevel = (gs.levelTime.continuum / Constants.CONTINUUM_MAX) * Constants.CONTINUUM_MIN;
        }

        public void Update(GameTimerEventArgs e)
        {
            gs.albertTime.Update((float)e.ElapsedTime.TotalSeconds);
            if (time_tank_timer_start)
                time_tank_timer += gs.albertTime.elapsedContinuumTime;
            // Transizioni
            switch (State)
            {
                case TimeState.START_GAME_STATE:
                    //da fare
                    break;
                case TimeState.BEGIN_REWIND:
                    if (gs.playerTime.continuum == 0)
                        State = TimeState.START_REWIND;
                    break;
                case TimeState.START_REWIND:
                    if (gs.playerTime.continuum == Constants.CONTINUUM_MIN)
                        State = TimeState.REWIND;
                    break;
                case TimeState.REWIND:
                    if (gs.timeTank <= Constants.TIME_TANK_CRITICAL_VALUE)
                        State = TimeState.BEGIN_FORWARD;
                    break;
                case TimeState.BEGIN_FORWARD:
                    if (gs.playerTime.continuum == 0)
                        State = TimeState.START_FORWARD;
                    break;
                case TimeState.START_FORWARD:
                    if (gs.playerTime.continuum == Constants.CONTINUUM_MAX)
                        State = TimeState.FORWARD;
                    break;
                case TimeState.BOOSTER:
                    //da fare
                    break;
                case TimeState.ENTER_PLASMA_GRANADE_LAUNCHER:
                    if (NormalizedDelta > 1)
                        State = TimeState.PLASMA_GRANADE_LAUNCHER;
                    break;
                case TimeState.PLASMA_GRANADE_LAUNCHER:
                    if (NormalizedDelta > 1)
                        State = TimeState.EXIT_PLASMA_GRANADE_LAUNCHER;
                    break;
                case TimeState.CALIBRATION_PLASMA_GRANADE_PATH_TARGET_POINT:
                    //da fare
                    break;
                case TimeState.CALIBRATION_PLASMA_GRANADE_PATH_CONTROL_POINT:
                    //da fare
                    break;
                case TimeState.EXIT_PLASMA_GRANADE_LAUNCHER:
                    if (NormalizedDelta > 1)
                        State = TimeState.FORWARD;
                    break;

            }
            switch (State)
            {
                case TimeState.START_GAME_STATE:
                    StartGameState();  //da fare
                    break;
                case TimeState.FORWARD:
                    Forward();
                    break;
                case TimeState.BEGIN_REWIND:
                    BeginRewind();
                    break;
                case TimeState.START_REWIND:
                    StartRewind();
                    break;
                case TimeState.REWIND:
                    Rewind();
                    break;
                case TimeState.BEGIN_FORWARD:
                    BeginForward();
                    break;
                case TimeState.START_FORWARD:
                    StartForward();
                    break;
                case TimeState.BOOSTER:
                    Booster(); //da fare
                    break;
                case TimeState.ENTER_PLASMA_GRANADE_LAUNCHER:
                    Forward();
                    EnterPlasmaGranadeLauncher();
                    break;
                case TimeState.PLASMA_GRANADE_LAUNCHER:
                    Forward();
                    PlasmaGranadeLauncher(); 
                    break;
                case TimeState.CALIBRATION_PLASMA_GRANADE_PATH_TARGET_POINT:
                    Forward();
                    CalibrationPlasmaGranadePathTargetPoint(); 
                    break;
                case TimeState.CALIBRATION_PLASMA_GRANADE_PATH_CONTROL_POINT:
                    Forward();
                    CalibrationPlasmaGranadePathControlPoint(); 
                    break;
                case TimeState.EXIT_PLASMA_GRANADE_LAUNCHER:
                    Forward();
                    ExitPlasmaGranadeLauncher();
                    break;
            }
            gs.playerTime.Update(gs.albertTime.elapsedContinuumTime);
            gs.levelTime.Update(gs.albertTime.elapsedContinuumTime);
        }

        private void StartGameState()
        {
            throw new NotImplementedException();
            //da fare
        }

        private void CalibrationPlasmaGranadePathControlPoint()
        {
            path.startPoint = gs.playerPosition;
            path.targetPoint = gs.firstFingerPosition;
            path.controlPoint = gs.secondFingerPosition;
            UpdateTimer();
            rotation = timerElapsedTime * Constants.SCOPE_ROTATION_SPEED;
        }

        private void CalibrationPlasmaGranadePathTargetPoint()
        {
            UpdateTimer();
            rotation = timerElapsedTime * Constants.SCOPE_ROTATION_SPEED;
        }

        private void ExitPlasmaGranadeLauncher()
        {
            UpdateTimer();
            float delta = NormalizedDelta * Constants.SCREEN_HEIGHT;
            for (int i = 0; i < gs.gridVerticalPoints.Length; i++)
            {
                if (i % 2 != 0)
                {
                    gs.gridVerticalPoints[i][1].Y = delta;
                }
                else
                {
                    gs.gridVerticalPoints[i][1].Y = Constants.SCREEN_HEIGHT - delta;
                }
            }
            delta = NormalizedDelta * Constants.SCREEN_WIDTH;
            for (int i = 0; i < gs.gridHorizontalPoints.Length; i++)
            {
                if (i % 2 != 0)
                {
                    gs.gridHorizontalPoints[i][1].X = delta;
                }
                else
                {
                    gs.gridHorizontalPoints[i][1].X = Constants.SCREEN_WIDTH - delta;
                }
            }
        }

        private void PlasmaGranadeLauncher()
        {
            UpdateTimer();        
        }

        private void EnterPlasmaGranadeLauncher()
        {
            UpdateTimer();
            float delta = NormalizedDelta * Constants.SCREEN_HEIGHT;
            for (int i = 0; i < gs.gridVerticalPoints.Length; i++)
            {
                if (i % 2 == 0)
                {
                    gs.gridVerticalPoints[i][1].Y = delta;
                }
                else
                {
                    gs.gridVerticalPoints[i][1].Y = Constants.SCREEN_HEIGHT - delta;
                }
            }
            delta = NormalizedDelta * Constants.SCREEN_WIDTH;
            for (int i = 0; i < gs.gridHorizontalPoints.Length; i++)
            {
                if (i % 2 == 0)
                {
                    gs.gridHorizontalPoints[i][1].X = delta;
                }
                else
                {
                    gs.gridHorizontalPoints[i][1].X = Constants.SCREEN_WIDTH - delta;
                }
            }
        }

        private void Booster()
        {
            throw new NotImplementedException();
            //da fare
        }

        private void StartForward()
        {
            if (ElementRecords.Last != null && gs.playerTime.time - ElementRecords.Last.Value.Time > gs.timeTank)
                ElementRecords.RemoveLast();

            UpdateTimer();
            gs.playerTime.continuum = Math.Min(0 + timerElapsedTime * Constants.CONTINUUM_LEAN, Constants.CONTINUUM_MAX);

            AddElementRecord(Value => gs.levelTime.continuum = gs.playerTime.continuum * (float)Value, gs.levelTime.continuum / gs.playerTime.continuum);
            gs.levelTime.continuum = Math.Min(0 + timerElapsedTime * Constants.CONTINUUM_LEAN, TachyonStreamSlowDown());
        }

        private void BeginForward()
        {
            UpdateTimer();
            gs.playerTime.continuum = Math.Min(Constants.CONTINUUM_MIN + timerElapsedTime * Constants.CONTINUUM_LEAN, 0);
            gs.levelTime.continuum = Math.Min(continuumMinLevel + timerElapsedTime * continuumLeanLevel, 0);
            gs.timeTank += gs.levelTime.elapsedContinuumTime;
        }

        private void Rewind()
        {
            while (ElementRecords.First != null && ElementRecords.First.Value.Time > gs.playerTime.time)
            {
                ElementRecords.First.Value.Rewind();
                ElementRecords.RemoveFirst();
            }
            gs.timeTank += gs.playerTime.elapsedContinuumTime;
        }

        private void StartRewind()
        {
            UpdateTimer();
            gs.playerTime.continuum = Math.Max(0 - timerElapsedTime * Constants.CONTINUUM_LEAN, Constants.CONTINUUM_MIN);
            gs.levelTime.continuum = Math.Max(0 - timerElapsedTime * continuumLeanLevel, continuumMinLevel);
            gs.timeTank += gs.levelTime.elapsedContinuumTime;
            time_tank_timer = 0;
            time_tank_timer_start = true;
        }

        private void BeginRewind()
        {
            UpdateTimer();
            gs.playerTime.continuum = Math.Max(Constants.CONTINUUM_MAX - (timerElapsedTime * Constants.CONTINUUM_LEAN), 0);
            gs.levelTime.continuum = Math.Max(beginRewindContinuumLevel - (timerElapsedTime * continuumLeanLevel), 0);
        }

        private void Forward()
        {
            if (ElementRecords.Last != null && gs.playerTime.time - ElementRecords.Last.Value.Time > gs.timeTank)
                ElementRecords.RemoveLast();

            gs.playerTime.continuum = Constants.CONTINUUM_MAX;

            //if (TachyonStreamSlowDown() != gs.levelTime.continuum)
            AddElementRecord(Value => gs.levelTime.continuum = gs.playerTime.continuum * (float)Value, gs.levelTime.continuum / gs.playerTime.continuum);
            gs.levelTime.continuum = TachyonStreamSlowDown();
            
        }

        /// <summary>
        /// Modifica del continuum del livello quando il player si trova in un TachyonStream.
        /// </summary>
        /// <param name="distanceFromStream">Distanza in valore assoluto dal centro del tachyon stream. Non deve superare TACHYON_STREAM_WIDTH / 2.</param>
        public float TachyonStreamSlowDown()
        {
            if (gs.tachyonStream != null && gs.tachyonStream.lifeState == LifeState.NORMAL)
            {
                float playerDistance = Math.Abs(gs.playerPosition.X - gs.tachyonStream.CurrentPosition.X);
                if (playerDistance <= Constants.TACHYON_STREAM_WIDTH / 2)
                {
                    return (playerDistance * 2 * (Constants.CONTINUUM_MAX - Constants.MIN_CONTINUUM_SLOW_DOWN)) / (Constants.TACHYON_STREAM_WIDTH) + Constants.MIN_CONTINUUM_SLOW_DOWN;
                }
            }
            return Constants.CONTINUUM_MAX;
        }

        /// <summary>
        /// Registra un metodo che verrà invocato tornando indietro nel tempo.
        /// Questo metodo servirà per ripristinare il valore della variabile che sta per essere modificata.
        /// </summary>
        /// <param name="Rewind">Il metodo che verrà invocato tornando indietro nel tempo. Inizializzarlo così: "Value => [ istruzioni ]"</param>
        /// <param name="Value">Il valore della variabile prima che sia modificata.</param>
        private void AddElementRecord(RewindMethod Rewind, object Value)
        {
            if (gs.playerTime.continuum > 0)
                ElementRecords.AddFirst(new ElementRecord(gs.playerTime.time, Rewind, Value));
        }

        /// <summary>
        /// Inizia a tornare indietro nel tempo (se possibile)
        /// </summary>
        public void Back()
        {
            if (gs.timeTank > Constants.TIME_TANK_CRITICAL_VALUE)
                State = TimeState.BEGIN_REWIND;
        }

        /// <summary>
        /// Interrompe il riavvolgimento del tempo (se possibile)
        /// </summary>
        public void Stop()
        {
            State = TimeState.BEGIN_FORWARD;
        }

        /// <summary>
        /// Attiva il Boost (se possibile)
        /// </summary>
        public void ActivateBooster()
        {
            if (State == TimeState.FORWARD && gs.timeTank == Constants.MAX_TIME_TANK_VALUE)
            {
                State = TimeState.BOOSTER;
            }
        }

        /// <summary>
        /// Esce dalla modalità di lancio delle granate al plasma.
        /// </summary>
        public void DeactivatePlasmaGranadeLauncher()
        {
            if (State == TimeState.PLASMA_GRANADE_LAUNCHER)
            {
                State = TimeState.EXIT_PLASMA_GRANADE_LAUNCHER;
            }
        }

        /// <summary>
        /// Entra nella modalità di lancio granate al plasma (se possibile).
        /// </summary>
        public void ActivatePlasmaGranadeLauncher()
        {
            if (State == TimeState.FORWARD)
            {
                State = TimeState.ENTER_PLASMA_GRANADE_LAUNCHER;
            }
        }

        /// <summary>
        /// Resetta il timer
        /// </summary>
        private void ResetTimer()
        {
            timerStartTime = gs.albertTime.time;
            timerElapsedTime = 0;
            timerTargetDuration = -1;
        }

        /// <summary>
        /// Resetta il timer, impostando una durata per lo stato.
        /// </summary>
        /// <param name="Duration">La durata prefissata per lo stato</param>
        private void ResetTimer(float Duration)
        {
            timerStartTime = gs.albertTime.time;
            timerElapsedTime = 0;
            timerTargetDuration = Duration;
        }

        /// <summary>
        /// Aggiorna il timer
        /// </summary>
        private void UpdateTimer()
        {
            timerElapsedTime = gs.albertTime.time - timerStartTime;
        }

        /// <summary>
        /// Rileva l'input di un dito nella modalità di mira
        /// </summary>
        public void FirstFinger()
        {
            State = TimeState.CALIBRATION_PLASMA_GRANADE_PATH_TARGET_POINT;
        }

        /// <summary>
        /// Annulla l'input del primo dito nella modalità di mira
        /// </summary>
        public void UndoFirstFinger()
        {
            if(State != TimeState.ENTER_PLASMA_GRANADE_LAUNCHER)
                State = TimeState.PLASMA_GRANADE_LAUNCHER;
        }

        /// <summary>
        /// Rileva l'input del secondo dito nella modalità di mira
        /// </summary>
        public void SecondFinger()
        {
            State = TimeState.CALIBRATION_PLASMA_GRANADE_PATH_CONTROL_POINT;
        }

        /// <summary>
        /// Lancia una Plasma Granade
        /// </summary>
        public void LaunchPlasmaGranade()
        {
            State = TimeState.EXIT_PLASMA_GRANADE_LAUNCHER;
        }

        /// <summary>
        /// Annulla la modalità di mira
        /// </summary>
        public void UndoPlasmaGranade()
        {
            State = TimeState.EXIT_PLASMA_GRANADE_LAUNCHER;
        }
    }
}
