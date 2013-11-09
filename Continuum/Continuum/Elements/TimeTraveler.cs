using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Continuum.Utilities;
using Continuum.State;

namespace Continuum.Elements
{
    /// <summary>
    /// Definisce una classe astratta per un generico oggetto in grado di viaggiare nel tempo.
    /// </summary>
    public abstract class TimeTraveler
    {
        /// <summary>
        /// La posizione di partenza del TimeTraveler
        /// </summary>
        public Vector2 startPosition;

        /// <summary>
        /// La rotazione di partenza del TimeTraveler
        /// </summary>
        public float startRotation;

        /// <summary>
        /// Se è impostata a true non permette l'update. Di norma è impostato a false.
        /// </summary>
        public bool blockUpdate;

        /// <summary>
        /// La lista di ElementRecord del TimeTraveler
        /// </summary>
        public LinkedList<ElementRecord> ElementRecords { get; private set; }

        /// <summary>
        /// L'ultima valutazione della posizione corrente del TimeTraveler
        /// </summary>
        public Vector2 currentPosition;

        /// <summary>
        /// L'indice della texture utilizzata per renderizzare il TimeTraveler
        /// </summary>
        public int TextureIndex;

        /// <summary>
        /// Lo stato di vita corrente dell'oggetto
        /// </summary>
        public LifeState lifeS;

        /// <summary>
        /// Lo stato del gioco
        /// </summary>
        public GameState gs;

        /// <summary>
        /// Tiene in memoria il tempo (della TimeMachine) che è passato da quando il TimeTraveler è passato in modalità "DEATH"
        /// </summary>
        public float deathCounter;

        /// <summary>
        /// L'istante in cui il TimeTraveler è passato in modalità "DEATH"
        /// </summary>
        public float deathTime;

        /// <summary>
        /// Il tempo che è passato da quando il TimeTraveler è stato creato, moltiplicato per la velocità di aggiornamento.
        /// </summary>
        public float delta;

        /// <summary>
        /// La velocità di Update del TimeTraveler.
        /// </summary>
        public float speed;

        /// <summary>
        /// La velocità di rotazione attorno all'origin
        /// </summary>
        public float rotationSpeed;

        /// <summary>
        /// Il tempo che è passato da quando il TimeTraveler è stato creato, moltiplicato per la velocità di rotazione
        /// </summary>
        public float rotationDelta;

        /// <summary>
        /// L'istante in cui è stato inizializzato il TimeTraveler
        /// </summary>
        public float StartTime { get; private set; }

        /// <summary>
        /// Il rettangolo in cui viene renderizzato il TimeTraveler
        /// </summary>
        public virtual Rectangle DestinationRectangle
        {
            get
            {
                return new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, Width, Height);
            }
        }

        /// <summary>
        /// La porzione di texture da renderizzare. Impostato a null renderizza l'intera texture.
        /// </summary>
        public Rectangle? SourceRectangle { get; protected set; }

        /// <summary>
        /// La rotazione attorno all'Origin
        /// </summary>
        public float Rotation { get; protected set; }

        /// <summary>
        /// L'origine delle trasformazioni (per es. Rotation) applicate al TimeTraveler. Di default è (0,0), e indica il punto in alto a sinistra del DestinationRectangle
        /// </summary>
        public Vector2 Origin{get;protected set;}

        /// <summary>
        /// Il tempo che è passato dalla creazione del TimeTraveler
        /// </summary>
        public float elapsedTime;

        /// <summary>
        /// Lo stato di vita corrente dell'oggetto
        /// </summary>
        public LifeState lifeState
        {
            get
            {
                return lifeS;
            }
            set
            {
                if (lifeS != LifeState.DELETING && value != LifeState.DELETING)
                {
                    if (lifeS != LifeState.DEAD && value == LifeState.DEAD)
                        deathTime = gs.levelTime.time;
                    if (gs.levelTime.continuum > 0 && value != lifeS)
                    {
                        AddElementRecord(Value => lifeS = (LifeState)Value, lifeS);
                        lifeS = value;
                    }
                }
                else
                    lifeS = LifeState.DELETING;
            }
        }

        /// <summary>
        /// Ritorna la posizione corrente del TimeTraveler
        /// </summary>
        public Vector2 CurrentPosition
        {
            get { return currentPosition; }
        }
        
        /// <summary>
        /// Ritorna la coordinata y del bordo superiore del DestinationRectangle
        /// </summary>
        public int Top
        {
            get
            {
                return DestinationRectangle.Top;
            }
        }

        /// <summary>
        /// Ritorna la coordinata y del bordo inferiore del DestinationRectangle
        /// </summary>
        public int Bottom
        {
            get
            {
                return DestinationRectangle.Bottom;
            }
        }

        /// <summary>
        /// La larghezza del TimeTraveler
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        /// L'altezza del TimeTraveler
        /// </summary>
        public int Height { get; protected set; }
        
