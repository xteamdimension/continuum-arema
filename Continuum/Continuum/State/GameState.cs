using System.Collections.Generic;
using Continuum.Utilities;
using Continuum.Elements;
using Continuum.Weapons;
using Continuum.Management;
using Continuum.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Devices;

namespace Continuum.State
{
    /// <summary>
    /// L'istantanea del gioco.
    /// </summary>
    public class GameState
    {
        //Liste di oggetti del gioco
        public LinkedList<BackgroundTexture> backgrounds;
        public LinkedList<Animation> animations;
        public LinkedList<Asteroid> asteroids;
        public LinkedList<Bullet> bullets;
        public LinkedList<Bullet> newBullets;
        public LinkedList<Enemy> enemies;
        public LinkedList<Tachyon> tachyons;
        public LinkedList<PlayerState> playerStates;
        public LinkedList<PowerUp> powerUps;
        public LinkedList<Randomizer> randomizers;
        public LinkedList<ExplosionParticle> explosionParticles;
        public TachyonStream tachyonStream;
        public Gun playerGun;
        public RocketLauncher playerRocketLauncher;
        public Collisions collisions;

        //Punti per linee della plasma granade
        public Vector2[][] gridVerticalPoints;
        public Vector2[][] gridHorizontalPoints;

        //Valori multitouch per plasma granade
        public Vector2 firstFingerPosition;
        public Vector2 secondFingerPosition;
        public int? firstFinger;
        public int? secondFinger;

        public Texture2D[] textures;
        public TextureList textureIndices;
        public bool endLevel = false;
        public bool pause = false;

        //Variabili LevelManager
        public int levelPosition;
        public int levelFramesWaited;

        //Variabili del gioco
        public TimeState timeState;
        public TimeMachine levelTime;
        public TimeMachine playerTime;
        public TimeMachine albertTime;

        //Variabili di backup continuum per la pausa
        public float? alberttimecontunuumbackup;
        public float? playertimecontunuumbackup;
        public float? leveltimecontunuumbackup;
        

        public bool PerformMultitouchPlasmaGranade
        {
            get
            {
                return timeState == TimeState.CALIBRATION_PLASMA_GRANADE_PATH_CONTROL_POINT ||
                    timeState == TimeState.CALIBRATION_PLASMA_GRANADE_PATH_TARGET_POINT ||
                    timeState == TimeState.PLASMA_GRANADE_LAUNCHER ||
                    timeState == TimeState.ENTER_PLASMA_GRANADE_LAUNCHER ||
                    timeState == TimeState.EXIT_PLASMA_GRANADE_LAUNCHER;
            }
        }

        public bool PerformGestureFlickDownRewindTime
        {
            get
            {
                return timeState == TimeState.FORWARD;
            }
        }

        public bool PerformGestureFlickDownStopRewind
        {
            get
            {
                return timeState == TimeState.REWIND;
            }
        }
        //Variabili player
        private float playerLife = Constants.MAX_PLAYER_LIFE;
        public float PlayerLife
        {
            get
            { 
                return this.playerLife;
            }
            set
            {
                if (value < playerLife)
                    damageTimer = Constants.PLAYER_LIFE_RISING_DELAY;
                playerLife = value;
                if (playerLife < 0)
                    playerLife = 0;
                else if(playerLife > Constants.MAX_PLAYER_LIFE)
                    playerLife = Constants.MAX_PLAYER_LIFE;
            }
        }
        private int playerGranadeCount;
        public int PlayerGranadeCount
        {
            get
            {
                return playerGranadeCount;
            }
            set
            {
                playerGranadeCount = value;
                if (playerGranadeCount < 0)
                {
                    playerGranadeCount = 0;
                }
            }
        }
        public Vector2 playerPosition;
        public float damageTimer;
        public float timeTank = 0;
        public LifeState playerLifeState = LifeState.NORMAL;
        public bool toggleGun = true;
        public int PlayerWidth { get; private set; }
        public int PlayerHeight { get; private set; }

