using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Continuum.Elements;

namespace Continuum.Utilities
{
    /// <summary>
    /// Metodo per ripristinare una variabile al suo stato precedente tornando indietro nel tempo.
    /// </summary>
    /// <param name="Value"></param>
    public delegate void RewindMethod(object Value);

    /// <summary>
    /// Contenitore di tutte le costanti di gioco.
    /// </summary>
    static class Constants
    {        
        /// <summary>
        /// Indica il numero massimo di punteggi da salvare nell'Isolated Storage.
        /// </summary>
        public const int MAX_SCORES = 10;

        /// <summary>
        /// Larghezza dello schermo.
        /// </summary>
        public static int SCREEN_WIDTH;

        /// <summary>
        /// Altezza dello schermo.
        /// </summary>
        public static int SCREEN_HEIGHT;

        /// <summary>
        /// Il valore massimo che può assumere la TimeTank
        /// </summary>
        public const float MAX_TIME_TANK_VALUE = 8;

        /// <summary>
        /// Indica di quanto viene modificato il continuum ogni secondo.
        /// </summary>
        public const float CONTINUUM_LEAN = 1f;

        /// <summary>
        /// Il valore di continuum a cui si viaggia per andare avanti nel tempo.
        /// </summary>
        public const float CONTINUUM_MAX = 1f;

        /// <summary>
        /// Il valore di continuum a cui si viaggia per tornare indietro nel tempo.
        /// </summary>
        public const float CONTINUUM_MIN = -1f;

        /// <summary>
        /// Valore di soglia in secondi in cui la variazione del continuum deve cominciare a crescere, e oltre il quale il player non può cominciare un viaggio indietro nel tempo.
        /// </summary>
        public static float TIME_TANK_CRITICAL_VALUE = 0.555f;

        /// <summary>
        /// Il tempo che intercorre tra la creazione di una Smoke Animation e un'altra
        /// </summary>
        public const float SMOKE_DELAY = 0.04f;

        /// <summary>
        /// Il valore di Delta oltre il quale una Rock deve cessare di esistere
        /// </summary>
        public const float MAX_ROCK_DELTA = 40;

        /// <summary>
        /// La larghezza totale di un TachyonStream
        /// </summary>
        public const int TACHYON_STREAM_WIDTH = 120;

        /// <summary>
        /// La velocità minima di un singolo Tachyon
        /// </summary>
        public const float MIN_TACHYON_SPEED = 1000;

        /// <summary>
        /// La velocità massima di un singolo Tachyon
        /// </summary>
        public const float MAX_TACHYON_SPEED = 2000;

        /// <summary>
        /// Modifica l'attesa minore tra la creazione di un Tachyon e il successivo.
        /// </summary>
        public const float MIN_TACHYON_DELAY_INDEX = 0.001f;

        /// <summary>
        /// Modifica l'attesa maggiore tra la creazione di un Tachyon e il successivo.
        /// </summary>
        public const float MAX_TACHYON_DELAY_INDEX = 0.001f;

        /// <summary>
        /// La velocità di un Rock.
        /// </summary>
        public const float ROCK_SPEED = 50;

        /// <summary>
        /// Il valore minimo che può assumere il continuum quando è in slow down a causa di un TachyonStream
        /// </summary>
        public const float MIN_CONTINUUM_SLOW_DOWN = 0.35f;

        /// <summary>
        /// La dimensione dell'array che contiene i TimeTraveler per le collisioni.
        /// </summary>
        public const int COLLISIONS_ARRAY_LENGTH = 1000;

        /// <summary>
        /// La componente x della direzione dei proiettili che vengono sparati in obliquo.
        /// </summary>
        public const float GUN_BULLET_X_DIRECTION = 0.4f;

        /// <summary>
        /// La componente x della direzione dei rocket che vengono sparati in obliquo.
        /// </summary>
        public const float ROCKET_X_DIRECTION = 0.3f;

        /// <summary>
        /// L'apporto di tempo fornito da un singolo tachione.
        /// </summary>
        public const float TACHYON_TIME_VALUE = 0.01f;

        /// <summary>
        /// Valore minimo del modulo per la generazione del control point nella traiettoria bezierpath random
        /// </summary>
        public const int MIN_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION = 100;

        /// <summary>
        /// Valore massimo del modulo per la generazione del control point nella traiettoria bezierpath random
        /// </summary>
        public const int MAX_RANDOM_MODULE_BEZIER_CONTROL_POINT_GENERATION = 200;