        /// <summary>
        /// Inizializza le variabili del TimeTraveler
        /// </summary>
        /// <param name="StartPosition">La posizione di partenza</param>
        /// <param name="Speed">La velocità di aggiornamento del TimeTraveler</param>
        /// <param name="TextureName">Il nome della texture del TimeTraveler</param>
        /// <param name="gameState">Lo stato del gioco</param>
        protected void InitializeTimeTraveler(Vector2 StartPosition, float Speed, string TextureName, GameState gameState)
        {
            this.TextureIndex = gameState.textureIndices.GetTextureIndex(TextureName);
            startPosition = StartPosition;
            currentPosition = StartPosition;
            lifeS = LifeState.NORMAL;
            gs = gameState;
            deathCounter = 0;
            deathTime = 0;
            delta = 0;
            speed = Speed;
            startRotation = 0;
            rotationSpeed = 0;
            StartTime = gs.levelTime.time;
            elapsedTime = 0;
            Width = gs.textures[TextureIndex].Width;
            Height = gs.textures[TextureIndex].Height;
            Origin = new Vector2(Width/2, Height/2);
            SourceRectangle = null;
            ElementRecords = new LinkedList<ElementRecord>();
            blockUpdate = false;
        }

        /// <summary>
        /// Inizializza le variabili del TimeTraveler
        /// </summary>
        /// <param name="StartPosition">La posizione di partenza</param>
        /// <param name="Speed">La velocità di aggiornamento del TimeTraveler</param>
        /// <param name="TextureName">Il nome della texture del TimeTraveler</param>
        /// <param name="gameState">Lo stato del gioco</param>
        /// <param name="StartRotation">La rotazione di partenza del TimeTraveler</param>
        /// <param name="RotationSpeed">La velocità di rotazione del TimeTraveler</param>
        protected void InitializeTimeTraveler(Vector2 StartPosition, float Speed, string TextureName, GameState gameState, float StartRotation, float RotationSpeed)
        {
            this.TextureIndex = gameState.textureIndices.GetTextureIndex(TextureName);
            startPosition = StartPosition;
            currentPosition = StartPosition;
            lifeS = LifeState.NORMAL;
            gs = gameState;
            deathCounter = 0;
            deathTime = 0;
            delta = 0;
            speed = Speed;
            startRotation = StartRotation;
            Rotation = startRotation;
            rotationSpeed = RotationSpeed;
            StartTime = gs.levelTime.time;
            elapsedTime = 0;
            Width = gs.textures[TextureIndex].Width;
            Height = gs.textures[TextureIndex].Height;
            Origin = new Vector2(Width / 2, Height / 2);
            SourceRectangle = null;
            ElementRecords = new LinkedList<ElementRecord>();
            blockUpdate = false;
        }

        /// <summary>
        /// Esegue l'aggiornamento del TimeTraveler
        /// </summary>
        public void Update()
        {
            if (!blockUpdate)
            {
                EvaluateDelta();

                if (ElementRecords.Last != null && gs.levelTime.time - ElementRecords.Last.Value.Time > gs.timeTank)
                    ElementRecords.RemoveLast();

                while (ElementRecords.First != null && ElementRecords.First.Value.Time > gs.levelTime.time)
                {
                    ElementRecords.First.Value.Rewind();
                    ElementRecords.RemoveFirst();
                }

                currentPosition = EvaluatePosition(delta);
                Rotation = EvaluateRotation(rotationDelta);

                if (lifeState == LifeState.DEAD)
                {
                    deathCounter = gs.levelTime.time - deathTime;
                    if (deathCounter > gs.timeTank)
                        lifeState = LifeState.DELETING;
                }
            }

            if (gs.levelTime.time < StartTime)
                lifeState = LifeState.DELETING;
        }

        /// <summary>
        /// Calcola la rotazione del TimeTraveler
        /// </summary>
        protected virtual float EvaluateRotation(float RotationDelta)
        {
            return startRotation + rotationDelta;
        }

        /// <summary>
        /// Valuta il tempo trascorso e il delta. E' un metodo a parte per lasciare la possibilità di effettuare override.
        /// </summary>
        protected virtual void EvaluateDelta()
        {
            elapsedTime = gs.levelTime.time - StartTime;
            delta = elapsedTime * speed;
            rotationDelta = elapsedTime * rotationSpeed;
        }

        /// <summary>
        /// Registra un metodo che verrà invocato tornando indietro nel tempo.
        /// Questo metodo servirà per ripristinare il valore della variabile che sta per essere modificata.
        /// </summary>
        /// <param name="Rewind">Il metodo che verrà invocato tornando indietro nel tempo. Inizializzarlo così: "Value => [ istruzioni ]"</param>
        /// <param name="Value">Il valore della variabile prima che sia modificata.</param>
        public void AddElementRecord(RewindMethod Rewind, object Value)
        {
            if(gs.levelTime.continuum > 0)
                ElementRecords.AddFirst(new ElementRecord(gs.levelTime.time, Rewind, Value));
        }
        
        /// <summary>
        /// Metodo che valuta la posizione in base al tempo di gioco.
        /// Per aggiustare la velocità di aggiornamento modificare il campo "speed"
        /// </summary>
        /// <returns>La posizione corrente</returns>
        public abstract Vector2 EvaluatePosition(float Delta);

        /// <summary>
        /// Esegue le modifiche da apportare al TimeTraveler quando entra in collisione.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="arg"></param>
        public abstract void HasCollided(int Value, object arg);
    }
}