        //Variabili armi player
        public int granadeNumber = 100; //Numero di test, inizialmente a zero



        /// <summary>
        /// Costruttore, da invocare nel GamePage prima di istanziare gli altri elementi del gioco.
        /// </summary>
        public GameState()
        {
            backgrounds = new LinkedList<BackgroundTexture>();
            asteroids = new LinkedList<Asteroid>();
            bullets = new LinkedList<Bullet>();
            newBullets = new LinkedList<Bullet>();
            enemies = new LinkedList<Enemy>();
            tachyons = new LinkedList<Tachyon>();
            playerStates = new LinkedList<PlayerState>();
            animations = new LinkedList<Animation>();
            powerUps = new LinkedList<PowerUp>();
            randomizers = new LinkedList<Randomizer>();
            explosionParticles = new LinkedList<ExplosionParticle>();
            playerPosition = new Vector2(Constants.SCREEN_WIDTH / 2, 3 * Constants.SCREEN_HEIGHT / 4);
            levelPosition = 0;
            levelFramesWaited = 0;
            playerGranadeCount = 0;
            damageTimer = 0;
            levelTime = new TimeMachine();
            playerTime = new TimeMachine();
            albertTime = new TimeMachine();
            collisions = new Collisions();
            textureIndices = new TextureList();
            playerGun = new Gun(true, this);
            playerRocketLauncher = new RocketLauncher(true, this);
            gridVerticalPoints = new Vector2[Constants.GRID_COLUMNS + 1][];
            gridHorizontalPoints = new Vector2[Constants.GRID_ROWS + 1][];
            for (int i = 0; i < gridVerticalPoints.Length; i++)
            {
                gridVerticalPoints[i] = new Vector2[2];
            }
            for (int i = 0; i < gridHorizontalPoints.Length; i++)
            {
                gridHorizontalPoints[i] = new Vector2[2];
            }
        }

        /// <summary>
        /// Handler per la gestione dell'evento di creazione un nuovo proiettile
        /// </summary>
        /// <param name="Position">Posizione di partenza del proiettile</param>
        /// <param name="Direction">Direzione che seguirà il proiettile</param>
        /// <param name="weapon">Istanza dell'arma che ha generato il proiettile</param>
        /// <param name="player">Boolean che indica se il proiettile è stato generato dal Player.</param>
        public void newGunBullet(Vector2 Position, Vector2 Direction, Gun gun)
        {
            Bullet b;
            b = new GunBullet(Position, Direction, gun, this);
            bullets.AddFirst(b);
            collisions.Insert(b);
        }

        /// <summary>
        /// Handler per la gestione dell'evento di creazione un nuovo rocket
        /// </summary>
        /// <param name="Position">Posizione di partenza del rocket</param>
        /// <param name="Direction">Direzione che seguirà il rocket</param>
        /// <param name="weapon">Istanza dell'arma che ha generato il rocket</param>
        /// <param name="player">Boolean che indica se il rocket è stato generato dal Player.</param>
        public void newRocket(Vector2 Position, Vector2 Direction, RocketLauncher rocketLauncher)
        {
            Bullet r;
            r = new Rocket(Position, Direction, rocketLauncher, this);
            bullets.AddFirst(r);
            collisions.Insert(r);
        }

        /// <summary>
        /// Handler per la gestione dell'evento di creazione un nuovo rocket
        /// </summary>
        /// <param name="Position">Posizione di partenza del rocket</param>
        /// <param name="Direction">Direzione che seguirà il rocket</param>
        /// <param name="weapon">Istanza dell'arma che ha generato il rocket</param>
        /// <param name="player">Boolean che indica se il rocket è stato generato dal Player.</param>
        public void newFollowingRocket(Vector2 Position, Vector2 Direction, RocketLauncher rocketLauncher)
        {
            Bullet r;
            r = new FollowingRocket(Position, Direction, rocketLauncher, this);
            bullets.AddFirst(r);
            collisions.Insert(r);
        }