        /// <summary>
        /// Valore da moltiplicare alla vita totale per determinare il valore intero in cui un nemico è considerato  danneggiato
        /// </summary>
        public const float CRITICAL_BOUND_DAMAGE = 1f / 3f;

        /// <summary>
        /// Indica di quanto viene modificato al secondo l'angolo di direzione di un rocket che insegue un nemico (in radianti).
        /// </summary>
        public const float FOLLOWING_ROCKET_RADIANS_STEP = 0.0003f;

        /// <summary>
        /// Valore massimo della vita del player.
        /// </summary>
        public const int MAX_PLAYER_LIFE = 20;

        /// <summary>
        /// Valore di vita critico
        /// </summary>
        public const int PLAYER_LIFE_CRITICAL_VALUE = (int)((MAX_PLAYER_LIFE / 3f) * 2);

        /// <summary>
        /// Il tempo (in secondi) che attende il player prima che la vita ricominci a crescere, subito dopo essere stato colpito.
        /// </summary>
        public const float PLAYER_LIFE_RISING_DELAY = 5.0f;

        /// <summary>
        /// L'incremento di vita del player in vita/secondo
        /// </summary>
        public const float PLAYER_LIFE_RISING_INCREMENT = 0.5f;

        /// <summary>
        /// La velocità di un PowerUp
        /// </summary>
        public const float POWERUP_SPEED = 60;

        /// <summary>
        /// Colore per il viaggio indietro nel tempo.
        /// </summary>
        public static Color BACK_IN_TIME_COLOR = new Color(100, 105, 150);

        /// <summary>
        /// Durata in secondi del nero iniziale, che mostra titolo e sottotitolo del livello.
        /// </summary>
        public const int INITIAL_BLACK_DURATION = 2;

        /// <summary>
        /// Durata in secondi del fade dopo il nero iniziale che mostra titolo e sottotitolo del livello.
        /// </summary>
        public const int INITIAL_FADE_DURATION = 3;

        /// <summary>
        /// Moltiplicatore da applicare al valore massimo della TimeTank per l'avanzamento veloce provocato dal Booster.
        /// </summary>
        public const float BOOSTER_TIME_TANK_MULTIPLIER = 2.0f;

        /// <summary>
        /// Durata in secondi degli stati di transizione ENTER_PLASMA_GRANADE_LAUNCHER ed EXIT_PLASMA_GRANADE_LAUNCHER.
        /// </summary>
        public const float PLASMA_GRANADE_LAUNCHER_ENTERING_EXITING_MODE_TIME = 0.4f;

        /// <summary>
        /// Tempo massimo entro il quale l'utente può rimanere in modalità di mira senza far nulla. Passato questo tempo la modalità di mira viene disattivata.
        /// </summary>
        public const float PLASMA_GRANADE_TIME_OUT = 5f;

        /// <summary>
        /// Velocità di rotazione del mirino
        /// </summary>
        public const float SCOPE_ROTATION_SPEED = 1f;

        /// <summary>
        /// Il numero di colonne della griglia che viene visualizzata quando si è nella modalità di mira.
        /// </summary>
        public const int GRID_COLUMNS = 5;

        /// <summary>
        /// Il numero di righe della griglia che viene visualizzata quando si è nella modalità di mira.
        /// </summary>
        public const int GRID_ROWS = 10;

        /// <summary>
        /// Il numero massimo di lanci contemporanei che può eseguire un Randomizer
        /// </summary>
        public const int MAX_RANDOMIZER_LAUNCHES = 1;
    }

    /// <summary>
    /// Contenitore di metodi utili all'interno del gioco
    /// </summary>
    static class Utility
    {
        static Random r = new Random();