        /// <summary>
        /// Handler per la gestione dell'evento di creazione di un nuovo asteroide
        /// </summary>
        /// <param name="XPosition">La posizione orizzontale da cui parte l'asteroide (che viene creato sempre in alto oltre i limiti dello schermo)</param>
        /// <param name="speed">La velocità dell'asteroide</param>
        /// <param name="damage">I punti vita dell'asteroide (Rappresenta anche il danno che reca l'asteroide quando entra in collisione con gli altri oggetti)</param>
        public void newAsteroid(int XPosition, int speed, int life, string Texture)
        {
            float rotation = (float)Utility.NextRandom(0, 50) / (float)Utility.NextRandom(1, 100);
            float XDirection = 0;
            if (XPosition < 20)
                XDirection = 0.5f;
            else if (XPosition > 460)
                XDirection = -0.5f;
            Asteroid a = new Asteroid(new Vector2(XPosition, 0), new Vector2(XDirection, 1), speed, life, Texture, this);
            asteroids.AddFirst(a);
            collisions.Insert(a);
        }

        /// <summary>
        /// Crea un nuovo BackgroundTexture
        /// </summary>
        /// <param name="Level">Il livello di profondità in cui viene renderizzata la texture</param>
        /// <param name="Speed">La velocità di scorrimento della texture</param>
        /// <param name="Texture">Il nome della texture</param>
        /// <param name="TransitionTexture">Il nome della texture di transizione qualora fosse necessaria. Inserire null se non è prevista una texture di transizione</param>
        public void newBackgroundTexture(int Level, int Speed, string Texture, string TransitionTexture)
        {
            backgrounds.AddLast(new BackgroundTexture(Level, Speed, Texture, TransitionTexture, this));
        }

        /// <summary>
        /// Crea una nuova Animation
        /// </summary>
        /// <param name="Position">La posizione dell'animazione</param>
        /// <param name="SequenceTexture">Posizione della texture nell'array delle texture</param>
        /// <param name="NumberOfFrames">Il numero totale di frame della sequenza</param>
        /// <param name="NumberOfRows">Il numero di righe della matrice di frame della sequenza</param>
        /// <param name="NumberOfCols">IL numero di colonne della matrice di frame della sequenza</param>
        public void newAnimation(Vector2 Position, string SequenceTexture, int NumberOfFrames, int NumberOfRows, int NumberOfCols, int FramePerSecond, float RotationStartRadiant, float RotationSpeed)
        {
            animations.AddLast(new Animation(Position, SequenceTexture, NumberOfFrames, NumberOfRows, NumberOfCols, FramePerSecond, RotationStartRadiant, RotationSpeed, this));
        }

        /// <summary>
        /// Crea un nuovo danno ad area
        /// </summary>
        /// <param name="Position">La posizione del danno ad area</param>
        /// <param name="SequenceTexture">La texture dell'animazione associata al danno ad area</param>
        /// <param name="NumberOfFrames">Il numero di frames dell'animazione</param>
        /// <param name="NumberOfRows">Il numero di righe dell'animazione</param>
        /// <param name="NumberOfCols">Il numero di colonne dell'animazione</param>
        /// <param name="DamageDuration">La durata del danno ad'area</param>
        /// <param name="Range">Il raggio del danno ad area</param>
        /// <param name="Damage">Il danno provocato in ogni istante dal danno ad area</param>
        public void newAreaDamage(Vector2 Position, string SequenceTexture, int NumberOfFrames, int NumberOfRows, int NumberOfCols, float DamageDuration, float Range, int Damage)
        {
            AreaDamage ad = new AreaDamage(Position, SequenceTexture, NumberOfFrames, NumberOfRows, NumberOfCols, DamageDuration, Range, Damage, this);
            collisions.Insert(ad);
            newBullets.AddLast(ad);
        }