        public static float StringToFloat(string s)
        {
            return Single.Parse(s, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Fornisce un intero randomizzato.
        /// </summary>
        /// <param name="minValue">Il valore minimo che può assumere l'intero</param>
        /// <param name="maxValue">Il valore massimo che può assumere l'intero</param>
        /// <returns>Il valore randomizzato</returns>
        public static int NextRandom(int minValue, int maxValue)
        {
            int rflush = r.Next(20);
            for (int i = 0; i < rflush; i++)
                r.Next(minValue, maxValue);
            return r.Next(minValue, maxValue);
        }

        /// <summary>
        /// Fornisce un float random
        /// </summary>
        /// <param name="minValue">Il valore minimo che può assumere il float</param>
        /// <param name="maxValue">Il valore massimo che può assumere il float</param>
        /// <returns>Il valore random</returns>
        public static float NextRandom(float minValue, float maxValue)
        {
            int rflush = r.Next(20);
            for (int i = 0; i < rflush; i++)
                r.NextDouble();
            return ((maxValue - minValue) * (float) r.NextDouble()) + minValue;
        }

        /// <summary>
        /// Fornisce un double random
        /// </summary>
        /// <param name="minValue">Il valore minimo che può assumere il double</param>
        /// <param name="maxValue">Il valore assimo che può assumere il double</param>
        /// <returns>Il valore random</returns>
        public static double NextRandom(double minValue, double maxValue)
        {
            int rflush = r.Next(20);
            for (int i = 0; i < rflush; i++)
                r.NextDouble();
            return ((maxValue - minValue) * r.NextDouble()) + minValue;
        }

        /// <summary>
        /// Crea un rettangolo a partire dal centro invece che dal punto in alto a sinistra.
        /// </summary>
        /// <param name="Center">Il vettore che indica la posizione del centro</param>
        /// <param name="Width">La larghezza del rettangolo</param>
        /// <param name="Height">L'altezza del rettangolo</param>
        /// <returns></returns>
        public static Rectangle newRectangleFromCenterPosition(Vector2 Center, int Width, int Height)
        {
            return new Rectangle((int)Center.X - Width / 2, (int)Center.Y - Height / 2, Width, Height);
        }

        /// <summary>
        /// Determina se un elemento si trova o no per almeno 1 pixel all'interno dello schermo.
        /// </summary>
        /// <param name="Position">La posizione dell'elemento.</param>
        /// <param name="Rect">Il rettangolo in cui è inscritto l'elemento.</param>
        /// <returns>Ritorna true se l'elemento è nello spazio dello schermo. Ritorna false altrimenti</returns>
        public static bool IsInScreenSpace(Rectangle Rect)
        {
            if (Rect.X > 480 || Rect.X < -Rect.Width || Rect.Y > 800 || Rect.Y < -Rect.Height)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Determina se un elemento si trova o no completamente all'interno dello schermo.
        /// </summary>
        /// <param name="Position">La posizione dell'elemento.</param>
        /// <param name="Rect">Il rettangolo in cui è inscritto l'elemento.</param>
        /// <returns>Ritorna true se l'elemento è nello spazio dello schermo. Ritorna false altrimenti</returns>
        public static bool IsTotallyInScreenSpace(Rectangle Rect)
        {
            if (Rect.X > 480 - Rect.Width || Rect.X < 0 || Rect.Y > 800 - Rect.Height || Rect.Y < 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Calcola l'angolo tra un vettore e l'asse delle x.
        /// </summary>
        /// <param name="Vector">Il vettore di cui calcolare l'angolo</param>
        /// <returns>L'angolo</returns>
        public static float CalculateXAngleFromVector(Vector2 Vector)
        {
            Vector.Normalize();
            if (Math.Sin(Vector.Y) >= 0)
            {
                return (float)Math.Acos(Vector.X);
            }
            else
            {
                return (float)(2 * Math.PI - Math.Acos(Vector.X));
            }
        }

        /// <summary>
        /// Calcola l'angolo tra due vettori, partendo dal primo.
        /// </summary>
        /// <param name="Source">Il vettore di partenza</param>
        /// <param name="Dest">Il vettore di destinazione</param>
        /// <returns>L'angolo tra Source e Dest</returns>
        public static float CalculateAngleBetweenVectors(Vector2 Source, Vector2 Dest)
        {
            float alpha = CalculateXAngleFromVector(Source);
            float beta = CalculateXAngleFromVector(Dest);
            float gamma = alpha - beta;
            return gamma >= 0 ? gamma : gamma + 2 * (float)Math.PI;
        }

        /// <summary>
        /// Indica se una direzione deve essere modificata in senso orario per avvicinarsi verso un'altra direzione.
        /// </summary>
        /// <param name="Direction">La direzione sorgente</param>
        /// <param name="Target">La direzione destinazione</param>
        /// <returns>True se deve muoversi in senso orario, false altrimenti.</returns>
        public static bool TowardsClockwise(Vector2 Direction, Vector2 Target)
        {
            return CalculateAngleBetweenVectors(Direction, Target) >= Math.PI;
        }
    }

    /// <summary>
    /// Contenitore di texture comuni a tutti gli elements anche per le animazioni
    /// </summary>
    public static class TextureConstant
    {
        /// <summary>
        /// Animazione esplosione
        /// </summary>
        public const string ANIMATION_EXPLOSION = "explosion";

        /// <summary>
        /// Animazione fumo dovuto al danneggiamento
        /// </summary>
        public const string ANIMATION_DAMAGESMOKE = "damagesmoke";

        /// <summary>
        /// Animazione stream di tachioni
        /// </summary>
        public const string ANIMATION_TACHYONSTREAM = "tachyonstream";

        /// <summary>
        /// Texture del tachione
        /// </summary>
        public const string TACHYON = "tachyon";

        /// <summary>
        /// Texture del razzo
        /// </summary>
        public const string ROCKET = "rocket";

        /// <summary>
        /// Animazione scintilla
        /// </summary>
        public const string ANIMATION_SPARKS = "sparks";

        /// <summary>
        /// Texture del proiettile
        /// </summary>
        public const string GUNBULLET = "gunbullet";

        /// <summary>
        /// Texture del proiettile dei nemici.
        /// </summary>
        public const string ENEMYBULLET = "enemybullet";

        /// <summary>
        /// Texture del razzo inseguitore
        /// </summary>
        public const string FOLLOWINGROCKET = "followingrocket";

        /// <summary>
        /// Texture del PowerUp della Gun
        /// </summary>
        public const string GUN_POWERUP = "gunpowerup";

        /// <summary>
        /// Texture del PowerUp del Rocket
        /// </summary>
        public const string ROCKET_POWERUP = "rocketpowerup";

        /// <summary>
        /// Texture del PowerUp della granata al plasma.
        /// </summary>
        public const string GRANADE_POWERUP = "granadepowerup";

        /// <summary>
        /// Texture trasparente
        /// </summary>
        public const string VOID_TEXTURE = "void";

        /// <summary>
        /// Texture della granata al plasma
        /// </summary>
        public const string PLASMAGRANADE = "plasmagranade";

        /// <summary>
        /// Texture del mirino
        /// </summary>
        public const string SCOPE = "scope";
    }

    /// <summary>
    /// Definisce il tipo di traiettoria curva che l'oggetto segue
    /// </summary>
    public enum BezierPathTrajectory
    {
        /// <summary>
        /// Traiettoria Random
        /// </summary>
        RANDOM,

        /// <summary>
        /// 
        /// </summary>
        TRAIETTORIA1,

        /// <summary>
        /// 
        /// </summary>
        TRAIETTORIA2
    }

    public enum TimeState
    {
        /// <summary>
        /// Inizio del gioco
        /// </summary>
        START_GAME_STATE,

        /// <summary>
        /// Avanti nel tempo (velocità di crociera)
        /// </summary>
        FORWARD,

        /// <summary>
        /// Si comincia a riavvolgere il tempo. Il tempo sta ancora andando avanti.
        /// </summary>
        BEGIN_REWIND,

        /// <summary>
        /// Si comincia a riavvolgere il tempo. Il tempo sta già andando indietro.
        /// </summary>
        START_REWIND,

        /// <summary>
        /// Indietro nel tempo (velocità di crociera)
        /// </summary>
        REWIND,

        /// <summary>
        /// Si comincia ad andare avanti nel tempo. Il tempo sta ancora andando indietro.
        /// </summary>
        BEGIN_FORWARD,

        /// <summary>
        /// Si comincia ad andare avanti nel tempo. Il tempo sta già andando avanti.
        /// </summary>
        START_FORWARD,

        /// <summary>
        /// Stato di avanzamento super power del tempo.
        /// </summary>
        BOOSTER,

        /// <summary>
        /// Ingresso nella modalità di lancio delle granate al plasma.
        /// </summary>
        ENTER_PLASMA_GRANADE_LAUNCHER,

        /// <summary>
        /// Stato di idle del Plasma Granade Launcer.
        /// </summary>
        PLASMA_GRANADE_LAUNCHER,

        /// <summary>
        /// Stato di calibrazione del path di bezier secondo tocco per settare il Control Point.
        /// </summary>
        CALIBRATION_PLASMA_GRANADE_PATH_CONTROL_POINT,

        /// <summary>
        /// Stato di calibrazione del path di bezier primo tocco per settare il Target Point.
        /// </summary>
        CALIBRATION_PLASMA_GRANADE_PATH_TARGET_POINT,

        /// <summary>
        /// Uscita dalla modalità di lancio delle granate al plasma.
        /// </summary>
        EXIT_PLASMA_GRANADE_LAUNCHER
    }

    /// <summary>
    /// Definisce il tipo di nemico
    /// </summary>
    public enum EnemyType
    {
        /// <summary>
        /// Definisce un nemico con livello di difficoltà normale
        /// </summary>
        NORMAL,

        /// <summary>
        /// Definisce un nemico con livello di difficoltà facile
        /// </summary>
        EASY
    }

    /// <summary>
    /// Definisce lo stato di salute di un element
    /// </summary>
    public enum LifeState
    {
        /// <summary>
        /// Indica che l'elemento è in stato di piena salute
        /// </summary>
        NORMAL,

        /// <summary>
        /// Indica che l'elemento è stato danneggiato
        /// </summary>
        DAMAGED,

        /// <summary>
        /// Indica che l'elemento ha terminato il suo ciclo di vita.
        /// </summary>
        DEAD,

        /// <summary>
        /// Indica che l'elemento è in stato di transizione
        /// </summary>
        TRANSITIONING,

        /// <summary>
        /// Indica che l'elemento sta per essere rimpiazzato
        /// </summary>
        BEINGREPLACED,

        /// <summary>
        /// Indica un elemento che DEVE essere eliminato permanentemente dal gioco
        /// </summary>
        DELETING,
    }

    /// <summary>
    /// Definisce il tipo di PowerUp
    /// </summary>
    public enum PowerUpType
    {
        /// <summary>
        /// Indica un PowerUp che potenzia l'arma.
        /// </summary>
        GUN,

        /// <summary>
        /// Indica un PowerUp che potenzia l'arma RocketLauncher (se esiste) o la genera se il player non la possiede.
        /// </summary>
        ROCKET,

        /// <summary>
        /// Indica un PowerUp che aggiunge una granata al Player.
        /// </summary>
        GRANADE,

        /// <summary>
        /// Indica un Enemy che non rilascia PowerUp
        /// </summary>
        NONE
    }

       #region TextureList
    /// <summary>
    /// Definisce una struttura dati che mette in relazione il nome di una texture con la sua posizione nell'array di textures del GameState
    /// </summary>
    public class TextureList
    {
        /// <summary>
        /// Il primo elemento della lista
        /// </summary>
        private TextureNode First { get; set; }

        /// <summary>
        /// Inserisce un elemento nella lista
        /// </summary>
        /// <param name="Texture">L'indice della texture nell'array</param>
        /// <param name="Name">Il nome che identifica la texture</param>
        public void Add(int TextureIndex, string Name)
        {
            TextureNode node = new TextureNode(TextureIndex, Name);
            node.Next = First;
            First = node;
        }

        /// <summary>
        /// Cerca una texture dalla lista
        /// </summary>
        /// <param name="Name">Il nome che identifica la texture da cercare</param>
        /// <returns>Ritorna l'indice della Texture nell'array</returns>
        public int GetTextureIndex(string Name)
        {
            if (First != null)
                return First.GetTexture(Name);
            else
                throw new NullReferenceException("La texture cercata nella lista non esiste.");
        }

        /// <summary>
        /// Definisce i nodi della lista
        /// </summary>
        private class TextureNode
        {
            /// <summary>
            /// Il prossimo elemento della lista
            /// </summary>
            public TextureNode Next { get; set; }

            /// <summary>
            /// L'indice della Texture
            /// </summary>
            public int TextureIndex { get; set; }

            /// <summary>
            /// Il nome della Texture
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Costruttore del nodo
            /// </summary>
            /// <param name="TextureIndex">L'indice della texture</param>
            /// <param name="Name">Il nome che identifica la texture</param>
            public TextureNode(int TextureIndex, string Name)
            {
                this.TextureIndex = TextureIndex;
                this.Name = Name;
            }

            /// <summary>
            /// Cerca una texture dal nodo e dai nodi seguenti
            /// </summary>
            /// <param name="Name">Il nome che identifica la texture da cercare</param>
            /// <returns>Ritorna la texture trovata</returns>
            internal int GetTexture(string Name)
            {
                if (this.Name == Name)
                    return TextureIndex;
                else
                    if (Next == null)
                        throw new NullReferenceException("La texture " + Name + " cercata nella lista non esiste");
                    else
                        return Next.GetTexture(Name);
            }
        }
    }
    #endregion
}