        /// <summary>
        /// Crea una nuova granata al plasma
        /// </summary>
        /// <param name="Path">Il percorso seguito dalla granata al plasma</param>
        public void newPlasmaGranade(QuadraticBezierCurve Path)
        {
            PlasmaGranade pg = new PlasmaGranade(Path, TextureConstant.PLASMAGRANADE, this);
            collisions.Insert(pg);
            bullets.AddLast(pg);
        }

        /// <summary>
        /// Crea un nuovo flusso di tachioni
        /// </summary>
        /// <param name="xPosition">La posizione orizzontale del flusso di tachioni</param>
        /// <param name="duration">La durata del flusso di tachioni</param>
        /// <param name="sequenceTexture">L'animazione del flusso di tachioni</param>
        public void newTachyonStream(int xPosition, float duration, string sequenceTexture)
        {
            TachyonStream ts = new TachyonStream(xPosition, duration, sequenceTexture, this);
            animations.AddLast(ts);
            tachyonStream = ts;
        }

        /// <summary>
        /// Crea un'esplosione (che non fa danno)
        /// </summary>
        /// <param name="position">La posizione dell'esplosione</param>
        public void newExplosion(Vector2 position)
        {
            for (int i = 0; i < 15; i++)
            {
                explosionParticles.AddLast(new ExplosionParticle(position, this));
            }
        }

        /// <summary>
        /// Crea un nuovo nemico
        /// </summary>
        /// <param name="StartPosition">La posizione di partenza del nemico</param>
        /// <param name="Speed">La velocità del nemico</param>
        /// <param name="Texture">La texture del nemico</param>
        /// <param name="Weapon">L'arma del nemico</param>
        /// <param name="Life">La vita del nemico</param>
        /// <param name="PowerUpType">Il tipo di PowerUp lasciato dal nemico</param>
        public void newEnemy(Vector2 StartPosition, float Speed, string Texture, string Weapon, int Life, PowerUpType PowerUpType)
        {
            IWeapons NewWeapon;
            switch (Weapon)
            {
                case "Gun":
                    NewWeapon = new Gun(false, this);
                    break;
                case "RocketLauncher":
                    NewWeapon = new RocketLauncher(false, this);
                    break;
                default:
                    throw new NotImplementedException("Arma " + Weapon + "non esistente.");
            }
            Enemy e = new Enemy(StartPosition, Speed, Texture, NewWeapon, Life, this, PowerUpType);
            enemies.AddLast(e);
            collisions.Insert(e);
        }

        /// <summary>
        /// Crea un nuovo AsteroidRandomizer
        /// </summary>
        /// <param name="probability">La probabilità che avvenga un lancio per ogni secondo</param>
        /// <param name="probabilityIncrementPerMinute">L'aumento della probabilità per ogni minuto. Per il valore predefinito inserire null.</param>
        /// <param name="probabilityMax">Il massimo valore che può assumere la probabilità. Per il valore predefinito inserire null.</param>
        /// <param name="lifeRandomVariable">Una variabile aleatoria per la scelta della vita degli asteroidi</param>
        /// <param name="speedRandomVariable">Una variabile aleatoria per la scelta della velocità degli asteroidi</param>
        /// <param name="texture">La texture degli asteroidi lanciati</param>
        public void newAsteroidRandomizer (float probability, float? probabilityIncrementPerMinute, float? probabilityMax, DynamicNormalRandomVariable speedRandomVariable, DynamicNormalRandomVariable lifeRandomVariable ,string texture)
        {
            this.randomizers.AddLast(new AsteroidRandomizer(probability, probabilityIncrementPerMinute, probabilityMax, speedRandomVariable, lifeRandomVariable, texture, this));
        }

        /// <summary>
        /// Crea un nuovo PowerUp
        /// </summary>
        /// <param name="Position">La posizione del PowerUp</param>
        /// <param name="Type">Il tipo di PowerUp</param>
        public void newPowerUp(Vector2 Position, PowerUpType Type)
        {
            string texture;
            switch (Type)
            {
                case PowerUpType.GUN:
                    texture = TextureConstant.GUN_POWERUP;
                    break;
                case PowerUpType.ROCKET:
                    texture = TextureConstant.ROCKET_POWERUP;
                    break;
                case PowerUpType.GRANADE:
                    texture = TextureConstant.GRANADE_POWERUP;
                    break;
                default:
                    throw new NotImplementedException("Tipo di PowerUp non ancora implementata");
            }
            PowerUp p = new PowerUp(Position, Type, texture, this);
            powerUps.AddLast(p);
            collisions.Insert(p);
        }

        /// <summary>
        /// Crea un nuovo EnemyRandomizer
        /// </summary>
        /// <param name="probability">La probabilità che avvenga un lancio per ogni secondo</param>
        /// <param name="probabilityIncrementPerMinute">L'aumento della probabilità per ogni minuto</param>
        /// <param name="probabilityMax">Il massimo valore che può assumere la probabilità</param>
        /// <param name="powerUpProbabilityPerLaunch">La probabilità che un nemico lanciato possa avere un PowerUp</param>
        /// <param name="rocketPowerUpProbability">La probabilità che un nemico lanciato con PowerUp possegga un PowerUp di tipo Rocket</param>
        /// <param name="granadePowerUpProbability">La probabilità che un nemico lanciato con PowerUp possegga un PowerUp di tipo granade</param>
        /// <param name="lifeRandomVariable">Una variabile aleatoria per la scelta della vita dei nemici</param>
        /// <param name="speedRandomVariable">Una variabile aleatoria per la scelta della velocità dei nemici</param>
        /// <param name="texture">La texture dei nemici lanciati</param>
        public void newEnemyRandomizer(float probability, float? probabilityIncrementPerMinute, float? probabilityMax, float? powerUpProbabilityPerLaunch, float? rocketPowerUpProbability, float? granadePowerUpProbability, DynamicNormalRandomVariable speedRandomVariable, DynamicNormalRandomVariable lifeRandomVariable, string weapon, string texture)
        {
            this.randomizers.AddLast(new EnemyRandomizer(probability, probabilityIncrementPerMinute, probabilityMax, powerUpProbabilityPerLaunch, rocketPowerUpProbability, granadePowerUpProbability, speedRandomVariable, lifeRandomVariable, weapon, texture, this));
        }

        
        /// <summary>
        /// Crea un nuovo TachyonStreamRandomizer
        /// </summary>
        /// <param name="probability">La probabilità che avvenga un lancio per ogni secondo</param>
        /// <param name="probabilityIncrementPerMinute">L'aumento della probabilità per ogni minuto</param>
        /// <param name="probabilityMax">Il massimo valore che può assumere la probabilità</param>
        /// <param name="durationRandomVariable">Una variabile aleatoria per la scelta della durata del TachyonStream</param>
        /// <param name="texture">La SequenceTexture dei TachyonStream lanciati</param>
        public void newTachyonStreamRandomizer(float probability, float? probabilityIncrementPerMinute, float? probabilityMax, DynamicNormalRandomVariable durationRandomVariable, string texture)
        {
            this.randomizers.AddLast(new TachyonStreamRandomizer(probability, probabilityIncrementPerMinute, probabilityMax, durationRandomVariable, texture, this));
        }

        public void SetPlayerBounds()
        {
            Texture2D tx = textures[textureIndices.GetTextureIndex("playership")];
            PlayerWidth = tx.Width;
            PlayerHeight = tx.Height;
        }

        public void PlayerHasCollided(int damage, int gunDowngrade, int rocketLauncherDowngrade)
        {
            if (playerLifeState != LifeState.DEAD)
            {
                PlayerLife -= damage;
                VibrateController.Default.Start(new TimeSpan(0, 0, 0, 0, damage * 50));
                playerGun.Upgrade(-gunDowngrade);
                if (playerRocketLauncher.Level > 0)
                    playerRocketLauncher.Upgrade(-rocketLauncherDowngrade);
                else
                    playerGun.Upgrade(-rocketLauncherDowngrade);
            }
        }
    }
}
